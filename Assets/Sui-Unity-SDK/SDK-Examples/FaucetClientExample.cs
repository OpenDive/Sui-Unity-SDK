using System.Collections;
using System.Collections.Generic;
using Sui.Clients;
using Sui.Rpc;
using UnityEngine;

public class FaucetClientExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //RequestFaucet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //async void RequestFaucet()
    //{
    //    string address = "0x4ebe7aef1474166caa8ce2dd5bd77d72469780c91b18eb424d6211510bc2ca98";
    //    var faucet = new FaucetClient(Constants.DevnetConnection);
    //    var success = await faucet.AirdropGasAsync(address);

    //    Debug.Log($"Airdropped to {address}. Success: {success}");
    //}
}
