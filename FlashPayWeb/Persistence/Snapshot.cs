using System;
using FlashPayWeb.IO;
using FlashPayWeb.libs;

namespace FlashPayWeb.Persistence
{
    public abstract class Snapshot : IDisposable
    {
        public abstract Cache<UserKey, User> Users { get; }

        public virtual void Commit()
        {
            Users?.Commit();
        }

        public virtual void Dispose()
        {
        }
    }
}
