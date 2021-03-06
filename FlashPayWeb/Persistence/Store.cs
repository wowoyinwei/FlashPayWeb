﻿using FlashPayWeb.IO;
using FlashPayWeb.libs;
    
namespace FlashPayWeb.Persistence
{
    public abstract class Store
    {
        public abstract Cache<UInt256, User> GetUsers();

        public abstract Snapshot GetSnapshot();
        public abstract byte[] Get(byte[] tableId,byte[] key);
    }
}
