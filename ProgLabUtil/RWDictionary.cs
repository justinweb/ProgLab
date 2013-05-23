using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProgLab.Util
{
    public class RWDictionary<TKey, TData>
    {
        private ReaderWriterLock lockData = new ReaderWriterLock();
        private Dictionary<TKey, TData> dicData = new Dictionary<TKey, TData>();

        public TData Query(TKey key, int timeoutPeriod)
        {
            try
            {
                lockData.AcquireReaderLock(timeoutPeriod);
                TData result = default(TData);
                dicData.TryGetValue(key, out result);
                lockData.ReleaseReaderLock();

                return result;
            }
            catch (Exception exp)
            {
                return default(TData);
            }
        }

        public void Add(TKey key, TData data)
        {
            try
            {
                lockData.AcquireWriterLock(-1);
                if (dicData.ContainsKey(key) == false)
                    dicData.Add(key, data);
                lockData.ReleaseWriterLock();
            }
            catch (Exception exp)
            {
                // Timeout
            }
        }

        public void Remove(TKey key)
        {
            try
            {
                lockData.AcquireWriterLock(-1);
                if (dicData.ContainsKey(key))
                    dicData.Remove(key);
                lockData.ReleaseWriterLock();
            }
            catch (Exception exp)
            {
                // Timeout
            }
        }

        public void Clear()
        {
            try
            {
                lockData.AcquireWriterLock(-1);
                dicData.Clear();
                lockData.ReleaseWriterLock();
            }
            catch (Exception exp)
            {
                // Timeout
            }
        }
    }
}
