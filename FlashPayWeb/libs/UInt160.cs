using FlashPayWeb.IO;
using System;
using System.IO;
using System.Linq;

namespace FlashPayWeb.libs
{
    public class UInt160 : IComparable<UInt160>, IEquatable<UInt160>, ISerializable
    {
        public const int Length = 20;
        public static readonly UInt160 Zero = new UInt160();

        public UInt160()
        {
            data = new byte[20];
        }

        public UInt160(byte[] data)
        {
            if (data.Length != 20)
                throw new Exception("error length.");
            this.data = data;

        }

        public UInt160(string hexstr)
        {
            var bts = Conversion.HexString2Bytes(hexstr);
            if (bts.Length != 20)
                throw new Exception("error length.");
            this.data = bts.Reverse().ToArray();
        }
        public override string ToString()
        {
            return "0x" + Conversion.Bytes2HexString(this.data.Reverse().ToArray());
        }
        public byte[] data;

        public int Size => data.Length;

        public int CompareTo(UInt160 other)
        {
            byte[] x = data;
            byte[] y = other.data;
            for (int i = x.Length - 1; i >= 0; i--)
            {
                if (x[i] > y[i])
                    return 1;
                if (x[i] < y[i])
                    return -1;
            }
            return 0;
        }
        public override bool Equals(object obj)
        {
            return CompareTo(obj as UInt160) == 0;
        }
        public override int GetHashCode()
        {
            return new System.Numerics.BigInteger(data).GetHashCode();
        }

        public unsafe bool Equals(UInt256 other)
        {
            if (other is null) return false;
            fixed (byte* px = data, py = other.data)
            {
                ulong* lpx = (ulong*)px;
                ulong* lpy = (ulong*)py;
                //256bit / 64bit(ulong step) -1
                for (int i = (256 / 64 - 1); i >= 0; i--)
                {
                    if (lpx[i] != lpy[i])
                        return false;
                }
            }
            return true;
        }

        public static new UInt160 Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (value.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                value = value.Substring(2);
            if (value.Length != Length * 2)
                throw new FormatException();
            return new UInt160(value.HexString2Bytes().Reverse().ToArray());
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(data);
        }

        public void Deserialize(BinaryReader reader)
        {
            if (reader.Read(data, 0, data.Length) != data.Length)
            {
                throw new FormatException();
            }
        }

        /// <summary>
        /// Method Equals returns true if objects are equal, false otherwise
        /// </summary>
        public unsafe bool Equals(UInt160 other)
        {
            if (other is null) return false;
            fixed (byte* px = data, py = other.ToArray())
            {
                uint* lpx = (uint*)px;
                uint* lpy = (uint*)py;
                //160 bit / 32 bit(uint step)   -1
                for (int i = (160 / 32 - 1); i >= 0; i--)
                {
                    if (lpx[i] != lpy[i])
                        return false;
                }
            }
            return true;
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

        public static implicit operator byte[] (UInt160 value)
        {
            return value.data;
        }
        public static implicit operator UInt160(byte[] value)
        {
            return new UInt160(value);
        }
    }
}
