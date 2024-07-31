namespace Sui.Utilities
{
    public abstract class ResultBase<T>
    {
        public T Result { get; }

        public ErrorBase Error { get; }

        public ResultBase
        (
            T result,
            ErrorBase error
        )
        {
            this.Result = result;
            this.Error = error;
        }
    }

    public class SuiResult<T> : ResultBase<T>
    {
        public SuiResult(T result, ErrorBase error = null) : base(result, error) { }

        public static SuiResult<T> GetSuiErrorResult(string message)
            => new SuiResult<T>(default, new SuiError(-1, message, null));
    }
}