namespace Sui.Rpc.Models
{
    public class SuiObjectResponse
    {
        public ObjectData Data { get; set; }
        public ObjectResponseError Error { get; set; }
    }
}