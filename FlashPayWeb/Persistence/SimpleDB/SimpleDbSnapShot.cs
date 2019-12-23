using NEL.SimpleDB;
using FlashPayWeb.IO;
using FlashPayWeb.libs;

namespace FlashPayWeb.Persistence.SimpleDB
{
    public class SimpleDbSnapShot : Snapshot
    {
        private readonly DB db;
        private readonly ISnapShot snapshot;
        private readonly IWriteBatch batch;


        public override Cache<UInt256, User> Users{ get; }

        public SimpleDbSnapShot(DB db)
        {
            this.db = db;
            this.snapshot = db.UseSnapShot();
            this.batch = db.CreateWriteBatch();
            Users = new SimpleDbCache<UInt256, User>(this.db,this.batch,TableId.DATA_User);
        }

        public override void Commit()
        {
            base.Commit();
            db.WriteBatch(batch);
        }

        public override void Dispose()
        {
            snapshot.Dispose();
        }
    }
}
