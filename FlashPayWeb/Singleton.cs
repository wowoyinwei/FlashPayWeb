using FlashPayWeb.libs;
using FlashPayWeb.Persistence;
using FlashPayWeb.Persistence.SimpleDB;

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

        /*
        private static Cache<UInt160, UInt160> careAddrs;
        public static Cache<UInt160, UInt160> CareAddrs
        {
            get
            {
                if (careAddrs == null)
                {
                    careAddrs = Store.GetCareAddrs();
                    Logger.LogCommon("开始缓存需要在意的地址：");
                    foreach (var a in careAddrs.Find())
                    {
                        Logger.LogCommon(a.Key.ToString());
                    }
                }
                return careAddrs;
            }
        }

        private static Cache<UInt160, UInt160> careAssets;
        public static Cache<UInt160, UInt160> CareAssets
        {
            get
            {
                if (careAssets == null)
                {
                    careAssets = Store.GetCareAssets();
                    Logger.LogCommon("开始缓存需要在意的资产：");
                    foreach (var a in careAssets.Find())
                    {
                        Logger.LogCommon(a.Key.ToString());
                    }
                }
                return careAssets;
            }
        }

        private static Cache<UInt256, CareEvent> careEvents;
        public static Cache<UInt256, CareEvent> CareEvents
        {
            get
            {
                if (careEvents == null)
                {
                    careEvents = Store.GetCareEvents();
                    Logger.LogCommon("开始缓存需要在意的资产通知：");
                    foreach (var a in careEvents.Find())
                    {
                        Logger.LogCommon(a.Value.ToJson());
                    }
                }
                return careEvents;
            }
        }
        */
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
    }
}
