using FlashPayWeb.libs;
using FlashPayWeb.Persistence;
using FlashPayWeb.Persistence.SimpleDB;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace FlashPayWeb
{
    public class Singleton
    {
        private static Store store;
        public static Store Store
        {
            get
            {
                if (store == null)
                    store = new SimpleDbStore(Setting.Ins.SimpleDbPath);
                return store;
            }
        }

        private static EthHelper ethHelper;
        public static EthHelper EthHelper
        {
            get
            {
                if (ethHelper == null)
                    ethHelper = new EthHelper(Setting.Ins.EthCliUrl);
                return ethHelper;
            }
        }

        private static CrawlerHelper crawlerHelper;
        public static CrawlerHelper CrawlerHelper
        {
            get
            {
                if (crawlerHelper == null)
                    crawlerHelper = new CrawlerHelper(Setting.Ins.CrawlerUrl);
                return crawlerHelper;
            }
        }

        private static Web3 web3Ins;
        public static Web3 Web3Ins
        {
            get
            {
                if (web3Ins == null)
                    web3Ins = new Web3(new Account(Setting.Ins.OwnerPriKey), Setting.Ins.EthCliUrl);
                return web3Ins;
            }
        }
    }
}
