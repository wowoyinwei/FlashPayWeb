using System;
using System.Collections.Generic;
using System.Text;

namespace FlashPayWeb.libs
{
    public static class Conversion
    {
        public static string FormatHexStr(this string hexStr)
        {
            string result = hexStr.ToLower();

            if (result.IndexOf("0x", System.StringComparison.CurrentCulture) == -1)
                result = "0x" + result;
            return result;
        }

        public static string Bytes2HexString(this byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var d in data)
            {
                sb.Append(d.ToString("x02"));
            }
            return sb.ToString();
        }

        public static byte[] HexString2Bytes(this string str)
        {
            if (str.IndexOf("0x") == 0)
                str = str.Substring(2);
            byte[] outd = new byte[str.Length / 2];
            for (var i = 0; i < str.Length / 2; i++)
            {
                outd[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return outd;
        }

        public static byte[] ToBytes(this byte b)
        {
            return new byte[] { b };
        }

        public static uint HexToUint(this string str)
        {
            return uint.Parse(str, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
