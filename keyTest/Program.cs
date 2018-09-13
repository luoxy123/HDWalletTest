using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Web3;
using System;
namespace keyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            EthECKey keyPair = EthECKey.GenerateKey();
            Console.WriteLine("private key =>" + keyPair.GetPrivateKey());
            Console.WriteLine("public key=>" + keyPair.GetPubKey().ToHex());
            Console.WriteLine("address=>" + keyPair.GetPublicAddress());
            var wordlist = "brass bus same payment express already energy direct type have venture afraid";

            var privateKey = keyPair.GetPrivateKey();
            var addresss = keyPair.GetPublicAddress();
            Web3 web3 = new Web3("http://localhost:7545");
            var nonce = web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(addresss).GetAwaiter().GetResult();






            Console.ReadLine();
        }
    }
}
 