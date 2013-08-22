using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Extensions
{
    /// <summary>
    /// Dictionary擴充物件
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 將Dictionary轉成Key=Value的字串格式
        /// </summary>
        /// <typeparam name="TKey">資料鍵值</typeparam>
        /// <typeparam name="TValue">資料</typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string ToKeyValueString<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<TKey, TValue> pair in dic)
            {
                sb.Append(string.Format("{0}={1},", pair.Key.ToString(), pair.Value.ToString()));
            }

            return sb.ToString();
        }

        #region 擴充以List<>存放資料的Dictionary
        /// <summary>
        /// 新增指定的key及value
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="container">以List{TValue}存放資料的Dictionary</param>
        /// <param name="key">要新增的key</param>
        /// <param name="value">要新增的value</param>
        public static void Add<TKey, TValue>(this Dictionary<TKey, List<TValue>> container, TKey key, TValue value)
        {
            lock (container)
            {
                List<TValue> orgList = null;
                if (container.TryGetValue(key, out orgList))
                {
                    orgList.Add(value);
                }
                else
                {
                    orgList = new List<TValue>();
                    orgList.Add(value);
                    container.Add(key, orgList);
                }
            }
        }

        /// <summary>
        /// 移除指定的Key中的某個value
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="container">以List{TValue}存放資料的Dictionary</param>
        /// <param name="key">要移除的key</param>
        /// <param name="value">要移除的value</param>
        public static void Remove<TKey, TValue>(this Dictionary<TKey, List<TValue>> container, TKey key, TValue value)
        {
            lock (container)
            {
                List<TValue> orgList = null;
                if (container.TryGetValue(key, out orgList))
                {
                    orgList.Remove(value);
                    if (orgList.Count <= 0)
                        container.Remove(key);
                }
            }
        }
        #endregion
    }
}
