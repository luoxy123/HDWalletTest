using System;
using Nethereum.Signer;
using Nethereum.KeyStore;
using System.IO;
namespace KeyStore
{
   
    class Program
    { 
        static void Main(string[] args)
        {
            EthECKey ethECKey = EthECKey.GenerateKey();
            byte[] key = ethECKey.GetPrivateKeyAsBytes();
            var address = ethECKey.GetPublicAddress();
            string password = "12345";
            var ksvc = new KeyStoreService();
            string json = ksvc.EncryptAndGenerateDefaultKeyStoreAsJson(password, key, address);
            var ksfn = string.Format(@"e:\{0}.json", ksvc.GenerateUTCFileName(address));
            
            File.WriteAllText(ksfn,json);



        }
    }
}
