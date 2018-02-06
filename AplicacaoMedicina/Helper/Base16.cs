using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplicacaoMedicina.Helper
{
    public class Base16
    {
        private static readonly char[] encoding;

        static Base16()
        {
            encoding = new char[16]
            {
            '0', '1', '2', '3', '4', '5', '6', '7',
            '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
            };
        }

        public static string Encode(byte[] data)
        {
            char[] text = new char[data.Length * 2];

            for (int i = 0, j = 0; i < data.Length; i++)
            {
                text[j++] = encoding[data[i] >> 4];
                text[j++] = encoding[data[i] & 0xf];
            }

            return new string(text);
        }
    }
}