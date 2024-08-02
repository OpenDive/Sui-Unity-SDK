namespace Sui.Utilities
{
    /// <summary>
    /// A base class for representing an error value for the given result.
    /// </summary>
    public abstract class ReturnBase
    {
        /// <summary>
        /// Won't be null if there were any errors thrown when utilizing the class.
        /// </summary>
        public ErrorBase Error { get; protected internal set; }

        protected internal T SetError<T, U>(T item, string message, object data = null) where U : ErrorBase, new()
        {
            this.Error = new U();

            this.Error.Code = 0;
            this.Error.Message = message;
            this.Error.Data = data;

            return item;
        }

        protected internal T SetError<T>(T item, ErrorBase error)
        {
            this.Error = error;
            return item;
        }

        protected internal void SetError<T>(string message, object data = null) where T : ErrorBase, new()
        {
            this.Error = new T();

            this.Error.Code = 0;
            this.Error.Message = message;
            this.Error.Data = data;
        }
    }
}