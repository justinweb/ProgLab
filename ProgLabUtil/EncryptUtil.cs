using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ProgLab.Util
{
    public class EncryptUtil
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="_strQ"></param>
        /// <param name="strKey">要8個字元</param> 
        /// <param name="strIV">要>=8個字元</param>
        /// <returns></returns>
        public static string Encrypt(string _strQ, string strKey, string strIV)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(_strQ);
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(ms, tdes.CreateEncryptor(Encoding.UTF8.GetBytes(strKey), Encoding.UTF8.GetBytes(strIV)), CryptoStreamMode.Write);
            encStream.Write(buffer, 0, buffer.Length);
            encStream.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray()).Replace("+", "%");
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="_strQ"></param>
        /// <param name="strKey">要8個字元</param> 
        /// <param name="strIV">要>=8個字元</param>
        /// <returns></returns>
        public static string Decrypt(string _strQ, string strKey, string strIV)
        {
            _strQ = _strQ.Replace("%", "+");
            byte[] buffer = Convert.FromBase64String(_strQ);
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(ms, tdes.CreateDecryptor(Encoding.UTF8.GetBytes(strKey), Encoding.UTF8.GetBytes(strIV)), CryptoStreamMode.Write);
            encStream.Write(buffer, 0, buffer.Length);
            encStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static void UT()
        {
            string src = "Justin1021";
            string encStr = Encrypt(src, "12345678", "Test45.");
            string decStr = Decrypt(encStr, "12345678", "Test45.");
        }
    }
}
