using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using System.Linq;

namespace Sui.Tests
{
    public class NormalizedModuleTest
    {
        TestToolbox Toolbox;
        string DefaultPackage = "0x2";
        string DefaultModule = "coin";
        string DefaultFunction = "balance";
        string DefaultStruct = "Coin";

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();
        }

        [UnityTest]
        public IEnumerator MoveFunctionArgTypeFetchTest()
        {
            Task<RpcResult<MoveFunctionArgTypes>> arg_types_task = this.Toolbox.Client.GetMoveFunctionArgTypes(this.DefaultPackage, this.DefaultModule, this.DefaultFunction);
            yield return new WaitUntil(() => arg_types_task.IsCompleted);

            Assert.IsTrue(arg_types_task.Result.Result.ArgTypes.SequenceEqual(new MoveFunctionArgType[]
            {
                new MoveFunctionArgType(ArgumentType.Object, ObjectValueType.ByImmutableReference)
            }));
        }

        [UnityTest]
        public IEnumerator MoveFunctionFetchByPackageTest()
        {
            Task<RpcResult<System.Collections.Generic.Dictionary<string, SuiMoveNormalizedModule>>> package_task = this.Toolbox.Client.GetNormalizedMoveModulesByPackage(this.DefaultPackage);
            yield return new WaitUntil(() => package_task.IsCompleted);
            Assert.IsTrue(package_task.Result.Result.ContainsKey(this.DefaultModule));
        }

        [UnityTest]
        public IEnumerator NormalizedMoveModuleFetchTest()
        {
            Task<RpcResult<SuiMoveNormalizedModule>> normalized_move_module_task = this.Toolbox.Client.GetNormalizedMoveModule(this.DefaultPackage, this.DefaultModule);
            yield return new WaitUntil(() => normalized_move_module_task.IsCompleted);
            Assert.IsTrue(normalized_move_module_task.Result.Result.ExposedFunctions.ContainsKey(this.DefaultFunction));
        }

        [UnityTest]
        public IEnumerator NormalizedMoveFunctionFetchTest()
        {
            Task<RpcResult<NormalizedMoveFunctionResponse>> normalized_move_function_task = this.Toolbox.Client.GetNormalizedMoveFunction
            (
                this.DefaultPackage,
                this.DefaultModule,
                this.DefaultFunction
            );
            yield return new WaitUntil(() => normalized_move_function_task.IsCompleted);
            Assert.IsFalse(normalized_move_function_task.Result.Result.IsEntry);
        }

        [UnityTest]
        public IEnumerator NormalizedMoveStructFetchTest()
        {
            Task<RpcResult<SuiMoveNormalizedStruct>> normalized_move_struct_test = this.Toolbox.Client.GetNormalizedMoveStruct
            (
                this.DefaultPackage,
                this.DefaultModule,
                this.DefaultStruct
            );
            yield return new WaitUntil(() => normalized_move_struct_test.IsCompleted);
            Assert.Greater(normalized_move_struct_test.Result.Result.Fields.Count(), 1);
        }
    }
}
