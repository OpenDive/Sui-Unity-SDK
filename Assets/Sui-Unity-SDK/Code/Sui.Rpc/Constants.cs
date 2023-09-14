namespace Sui.Rpc
{
    public class Constants
    {
        public static Connection LocalnetConnection = new Connection(
            "http://127.0.0.1:9000",
            "http://127.0.0.1:9123/gas"
        );

        public static Connection DevnetConnection = new Connection(
            "https://fullnode.devnet.sui.io:443/",
            "https://faucet.devnet.sui.io/gas"
        );

        public static Connection TestnetConnection = new Connection(
            "https://fullnode.testnet.sui.io:443/",
            "https://faucet.testnet.sui.io/gas"
        );

        public static Connection MainnetConnection = new Connection(
            "https://fullnode.mainnet.sui.io:443/"
        );
    }

    public class Connection {
        public string FULL_NODE { set; get; }
        public string FAUCET { set; get; }

        public Connection(string fullNodeUrl, string faucet = null)
        {
            FULL_NODE = fullNodeUrl;
            FAUCET = faucet;
        }
    }
}