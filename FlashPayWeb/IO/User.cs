using System.IO;
using Newtonsoft.Json.Linq;
using FlashPayWeb.libs;
using System;

namespace FlashPayWeb.IO
{
    public class User : ISerializable
    {
        public string Username;
        public string Password;
        public string PrivateKey;
        public UInt160 Address;

        public void Deserialize(BinaryReader reader)
        {
            Address = reader.ReadSerializable<UInt160>();
            Username = reader.ReadVarString();
            Password = reader.ReadVarString();
            PrivateKey = reader.ReadVarString();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Address);
            writer.Write(Username);
            writer.Write(Password);
            writer.Write(PrivateKey);
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
            jo["username"] = Username;
            jo["password"] = Password;
            //jo["privatekey"] = PrivateKey;
            jo["address"] = Address.ToString();
            return jo;
        }

        public JObject ToJson_OnlyAddr()
        {
            JObject jo = new JObject();
            jo["address"] = Address.ToString();
            return jo;
        }
    }
}
