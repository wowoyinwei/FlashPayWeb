using FlashPayWeb.IO;
using System;
using System.IO;
using System.Linq;

namespace FlashPayWeb.libs
{
    public class UInt256 : IComparable<UInt256>, IEquatable<UInt256>, ISerializable
    {
        public const int Length = 32;
        public static readonly UInt256 Zero = new UInt256();


        /// <summary>
        /// The empty constructor stores a null byte array
        /// </summary>
        public UInt256()
        {
            data = new byte[32];
        }

        public UInt256(byte[] data)
        {
            if (data.Length != 32)
                throw new Exception("error length.");
            this.data = data;
        }
        public UInt256(string hexstr)
        {
            var bts = Conversion.HexString2Bytes(hexstr);
            if (bts.Length != 32)
                throw new Exception("error length.");
            this.data = bts.Reverse().ToArray();
        }
        public override string ToString()
        {
            return "0x" + this.data.Reverse().ToArray().Bytes2HexString();
        }
        public byte[] data;

        public int CompareTo(UInt256 other)
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
            return CompareTo(obj as UInt256) == 0;
        }
        public override int GetHashCode()
        {
            return new System.Numerics.BigInteger(data).GetHashCode();
        }

        /// <summary>
        /// Method Equals returns true if objects are equal, false otherwise
        /// </summary>
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

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                this.Serialize(writer);
                return ms.ToArray();
            }
        }

        public static implicit operator byte[] (UInt256 value)
        {
            return value.data;
        }
        public static implicit operator UInt256(byte[] value)
        {
            return new UInt256(value);
        }
    }
}
