using FlashPayWeb.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using NEL.SimpleDB;

namespace FlashPayWeb.Persistence
{
    public static class Helper
    {
        public static IEnumerable<T> Find<T>(this DB db,byte[] tableId, byte[] beginKey) where T : class, ISerializable, new()
        {
            return Find(db, tableId, beginKey, (k, v) => v.ToArray().AsSerializable<T>());
        }

        public static IEnumerable<T> Find<T>(this DB db,byte[] tableId, byte[] beginKey, Func<byte[], byte[], T> resultSelector)
        {
            var snapShot = db.UseSnapShot();
            var it = snapShot.CreateKeyIterator(tableId,beginKey);
            {
                it.SeekToFirst();
                while (it.MoveNext())
                {
                    byte[] key = it.Current;
                    byte[] y = tableId.Concat(beginKey).ToArray();
                    if (key.Length < y.Length) break;
                    byte[] value = db.GetDirect(tableId, key);
                    yield return resultSelector(key, value);
                }
            }
        }
    }
}
