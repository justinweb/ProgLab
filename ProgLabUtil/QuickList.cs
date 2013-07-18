using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util
{
    /// <summary>
    /// 整合Dictionary及List兩種儲存體。對外提供List，對內則用Dictionary的方式來管理以便快速更新
    /// </summary>
    /// <typeparam name="TKey">資料主索引鍵</typeparam>
    /// <typeparam name="TData">資料</typeparam>
    public class QuickList<TKey, TData>
    {
        private object lock_Process = new object();
        private Dictionary<TKey, TData> dicData = new Dictionary<TKey, TData>();
        private List<TData> listData = new List<TData>();
        /// <summary>
        /// 取得資料串列
        /// </summary>
        public List<TData> DataList
        {
            get { return listData; }
        }

        /// <summary>
        /// 加入資料
        /// </summary>
        /// <param name="key">資料主索引鍵</param>
        /// <param name="data">資料</param>
        /// <param name="updateData">更新資料，Param1=org, Param2=new</param>
        /// <rturn>是否已存在</rturn>
        /// <remarks>
        /// 加入資料時，會先用所指定的<paramref name="key"/>查詢Dictionary，如果資料不存在，則會加入。如果資料已存在，則會呼叫所指定的更新函式<paramref name="updateData"/>來進行更新。
        /// </remarks>
        public bool Add(TKey key, TData data, Action<TData, TData> updateData)
        {
            lock (lock_Process)
            {
                TData orgData = default(TData);
                if (dicData.TryGetValue(key, out orgData))
                {
                    // Update
                    updateData(orgData, data);
                    return true;
                }
                else
                {
                    dicData.Add(key, data);
                    listData.Add(data);
                    return false;
                }
            }
        }

        /// <summary>
        /// 清除所有資料
        /// </summary>
        public void Clear()
        {
            lock (lock_Process)
            {
                listData.Clear();
                dicData.Clear();
            }
        }
    }
}
