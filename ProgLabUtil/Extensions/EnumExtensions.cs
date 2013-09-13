using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace ProgLab.Util.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 取得某列舉型別的Description字串
        /// </summary>
        /// <typeparam name="TEnum">列舉的型別(可不用指定)</typeparam>
        /// <param name="eValue">列舉值</param>
        /// <returns>列舉值對應的Description字串</returns>
        public static string GetEnumDescription<TEnum>(this TEnum eValue)
        {
            FieldInfo fi = eValue.GetType().GetField(eValue.ToString());
            if (fi != null)
            {
                object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
                else
                    return "";
            }
            else
                return "";
        }
    }
}
