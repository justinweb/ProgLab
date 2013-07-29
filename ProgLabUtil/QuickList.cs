using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProgLab.Util
{
    #region QuickUpdateList
    /// <summary>
    /// 整合Dictionary及List兩種儲存體。對外提供List(方便給需要DataSource的介面來顯示，例如UltraGrid)，對內則用Dictionary的方式來管理以便快速更新
    /// </summary>
    /// <typeparam name="TKey">資料主索引鍵</typeparam>
    /// <typeparam name="TData">資料</typeparam>
    /// <remarks>
    /// 主索引鍵是由外部在給定的
    /// </remarks>
    public class QuickUpdateList<TKey, TData>
    {
        /// <summary>
        /// 資料存取同步器
        /// </summary>
        private ReaderWriterLock rwLock = new ReaderWriterLock();
        /// <summary>
        /// 以Dictionary方式儲存資料，方便更新使用
        /// </summary>
        private Dictionary<TKey, TData> dicData = new Dictionary<TKey, TData>();
        /// <summary>
        /// 以List型式儲存資料，方便給外部顯示使用
        /// </summary>
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
            try
            {
                rwLock.AcquireWriterLock(Timeout.Infinite);
                try
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
                finally
                {
                    rwLock.ReleaseWriterLock();
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 依指定的Key查詢資料
        /// </summary>
        /// <param name="key">主索引鍵</param>
        /// <returns>傳回查到的資料。查不到時傳回null</returns>
        public TData Query(TKey key)
        {
            try
            {
                rwLock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    if (dicData.ContainsKey(key))
                        return dicData[key];
                    else
                        return default(TData);
                }
                catch
                {
                    return default(TData);
                }
                finally
                {
                    rwLock.ReleaseReaderLock();
                }
            }
            catch
            {
                return default(TData);
            }
        }

        /// <summary>
        /// 刪除項目
        /// </summary>
        /// <param name="key">要刪除的項目的索引鍵</param>
        public void Remove(TKey key)
        {
            try
            {
                rwLock.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    if (dicData.ContainsKey(key))
                    {
                        TData removeData = dicData[key];
                        dicData.Remove(key);

                        listData.Remove(removeData);
                    }
                }
                finally
                {
                    rwLock.ReleaseWriterLock();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 清除所有資料
        /// </summary>
        public void Clear()
        {
            try
            {
                rwLock.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    listData.Clear();
                    dicData.Clear();
                }
                finally
                {
                    rwLock.ReleaseWriterLock();
                }
            }
            catch
            {
            }
        }
    }
    #endregion
}
