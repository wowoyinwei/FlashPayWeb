using System;
using FlashPayWeb.IO;
using NEL.SimpleDB;
using FlashPayWeb.libs;

namespace FlashPayWeb.Persistence.SimpleDB
{
    public class SimpleDbStore : Store, IDisposable
    {
        public DB db;

        public SimpleDbStore(string path)
        {
            db = new DB();
            string fullpath = System.IO.Path.GetFullPath(path);
            if (System.IO.Directory.Exists(fullpath) == false)
                System.IO.Directory.CreateDirectory(fullpath);
            string pathDB = System.IO.Path.Combine(fullpath, "web");
            try
            {
                db.Open(pathDB,true);
                Console.WriteLine("db opened in:" + pathDB);
            }
            catch (Exception err)
            {
                Console.WriteLine("error msg:" + err.Message);
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public override Cache<UInt256, User> GetUsers()
        {
            return new SimpleDbCache<UInt256, User>(db,null,TableId.DATA_User);
        }

        public override Snapshot GetSnapshot()
        {
            return new SimpleDbSnapShot(db);
        }

        public override byte[] Get(byte[] tableId, byte[] key)
        {
            return db.GetDirect(tableId,key);
        }

    }
}
