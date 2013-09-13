using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using ProgLab.Util.Extensions;  

namespace ProgLab.GUI.Extensions
{
    public static class ComboBoxExtenstions
    {
        /// <summary>
        /// 將列舉值填入到指定的ComboBox裏作為選項。當使用都選擇後可以將ComboBox.SelectedValue直接轉型回原本的列舉型別
        /// </summary>
        /// <typeparam name="TEnum">列舉型別</typeparam>
        /// <param name="cmb">要填入資料的ComboBox</param>
        /// <remarks>
        /// http://stackoverflow.com/questions/1102022/display-enum-in-combobox-with-spaces
        /// </remarks>
        public static void FillEnum<TEnum>(this ComboBox cmb)
        {
            cmb.DataSource = Enum.GetValues(typeof(TEnum))
                           .Cast<TEnum>()
                           .Select(e => new { Value = e, Description = e.ToString() })
                           .ToList();
            cmb.DisplayMember = "Description";
            cmb.ValueMember = "Value";
        }

        /// <summary>
        /// 將列舉值對應的Description屬性中的字串填入到指定的ComboBox裏作為選項。當使用都選擇後可以將ComboBox.SelectedValue直接轉型回原本的列舉型別
        /// </summary>
        /// <typeparam name="TEnum">列舉型別</typeparam>
        /// <param name="cmb">要填入資料的ComboBox</param>
        /// <remarks>
        /// http://stackoverflow.com/questions/1102022/display-enum-in-combobox-with-spaces
        /// </remarks>
        public static void FillEnumByDescription<TEnum>(this ComboBox cmb)
        {
            cmb.DataSource = Enum.GetValues(typeof(TEnum))
                           .Cast<TEnum>()
                //.Select(e => new { Value = e, Description = GetEnumDescription(e) })
                           .Select(e => new { Value = e, Description = e.GetEnumDescription() })
                           .ToList();
            cmb.DisplayMember = "Description";
            cmb.ValueMember = "Value";
        }

        /// <summary>
        /// 取得某列舉型別的Description字串
        /// </summary>
        /// <typeparam name="TEnum">列舉的型別(可不用指定)</typeparam>
        /// <param name="eValue">列舉值</param>
        /// <returns>列舉值對應的Description字串</returns>
        public static string GetEnumDescription<TEnum>(TEnum eValue)
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
