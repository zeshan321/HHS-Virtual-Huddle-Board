using HHSBoard.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Web;

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

        public static void EncodeUserHtml(this BaseCreateModel baseCreateModel)
        {
            foreach (var prop in baseCreateModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.PropertyType == typeof(string)))
            {
                var value = (string)prop.GetValue(baseCreateModel, null);
                if (!string.IsNullOrEmpty(value))
                {
                    prop.SetValue(baseCreateModel, HttpUtility.HtmlEncode(value));
                }
            }
        }
    }
}
