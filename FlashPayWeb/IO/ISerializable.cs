using Newtonsoft.Json.Linq;
using System.IO;

namespace FlashPayWeb.IO
{
    public interface ISerializable
    {
        void Serialize(BinaryWriter writer);
        byte[] Serialize();
        void Deserialize(BinaryReader reader);
    }
}
