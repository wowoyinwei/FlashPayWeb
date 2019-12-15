using FlashPayWeb.libs;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlashPayWeb.IO
{
    public class LogKey : ISerializable
    {
        public UInt256 TransactionHash;
        public uint LogIndex;

        public void Deserialize(BinaryReader reader)
        {
            TransactionHash = reader.ReadSerializable<UInt256>();
            LogIndex = reader.ReadUInt32();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(TransactionHash);
            writer.Write(LogIndex);
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

    public class Log : ISerializable
    {
        public UInt160 ContractAddress;
        public UInt256 BlockHash;
        public uint BlockNumber;
        public string[] Data;
        public string[] Topics;
        public uint LogIndex;
        public bool Removed;
        public UInt256 TransactionHash;
        public uint TransactionIndex;
        public string Event;

        public void Deserialize(BinaryReader reader)
        {
            ContractAddress = reader.ReadSerializable<UInt160>();
            BlockHash = reader.ReadSerializable<UInt256>();
            BlockNumber = reader.ReadUInt32();
            Data = reader.ReadStringArray();
            Topics = reader.ReadStringArray();
            LogIndex = reader.ReadUInt32();
            Removed = reader.ReadBoolean();
            TransactionHash = reader.ReadSerializable<UInt256>();
            TransactionIndex = reader.ReadUInt32();
            Event = reader.ReadVarString();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ContractAddress);
            writer.Write(BlockHash);
            writer.Write(BlockNumber);
            writer.Write(Data);
            writer.Write(Topics);
            writer.Write(LogIndex);
            writer.Write(Removed);
            writer.Write(TransactionHash);
            writer.Write(TransactionIndex);
            writer.Write(Event);
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
            jo["ContractAddress"] = ContractAddress.ToString();
            jo["BlockHash"] = BlockHash.ToString();
            jo["BlockNumber"] = BlockNumber;
            jo["data"] = new JArray(Data);
            jo["Topics"] = new JArray(Topics);
            jo["LogIndex"] = LogIndex;
            jo["Removed"] = Removed;
            jo["TransactionHash"] = TransactionHash.ToString();
            jo["TransactionIndex"] = TransactionIndex;
            jo["Event"] = Event;
            return jo;
        }

        public static Log FromJson(JObject jo)
        {
            Log l = new Log();
            l.ContractAddress = new UInt160((string)jo["address"]);
            l.BlockHash = new UInt256((string)jo["blockHash"]);
            l.BlockNumber =((string)jo["blockNumber"]).Replace("0x","").HexToUint();
            string data = ((string)jo["data"]).Replace("0x", "");
            List<string> list = new List<string>();
            for (int i = 0; i < data.Trim().Length; i += 64)
            {
                if ((data.Trim().Length - i) >= 64)
                    list.Add(data.Trim().Substring(i, 64));
                else
                    list.Add(data.Trim().Substring(i, data.Trim().Length - i));
            }
            l.Data = list.ToArray();
            l.Topics = ((JArray)jo["topics"]).Count == 0 ? new string[0] :((JArray)jo["topics"]).Select(p => (string)p).ToArray();
            l.LogIndex = ((string)jo["logIndex"]).Replace("0x", "").HexToUint();
            l.Removed = (bool)jo["removed"];
            l.TransactionHash = new UInt256((string)jo["transactionHash"]);
            l.TransactionIndex = ((string)jo["transactionIndex"]).Replace("0x", "").HexToUint();
            return l;   
        }
    }
}
