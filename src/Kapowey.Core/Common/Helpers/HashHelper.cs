﻿using System.Security.Cryptography;
using System.Text;

namespace Kapowey.Core.Common.Helpers
{
    public static class HashHelper
    {
        public static string CreateMD5(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            return CreateMD5(System.Text.Encoding.UTF8.GetBytes(input));
        }

        public static string CreateMD5(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }
            using (var md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(bytes);

                // Create a new Stringbuilder to collect the bytes and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }
    }
}
