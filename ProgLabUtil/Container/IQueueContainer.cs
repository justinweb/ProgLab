using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProgLab.Util.Container
{
    #region IQueueContainer
    /// <summary>
    /// 要實作同步功能
    /// </summary>
    /// <typeparam name="TData">資料型別</typeparam>
    public interface IQueueContainer<TData>
    {
        /// <summary>
        /// 目前的資料數
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// 加入資料
        /// </summary>
        /// <param name="data">資料</param>
        void Enqueue(TData data);
        /// <summary>
        /// 取出資料
        /// </summary>
        /// <returns>資料</returns>
        TData Dequeue();
        /// <summary>
        /// 如果有資料時就取出
        /// </summary>
        /// <returns>資料。如果沒資料時就傳回null</returns>
        TData GetIfAny();
        /// <summary>
        /// 清除資料
        /// </summary>
        void Clear();

    }
    #endregion

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

    
    public interface IObservable
    {
        event Action<bool> OnChanged;
    }
}
