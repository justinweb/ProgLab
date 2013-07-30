using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Container
{
    /// <summary>
    /// 以key的方式儲存最後一筆資訊
    /// </summary>
    /// <typeparam name="TData">資料型別</typeparam>
    /// <typeparam name="TKey">索引鍵型別</typeparam>
    public class LatestContainer<TData,TKey> : IQueueContainer<TData>
    {
        private Queue<TKey> queueKey = new Queue<TKey>();
        private Dictionary<TKey, TData> dicData = new Dictionary<TKey, TData>();
        private Func<TData, TKey> GetDatakey = null;
        private object syncRoot = new object();

        /// <summary>
        /// 同步化物件
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return syncRoot;
            }
        }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="getDatakey">為資料型別產生索引鍵的函式</param>
        public LatestContainer(Func<TData, TKey> getDatakey)
        {
            GetDatakey = getDatakey;
        }

        #region IQueueContainer<TData> 成員
        /// <inheritdoc/>
        public int Count
        {
            get { return queueKey.Count; }
        }
        /// <inheritdoc/>
        public void Enqueue(TData data)
        {
            System.Diagnostics.Debug.Assert(GetDatakey != null);

            TKey key = GetDatakey(data);

            lock (SyncRoot)
            {
                if (dicData.ContainsKey(key))
                {
                    dicData[key] = data;
                }
                else
                {
                    dicData.Add(key, data);
                    queueKey.Enqueue(key);
                }
            }
        }
        /// <inheritdoc/>
        public TData Dequeue()
        {
            lock (SyncRoot)
            {
                if (queueKey.Count > 0)
                {
                    TKey crtKey = queueKey.Dequeue();
                    if (dicData.ContainsKey(crtKey))
                    {
                        TData result = dicData[crtKey];
                        dicData.Remove(crtKey);
                        return result;
                    }
                    return default(TData);
                }
                else
                    return default(TData);
            }
        }
        /// <inheritdoc/>
        public TData GetIfAny()
        {
            lock (SyncRoot)
            {
                if (Count > 0)
                    return Dequeue();
                else
                    return default(TData);
            }
        }
        /// <inheritdoc/>
        public void Clear()
        {
            lock (SyncRoot)
            {
                dicData.Clear();
                queueKey.Clear(); 
            }
        }

        #endregion
    }
}
