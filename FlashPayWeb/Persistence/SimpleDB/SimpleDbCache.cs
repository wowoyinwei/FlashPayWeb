using FlashPayWeb.IO;
using System;
using System.IO;
using NEL.SimpleDB;
using System.Collections.Generic;
using System.Linq;

namespace FlashPayWeb.Persistence.SimpleDB
{
    public class SimpleDbCache<TKey, TValue> : Cache<TKey, TValue>
        where TKey : ISerializable, new()
        where TValue : class, ISerializable, new()
    {
        private readonly DB db;
        private readonly IWriteBatch batch;
        private readonly byte tableId;

        public SimpleDbCache(DB _db,IWriteBatch _batch,byte _tableId)
        {
            this.db = _db;
            this.batch = _batch;
            this.tableId = _tableId;
        }

        public override void DeleteInternal(TKey key)
        {
            batch?.Delete(new byte[] { tableId }, key.Serialize());
        }

        protected override void AddInternal(TKey key, TValue value)
        {
            batch?.Put(new byte[] { tableId }, key.Serialize(), value.Serialize());
        }

        protected override void UpdateInternal(TKey key, TValue value)
        {
            batch?.Put(new byte[] { tableId }, key.Serialize(), value.Serialize());
        }

        public override IEnumerable<KeyValuePair<TKey, TValue>> FindInternal(byte[] beginKey)
        {
            return db.Find(new byte[] { tableId},beginKey, (k, v) => new KeyValuePair<TKey, TValue>(k.ToArray().AsSerializable<TKey>(), v.ToArray().AsSerializable<TValue>()));
        }

        public override TValue GetInternal(TKey key)
        {
            return db.GetDirect(new byte[] { tableId },key.Serialize())?.AsSerializable<TValue>();
        }
    }
}
