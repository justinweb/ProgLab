using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Container
{
    #region QueueContainer
    /// <summary>
    /// 包裝基本的Queue，實作IQueueContainer
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class QueueContainer<TData> : IQueueContainer<TData>
    {
        /// <summary>
        /// 內部的Queue
        /// </summary>
        private Queue<TData> innerQueue = new Queue<TData>();

        #region IQueueContainer<TData> 成員
        /// <inheritdoc/>
        public int Count
        {
            get { return innerQueue.Count; }
        }
        /// <inheritdoc/>
        public void Enqueue(TData data)
        {
            lock (innerQueue)
            {
                innerQueue.Enqueue(data);
            }
        }
        /// <inheritdoc/>
        public TData Dequeue()
        {
            lock (innerQueue)
            {
                return innerQueue.Dequeue();
            }
        }
        /// <inheritdoc/>
        public TData GetIfAny()
        {
            lock (innerQueue)
            {
                if (Count > 0)
                {
                    return Dequeue();
                }
                else
                    return default(TData);
            }
        }
        /// <inheritdoc/>
        public void Clear()
        {
            lock (innerQueue)
            {
                innerQueue.Clear();
            }
        }
        #endregion
    }
    #endregion
}
