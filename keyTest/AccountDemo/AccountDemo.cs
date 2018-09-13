using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using Nethereum.Web3.Accounts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
using Nethereum.Signer;
using System.Numerics;
using System.Threading;
using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.KeyStore;
using Nethereum.RPC.Eth.DTOs;

namespace AccountDemo
{
    public class AccountDemo
    {
        public async void Run()
        {
            //CreateAccountFromkey();
            //Console.WriteLine("start to create online account");
            // CreateManagedAccount();
            //Console.WriteLine("start to create online account11111");
            // CreateManagedAccount1();

            // CreateHDWallet();
           createAccountFromKeyStory();
           //TransactionTest();
        }


        //从私钥创建账号
        public void CreateAccountFromkey()
        {
            Console.WriteLine("create offline account from private key");
            var keyPairs = EthECKey.GenerateKey();
            var privateKey = keyPairs.GetPrivateKey();
            var chainId = new BigInteger(1234);
            var account = new Account(privateKey, chainId);
            Console.WriteLine("address=>" + account.Address);
            Console.WriteLine("TransactionManager=>" + account.TransactionManager);

        }

        // 创建在线管理账号
        public async void CreateManagedAccount()
        {
            Console.WriteLine("create online account ........");
            var web3 = new Web3("http://localhost:7545");
            string[] accounts = await web3.Eth.Accounts.SendRequestAsync();
            ManagedAccount account = new ManagedAccount(accounts[0], "");
            Console.WriteLine("  Address => " + account.Address);
            Console.WriteLine("  TransactionManager => " + account.TransactionManager);

            

        }
        // 创建在线管理账号
        public async void CreateManagedAccount1()
        {
            Console.WriteLine("create online the really account");
            var web3 = new Web3("http://localhost:7545");
            string account1 = await web3.Personal.NewAccount.SendRequestAsync("123456");
            Console.WriteLine("Address=>" + account1);
            //  var succ = await web3.Personal.UnlockAccount.SendRequestAsync(account1,"123456",600);
            //     Console.WriteLine("UnlockAccount is ", succ.ToString());

            var accounts = await web3.Eth.Accounts.SendRequestAsync();
            Console.WriteLine("all the account count is {0} ", accounts.Length);


        }
    
        // 创建钱包
        public const string Words = "ripple scissors kick mammal hire column oak again sun offer wealth tomorrow wagon turn fatal";
        public const string Password = "TREZOR";
        public void CreateHDWallet()
        {
            // 通过已知的助记词生成账号
            var wallet = new Wallet(Words,Password);
            var account = wallet.GetAccount(0);
            Console.WriteLine(account.Address);
            var prk = wallet.GetPrivateKey(account.Address);//获取账号对应的秘钥
            var accprk = account.PrivateKey;
            Console.WriteLine("the prk is {0}",prk.ToHex());
            Console.WriteLine("the accprk is {0}",accprk);
            // 第一次创建钱包 生成随机的助记词 并保存
            var mnemonic= new Mnemonic(Wordlist.English,WordCount.Twelve);
            
            Console.WriteLine(mnemonic.ToString());
            var masterKey = mnemonic.DeriveExtKey("123456"); //生成一组组秘钥对   可以通过该秘钥继续生成子孙密钥对
            var privateKey = masterKey.PrivateKey;//获取的是bitcoin的私钥

            var  wallet1 = new  Wallet(mnemonic.ToString(),"123456");
            Console.WriteLine("the wallet1 mnemonic is {0}", mnemonic.ToString());
            var account1 = wallet1.GetAccount(0);
            Console.WriteLine("the wallet1 account is {0}",account1.Address);
            var keySvc = new KeyStoreService();
            var json= keySvc.EncryptAndGenerateDefaultKeyStoreAsJson("123456", account1.PrivateKey.HexToByteArray(),
                account1.Address);
            var path = string.Format(@"e:\{0}.json", keySvc.GenerateUTCFileName(account1.Address));
            File.WriteAllText(path,json);
           
        }

        //创建账号离线签名转账 
        public async void createAccountFromKeyStory()
        {
            var path = @"e:\UTC--2018-09-13T11-21-18.5956837Z--40ef545B54730D564424D8c629878c018D1999A9.json";
            var keySvc = new KeyStoreService();
            var json = File.ReadAllText(path);
            var rkey = keySvc.DecryptKeyStoreFromJson("123456", json);
            var addr = keySvc.GetAddressFromKeyStore(json);
            Console.WriteLine("the ketStore account is{0}", addr);
   
            var web3= new Web3("http://localhost:7545");
            var nonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(addr);
            var to = "0x2BBA1062c3Ba7Fab710a96D68CF7d74B7830f679";
            var value = new  BigInteger(2000000000000000000);
            var signed = "0x" + Web3.OfflineTransactionSigner.SignTransaction(rkey.ToHex(),to,value,nonce);

            var txhash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signed);
            var receipt =
                await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txhash);

            var balance = await web3.Eth.GetBalance.SendRequestAsync(addr);

            Console.WriteLine("the balance is{0}",balance.Value);



        }


        //普通转账
        public async void TransactionTest()
        {
            var web3 = new Web3("http://localhost:7545");

            string[] accounts = await web3.Eth.Accounts.SendRequestAsync();
            var prk = "52d742d112d2c172d4c006e8a3a8ad11d149ab4a341f0a433a7c9e27d64fe216";
            TransactionInput txinput = new TransactionInput
            {
                From = accounts[0],
                To = "0x40ef545B54730D564424D8c629878c018D1999A9",
                Value = new HexBigInteger(1000000000000000000)
            };
            var txhash = await web3.Eth.Transactions.SendTransaction.SendRequestAsync(txinput);
            var balance = await web3.Eth.GetBalance.SendRequestAsync("0x40ef545B54730D564424D8c629878c018D1999A9");
            Console.WriteLine("transaction success");
        }
    }

}
