using System.IO;
using Newtonsoft.Json.Linq;
using FlashPayWeb.libs;

namespace FlashPayWeb.IO
{
    public class UserKey : ISerializable
    {
        public string Username;
        public string Password;

        public void Deserialize(BinaryReader reader)
        {
            Username = reader.ReadVarString();
            Password = reader.ReadVarString();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Username);
            writer.Write(Password);
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
            jo["Username"] = Username;
            jo["Password"] = Password;
            return jo;
        }
    }

    public class User : ISerializable
    {
        public string Username;
        public string Password;
        public string PrivateKey;
        public UInt160 Address;

        public void Deserialize(BinaryReader reader)
        {
            Username = reader.ReadVarString();
            Password = reader.ReadVarString();
            PrivateKey = reader.ReadVarString();
            Address = reader.ReadSerializable<UInt160>();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Username);
            writer.Write(Password);
            writer.Write(PrivateKey);
            writer.Write(Address);
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
            jo["privatekey"] = PrivateKey;
            jo["address"] = Address.ToString();
            return jo;
        }
    }
}
