using FlashPayWeb.RPC;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json.Linq;
using FlashPayWeb.Persistence;
using FlashPayWeb.IO;
using FlashPayWeb.libs;
using System.Numerics;
using Nethereum.Web3;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPayWeb.Apis
{
    public class UserApi : BaseApi
    {
        public UserApi(string node) : base(node)
        {
        }


        protected async override Task<JArray> ProcessRes(JsonRPCrequest req)
        {
            JArray result = new JArray();
            result =await base.ProcessRes(req);

            switch (req.method)
            {
                case "getEthAddr":
                    {
                        string userName = (string)req.@params[0];
                        string password = (string)req.@params[1];
                        JObject jo = new JObject();
                        jo.Add(new JProperty("UserName",userName));
                        jo.Add(new JProperty("Password", password));
                        string hexStr = Conversion.Bytes2HexString(Encoding.UTF8.GetBytes(jo.ToString()));
                        UInt256 hash = Singleton.EthHelper.GetSha3(hexStr);
                        //先验证这个账户有没有申请过私钥
                        User user = Singleton.Store.GetUsers().TryGet(hash);
                        if (user != null)
                        {
                            result = getJAbyJ(user.ToJson());
                            break;
                        }
                        //随机产生一个私钥
                        var ecKey = EthECKey.GenerateKey();
                        var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
                        var account = new Account(privateKey);

                        //向爬虫汇报地址
                        bool r = Singleton.CrawlerHelper.AddCareAddress(new UInt160(account.Address));
                        if (!r)
                        {
                            result = getJAbyKV("error","addCareAddress faild");
                            break;
                        }
                        //将私钥存储在db中
                        using (Snapshot snapshot = Singleton.Store.GetSnapshot())
                        {
                            User u = new User() { Password = password, Username = userName, PrivateKey = account.PrivateKey, Address = new UInt160(account.Address) };
                            snapshot.Users.Add(hash, u);
                            snapshot.Commit();
                            //给这个地址转一笔eth方便归集的时候支付手续费
                            var receipt =await Singleton.Web3Ins.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(account.Address, 0.05m, 2);
                            System.Console.WriteLine(receipt.TransactionHash);
                            result = getJAbyJ(u.ToJson());
                        }
                        break;
                    }
                case "userTransfer":
                    {
                        var receiverAddress = (string)req.@params[0];
                        var amount = BigInteger.Parse((string)req.@params[1]);
                        var tag = (string)req.@params[2];
                        var signData = (string)req.@params[3];
                        var transferHandler = Singleton.Web3Ins.Eth.GetContractTransactionHandler<TransferFunction>();
                        var transfer = new TransferFunction()
                        {
                            To = receiverAddress,
                            TokenAmount = amount
                        };
                        var transactionHash = await transferHandler.SendRequestAsync("0x2774c07591067523cc72cee4876620e9d304268c", transfer);
                        result = getJAbyKV("result", transactionHash);
                        break;
                    }
                case "getAllAddrs":
                    {
                        JArray ja = new JArray();
                        ja.Add(Singleton.Store.GetUsers().Find().Select(p=>p.Value.ToJson_OnlyAddr()));
                        result = ja;
                        break;
                    }
                case "collection":
                    {
                        List<User> users = new List<User>();
                        users.AddRange(Singleton.Store.GetUsers().Find().Select(p => p.Value));
                        JArray ja = new JArray();
                        for (var i = 0; i < users.Count; i++)
                        {
                            var privateKey = users[i].PrivateKey;
                            var account = new Account(privateKey);
                            var web3 = new Web3(account, Setting.Ins.EthCliUrl);

                            var contract = web3.Eth.GetContract(Setting.Ins.ABI, "0x2774c07591067523cc72cee4876620e9d304268c");
                            var f = contract.GetFunction("balanceOf");
                            var amount = await f.CallAsync<BigInteger>(account.Address);
                            if (amount == 0)
                                continue;
                            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();
                            var transfer = new TransferFunction()
                            {
                                To = "0x4b8db98D09e35D85E501321b68195F9f23EfDd87",
                                TokenAmount = amount
                            };
                            var transactionHash = await transferHandler.SendRequestAsync("0x2774c07591067523cc72cee4876620e9d304268c", transfer);
                            ja.Add(new JObject() { { account.Address,transactionHash} });
                        }
                        result = ja;
                        break;
                    }
            }
            return result;
        }


    }
}
