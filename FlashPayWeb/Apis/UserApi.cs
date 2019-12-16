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

namespace FlashPayWeb.Apis
{
    public class UserApi : BaseApi
    {
        public UserApi(string node) : base(node)
        {
        }


        protected override JArray ProcessRes(JsonRPCrequest req)
        {
            JArray result = new JArray();
            result = base.ProcessRes(req);

            switch (req.method)
            {
                case "getEthAddr":
                    {
                        string userName = (string)req.@params[0];
                        string password = (string)req.@params[1];
                        UserKey uk = new UserKey() { Password = password, Username = userName };
                        
                        //先验证这个账户有没有申请过私钥
                        User user = Singleton.Store.GetUsers().TryGet(uk);
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
                            snapshot.Users.Add(uk,u);
                            snapshot.Commit();
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
                        var transactionReceipt = transferHandler.SendRequestAndWaitForReceiptAsync("0x2774c07591067523cc72cee4876620e9d304268c", transfer);
                        result = getJAbyKV("result",true);
                        break;
                    }
            }
            return result;
        }
    }
}
