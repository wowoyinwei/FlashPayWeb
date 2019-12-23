using System;
using FlashPayWeb.IO;
using FlashPayWeb.libs;

namespace FlashPayWeb.Persistence
{
    public abstract class Snapshot : IDisposable
    {
        public abstract Cache<UInt256, User> Users { get; }

        public virtual void Commit()
        {
            Users?.Commit();
        }

        public virtual void Dispose()
        {
        }
    }
}
