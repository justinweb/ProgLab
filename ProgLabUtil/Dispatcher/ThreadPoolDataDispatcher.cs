using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ProgLab.Util.Container;

namespace ProgLab.Util.Dispatcher
{
    public class ThreadPoolDataDispatcher<TData, TContainer> : IDispatcher<TData>
        where TContainer : IQueueContainer<TData>, new()
    {
        public event Action<TData> OnDataIn = null;

        public int WorkerCount
        {
            get;
            private set;
        }
        public bool IsRunning
        {
            get;
            private set;
        }
        
        private Queue<ManualResetEvent> workerFlag = new Queue<ManualResetEvent>();
        private TContainer queueData = new TContainer();        
        private object lock_DataWorker = new object();
        private bool isStop = false;

        public ThreadPoolDataDispatcher(int workerCount)
        {
            WorkerCount = workerCount;
        }

        public void Add(TData data)
        {
            lock (queueData)
            {
                queueData.Enqueue(data);
            }

            Dispatch();
        }

        private void Dispatch()
        {
            ManualResetEvent freeWorker = null;
            lock (workerFlag)
            {
                if (workerFlag.Count > 0)
                    freeWorker = workerFlag.Dequeue();
            }
            if (freeWorker != null)
            {
                freeWorker.Set();
                ThreadPool.QueueUserWorkItem(WokerJob, freeWorker);                
            }
        }

        public void Start(bool isClearData)
        {
            if (IsRunning)
                return;

            isStop = false;

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

                for (int idx = 0; idx < WorkerCount; ++idx)
                {   
                    ManualResetEvent newFlag = new ManualResetEvent(false);
                    workerFlag.Enqueue(newFlag);                 
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

        public void Stop()
        {
            if (!IsRunning)
                return;

            // 因為是使用ThreadPool，只能用旗標方式通知ThreadPool裏的Thread去停止
            isStop = true;

            lock (lock_DataWorker)
            {
                workerFlag.Clear();             
            }
        }

        private void WokerJob(object status)
        {
            if (status is ManualResetEvent)
            {
                try
                {
                    ManualResetEvent myFlag = (ManualResetEvent)status;

                    while (true)
                    {
                        myFlag.WaitOne();

                        if (isStop)
                            break;

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
