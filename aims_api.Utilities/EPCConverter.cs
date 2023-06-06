using aims_api.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Utilities
{
    public class EPCConverter
    {
        public async Task<EPCConversionType> ReverseConversionType(string type)
        {
            return await Task.Run(() =>
            {
                if (type == (EPCConversionType.ToAscii).ToString())
                {
                    // reverse type
                    return EPCConversionType.ToHex;
                }

                if (type == (EPCConversionType.ToHex).ToString())
                {
                    return EPCConversionType.ToAscii;
                }

                return EPCConversionType.None;
            });
        }

        public async Task<string> ConvertString(string method, string origString)
        {
            return await Task.Run(() =>
            {
                string val = string.Empty;

                if (method == "ToAscii")
                {
                    for (int i = 0; i < origString.Length; i += 2)
                    {
                        string hs = string.Empty;

                        hs = origString.Substring(i, 2); // Hex string value sample  2 = two digit of number.
                        ulong decval = Convert.ToUInt64(hs, 16);  // Decration Value 
                        long deccc = Convert.ToInt64(hs, 16); // Decration character
                        char character = Convert.ToChar(deccc); // convert int to char or value.
                        val += character;
                    }
                    return val;
                }
                else if (method == "ToHex")
                {
                    StringBuilder sb = new StringBuilder();

                    byte[] inputByte = Encoding.UTF8.GetBytes(origString);

                    foreach (byte HexType in inputByte)
                    {
                        sb.Append(string.Format("{0:x2}", HexType));
                    }
                    return sb.ToString();
                }
                else
                {
                    return origString;
                }
            });
        }
    }
}
