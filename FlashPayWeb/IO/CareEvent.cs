using FlashPayWeb.libs;
using Newtonsoft.Json.Linq;
using System.IO;

namespace FlashPayWeb.IO
{
    public class CareEvent : ISerializable
    {
        public UInt256 Hash;
        public string EventStr;
        public string HexStr;

        public void Deserialize(BinaryReader reader)
        {
            Hash = reader.ReadSerializable<UInt256>();
            EventStr = reader.ReadVarString();
            HexStr = reader.ReadVarString();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Hash);
            writer.Write(EventStr);
            writer.Write(HexStr);
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
            jo["Hash"] = Hash.ToString();
            jo["EventStr"] = EventStr;
            jo["HexStr"] = HexStr;
            return jo;
        }
    }
}
