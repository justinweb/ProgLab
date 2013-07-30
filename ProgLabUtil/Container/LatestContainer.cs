using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Container
{
    public class LatestContainer<TData,TKey> : IQueueContainer<TData>
    {
        private Queue<TKey> queueKey = new Queue<TKey>();
        private Dictionary<TKey, TData> dicData = new Dictionary<TKey, TData>();
        private Func<TData, TKey> GetDatakey = null;
        private object syncRoot = new object();

        public object SyncRoot
        {
            get
            {
                return syncRoot;
            }
        }

        public LatestContainer(Func<TData, TKey> getDatakey)
        {
            GetDatakey = getDatakey;
        }

        #region IQueueContainer<TData> 成員

        public int Count
        {
            get { return queueKey.Count; }
        }

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
