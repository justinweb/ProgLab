using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ProgLab.Util.Container;

namespace ProgLab.Util.Dispatcher
{
    /// <summary>
    /// 以自行管理執行緒的方式來實作<see cref="IDispatcher"/>介面
    /// </summary>
    /// <typeparam name="TData">資料型別</typeparam>
    /// <typeparam name="TContainer">資料儲存器(<see cref="IQueueContainer"/>)</typeparam>
    public class ThreadDataDispatcher<TData> : IDispatcher<TData>        
    {
        #region Events
        public event Action<TData> OnDataIn = null;
        #endregion

        #region Properties
        /// <summary>
        /// 可用來派送資料的執行緒數
        /// </summary>
        public int WorkerCount
        {
            get;
            private set;
        }        
        /// <inheritdoc/>
        public bool IsRunning
        {
            get;
            private set;
        }
        #endregion

        #region Member variables
        private Queue<ManualResetEvent> workerFlag = new Queue<ManualResetEvent>(); // 放置目前可使用的執行緒同步物件
        private IQueueContainer<TData> queueData = null;    // 資料儲存器
        private List<Thread> workers = new List<Thread>();  // 所有可用的執行緒
        private object lock_DataWorker = new object();      // Start(),Stop()時的同步物件
        #endregion

        #region Constructor
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="workerCount">設定可使用的執行緒數目</param>
        public ThreadDataDispatcher(int workerCount, IQueueContainer<TData> queueData)
        {
            WorkerCount = workerCount;
            this.queueData = queueData;
        }
        #endregion

        #region Functions extened from IDispatcher
        /// <inheritdoc/>
        public void Add(TData data)
        {
            lock (queueData)
            {
                queueData.Enqueue(data);
            }

            Dispatch();
        }

        /// <inheritdoc/>
        public void Start(bool isClearData)
        {
            if (IsRunning)
                return;

            #region 如果要求先清空資料
            if (isClearData)
            {
                lock (queueData)
                {
                    queueData.Clear();
                }
            }
            #endregion

            #region 初始化並執行
            lock (lock_DataWorker)
            {
                IsRunning = true;

                workerFlag.Clear();
                workers.Clear();

                for (int idx = 0; idx < WorkerCount; ++idx)
                {
                    Thread newWorker = new Thread(WokerJob);
                    ManualResetEvent newFlag = new ManualResetEvent(false);
                    workerFlag.Enqueue(newFlag);
                    newWorker.IsBackground = true;
                    newWorker.Start(newFlag);

                    workers.Add(newWorker);
                }

                // 如果已有資料就開始啟動
                lock (queueData)
                {
                    if (queueData.Count > 0)
                        Dispatch();
                }
            }
            #endregion
        }

        /// <inheritdoc/>
        public void Stop()
        {
            if (!IsRunning)
                return;

            lock (lock_DataWorker)
            {
                foreach (Thread worker in workers)
                {
                    worker.Abort();
                }

                workerFlag.Clear();
                workers.Clear();
            }
        }
        #endregion
        
        private void Dispatch()
        {
            ManualResetEvent freeWorker = null;
            lock (workerFlag)
            {
                if (workerFlag.Count > 0)
                    freeWorker = workerFlag.Dequeue();
            }
            if (freeWorker != null)
                freeWorker.Set();
        }

        /// <summary>
        /// 執行緒要執行的函式
        /// </summary>
        /// <param name="status">同步用的物件</param>
        private void WokerJob(object status)
        {
            if (status is ManualResetEvent)
            {
                try
                {
                    ManualResetEvent myFlag = (ManualResetEvent)status;

                    #region worker core function
                    // Get data and notify with events
                    // If there is no more data , then return the flag
                    while (true)
                    {
                        myFlag.WaitOne();

                        TData data = default(TData);
                        lock (queueData)
                        {
                            if (queueData.Count > 0)
                                data = queueData.Dequeue();
                        }
                        if (data != null && data.Equals(default(TData)) == false)
                        {
                            if (OnDataIn != null)
                                OnDataIn(data);
                        }
                        else
                        {
                            // 沒資料要處理時，就歸還Flag
                            myFlag.Reset();
                            lock (workerFlag)
                                workerFlag.Enqueue(myFlag);
                        }
                    }
                    #endregion
                }
                catch (Exception exp)
                {
                    // ERROR
                    Console.WriteLine(exp.Message);
                }
            }
            else
                throw new Exception("資料型別不是AutoResetEvent");
        }
    }
}
