using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Extensions
{
    public static class StringExtenstions
    {
        /// <summary>
        /// 取得分割字元前的字串
        /// </summary>
        /// <param name="org">原始字串</param>
        /// <param name="spliter">分割字元</param>
        /// <returns>分割字元前的字串。如果找不到分割字元時會傳回空字串</returns>
        public static string SubstringByChar(this string org, char spliter)
        {
            int dotPos = org.IndexOf(spliter);
            if (dotPos > 0)
            {
                return org.Substring(0, dotPos);
            }
            else
                return "";
        }

        /// <summary>
        /// 取得分割字元後的字串
        /// </summary>
        /// <param name="org">原始字串</param>
        /// <param name="spliter">分割字元</param>
        /// <returns>分割字元後的字串。如果找不到分割字元時會傳回空字串</returns>
        public static string SubstringByCharFromTail(this string org, char spliter)
        {
            int dotPos = org.IndexOf(spliter);
            if (dotPos > 0)
            {
                return org.Substring(dotPos + 1, org.Length - dotPos - 1);
            }
            else
                return "";
        }

    }
}
