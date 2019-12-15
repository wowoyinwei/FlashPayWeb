using FlashPayWeb.RPC;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json.Linq;
using FlashPayWeb.Persistence;
using FlashPayWeb.IO;
using FlashPayWeb.libs;

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
                case "createAddress":
                    {
                        string userName = (string)req.@params[0];
                        string password = (string)req.@params[1];

                        //var privateKeyOwner = "88f537ddc37758c32c793cbcf84d153e29747c1d23ac66783b9937e445118bee";
                        //var owner = new Account(privateKeyOwner);

                        //随机产生一个私钥
                        var ecKey = EthECKey.GenerateKey();
                        var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
                        var account = new Account(privateKey);

                        //将私钥存储在db中
                        using (Snapshot snapshot = Singleton.Store.GetSnapshot())
                        {
                            UserKey uk = new UserKey() { Password = password, Username = userName };
                            User u = new User() { Password = password, Username = userName, PrivateKey = account.PrivateKey, Address = new UInt160(account.Address) };
                            snapshot.Users.Add(uk,u);
                            snapshot.Commit();
                            result = getJAbyJ(u.ToJson());
                        }
                        break;
                    }

            }
            return result;
        }
    }
}
