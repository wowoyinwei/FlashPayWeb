using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlashPayWeb.libs;
using Newtonsoft.Json.Linq;

namespace FlashPayWeb.IO
{

    public class Transfer : ISerializable
    {
        public UInt256 TransactionHash;

        public uint LogIndex;

        public UInt160 Asset;

        public UInt160 From;

        public UInt160 To;

        public string Value;

        public void Deserialize(BinaryReader reader)
        {
            TransactionHash = reader.ReadSerializable<UInt256>();
            LogIndex = reader.ReadUInt32();
            Asset = reader.ReadSerializable<UInt160>();
            From = reader.ReadSerializable<UInt160>();
            To = reader.ReadSerializable<UInt160>();
            Value = reader.ReadVarString();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(TransactionHash);
            writer.Write(LogIndex);
            writer.Write(Asset);
            writer.Write(From);
            writer.Write(To);
            writer.Write(Value);
        }

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                this.Serialize(writer);
                return ms.ToArray();
            }
        }

        public JObject ToJson()
        {
            JObject jo = new JObject();
            jo["TransactionHash"] = TransactionHash.ToString();
            jo["LogIndex"] = LogIndex;
            jo["Asset"] = Asset.ToString();
            jo["From"] = From.ToString();
            jo["To"] = To.ToString();
            jo["Value"] = Value;
            return jo;
        }
    }

    public class TransferKey : ISerializable
    {
        public UInt160 address;
        public uint blockNumber; 

        public void Deserialize(BinaryReader reader)
        {
            address = reader.ReadSerializable<UInt160>();
            blockNumber = reader.ReadUInt32();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(address);
            writer.Write(blockNumber);
        }

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                this.Serialize(writer);
                return ms.ToArray();
            }
        }
    }

    public class TransferGroup : ISerializable
    {
        public Transfer[] transfers;

        public void Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            transfers = new Transfer[count];
            for (var i = 0; i < count; i++)
            {
                transfers[i] = reader.ReadSerializable<Transfer>();
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(transfers.Length);
            for (uint i = 0; i < transfers.Length; i++)
            {
                writer.Write(transfers[i]);
            }
        }

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                this.Serialize(writer);
                return ms.ToArray();
            }
        }
    }

}
