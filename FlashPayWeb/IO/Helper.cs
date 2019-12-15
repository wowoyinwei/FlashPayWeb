using System;
using System.IO;
using System.Text;

namespace FlashPayWeb.IO
{
    public static class Helper
    {
        public const int GroupingSizeInBytes = 16;

        public static T AsSerializable<T>(this byte[] value, int start = 0) where T : ISerializable, new()
        {
            using (MemoryStream ms = new MemoryStream(value, start, value.Length - start, false))
            using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
            {
                return reader.ReadSerializable<T>();
            }
        }

        public static T ReadSerializable<T>(this BinaryReader reader) where T : ISerializable, new()
        {
            T obj = new T();
            obj.Deserialize(reader);
            return obj;
        }

        public static T[] ReadSerializableArray<T>(this BinaryReader reader, int max = 0x1000000) where T : ISerializable, new()
        {
            T[] array = new T[reader.ReadVarInt((ulong)max)];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new T();
                array[i].Deserialize(reader);
            }
            return array;
        }

        public static byte[] ReadVarBytes(this BinaryReader reader, int max = 0x1000000)
        {
            return reader.ReadBytes((int)reader.ReadVarInt((ulong)max));
        }

        public static ulong ReadVarInt(this BinaryReader reader, ulong max = ulong.MaxValue)
        {
            byte fb = reader.ReadByte();
            ulong value;
            if (fb == 0xFD)
                value = reader.ReadUInt16();
            else if (fb == 0xFE)
                value = reader.ReadUInt32();
            else if (fb == 0xFF)
                value = reader.ReadUInt64();
            else
                value = fb;
            if (value > max) throw new FormatException();
            return value;
        }

        public static string[] ReadStringArray(this BinaryReader reader,int max = 0x1000000)
        {
            ulong count = reader.ReadVarInt();
            string[] sr = new string[count];
            for (int i = 0; i < sr.Length; i++)
            {
                sr[i] = reader.ReadVarString();
            }
            return sr;
        }

        public static string ReadVarString(this BinaryReader reader, int max = 0x1000000)
        {
            return Encoding.UTF8.GetString(reader.ReadVarBytes(max));
        }

        public static byte[] ToArray(this ISerializable value)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8))
            {
                value.Serialize(writer);
                writer.Flush();
                return ms.ToArray();
            }
        }

        public static byte[] ToByteArray<T>(this T[] value) where T : ISerializable
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8))
            {
                writer.Write(value);
                writer.Flush();
                return ms.ToArray();
            }
        }

        public static void Write(this BinaryWriter writer, ISerializable value)
        {
            value.Serialize(writer);
        }

        public static void Write<T>(this BinaryWriter writer, T[] value) where T : ISerializable
        {
            writer.WriteVarInt(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                value[i].Serialize(writer);
            }
        }

        public static void Write(this BinaryWriter writer, string[] value)
        {
            writer.WriteVarInt(value.Length);
            for(int i = 0;i<value.Length;i++)
            {
                writer.WriteVarString(value[i]);
            }
        }

        public static void WriteBytesWithGrouping(this BinaryWriter writer, byte[] value)
        {
            int index = 0;
            int remain = value.Length;
            while (remain >= GroupingSizeInBytes)
            {
                writer.Write(value, index, GroupingSizeInBytes);
                writer.Write((byte)GroupingSizeInBytes);
                index += GroupingSizeInBytes;
                remain -= GroupingSizeInBytes;
            }
            if (remain > 0)
                writer.Write(value, index, remain);
            int padding = GroupingSizeInBytes - remain;
            for (int i = 0; i < padding; i++)
                writer.Write((byte)0);
            writer.Write((byte)remain);
        }

        public static void WriteFixedString(this BinaryWriter writer, string value, int length)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value.Length > length)
                throw new ArgumentException();
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            if (bytes.Length > length)
                throw new ArgumentException();
            writer.Write(bytes);
            if (bytes.Length < length)
                writer.Write(new byte[length - bytes.Length]);
        }

        public static void WriteVarBytes(this BinaryWriter writer, byte[] value)
        {
            writer.WriteVarInt(value.Length);
            writer.Write(value);
        }

        public static void WriteVarInt(this BinaryWriter writer, long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException();
            if (value < 0xFD)
            {
                writer.Write((byte)value);
            }
            else if (value <= 0xFFFF)
            {
                writer.Write((byte)0xFD);
                writer.Write((ushort)value);
            }
            else if (value <= 0xFFFFFFFF)
            {
                writer.Write((byte)0xFE);
                writer.Write((uint)value);
            }
            else
            {
                writer.Write((byte)0xFF);
                writer.Write(value);
            }
        }

        public static void WriteVarString(this BinaryWriter writer, string value)
        {
            writer.WriteVarBytes(Encoding.UTF8.GetBytes(value));
        }
    }
}
