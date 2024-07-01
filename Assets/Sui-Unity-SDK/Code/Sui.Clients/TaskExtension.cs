using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Sui.Clients
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;
        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("CoroutineRunner");
                    _instance = obj.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }
    }

    public static class TaskExtensions
    {
        public static Task Await(this YieldInstruction instruction)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            CoroutineRunner.Instance.StartCoroutine(AwaitCoroutine(instruction, taskCompletionSource));

            return taskCompletionSource.Task;
        }

        private static IEnumerator AwaitCoroutine(YieldInstruction instruction, TaskCompletionSource<bool> taskCompletionSource)
        {
            yield return instruction;
            taskCompletionSource.SetResult(true);
        }
    }
}
