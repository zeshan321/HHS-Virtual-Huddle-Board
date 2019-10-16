using System;

namespace HHSBoard.Extensions
{
    public static class StringExtensions
    {
        public static string EncodeBase64(this string plainText)
        {
            if (String.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string DecodeBase64(this string base64EncodedData)
        {
            if (String.IsNullOrEmpty(base64EncodedData))
            {
                return base64EncodedData;
            }

            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
