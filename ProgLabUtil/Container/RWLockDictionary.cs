using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProgLab.Util.Container
{
    /// <summary>
    /// 使用ReaderWriterLock進行同步化存取的容器類別
    /// </summary>
    /// <typeparam name="TContainer">使用的容器型別</typeparam>
    public class RWLockContainer<TContainer>
    {
        /// <summary>
        /// 資料儲存容器
        /// </summary>
        protected TContainer container = default(TContainer);
        private ReaderWriterLock rwLock = new ReaderWriterLock();

        /// <summary>
        /// 取得目前的資料儲存容器
        /// </summary>
        public TContainer Container
        {
            get { return container; }
        }
        /// <summary>
        /// 取得目前使用的同步化物件
        /// </summary>
        public ReaderWriterLock SyncRoot
        {
            get { return rwLock; }
        }

        #region constructor
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="container"></param>
        public RWLockContainer(TContainer container)
        {
            this.container = container;
        }
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="container"></param>
        /// <param name="rwLock"></param>
        public RWLockContainer(TContainer container, ReaderWriterLock rwLock)
        {
            this.rwLock = rwLock;
        }
        #endregion

        /// <summary>
        /// 以讀取模式存取容器
        /// </summary>
        /// <param name="doReaderMode">存取容器的作業函式</param>
        public void ExecuteReaderMode(Action<TContainer> doReaderMode)
        {
            try
            {
                rwLock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    doReaderMode(Container);
                }
                finally
                {
                    rwLock.ReleaseReaderLock();
                }
            }
            catch (Exception exp)
            {
                //LogSystem.Instance.Error( exp.ToString() );         
            }
        }
        /// <summary>
        /// 以寫入模式存取容器
        /// </summary>
        /// <param name="doWriterMode">寫入容器的作業函式</param>
        public void ExecuteWriterMode(Action<TContainer> doWriterMode)
        {

            try
            {
                rwLock.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    doWriterMode(Container);
                }
                finally
                {
                    rwLock.ReleaseWriterLock();
                }
            }
            catch (Exception exp)
            {
                //LogSystem.Instance.Error( exp.ToString() );         
            }
        }
    }

    /// <summary>
    /// 以Dictionary作為儲存容器的ReaderWriterLock容器
    /// </summary>
    /// <typeparam name="TKey">索引鍵型別</typeparam>
    /// <typeparam name="TValue">值型別</typeparam>
    public class RWLockDictionary<TKey, TValue> : RWLockContainer<Dictionary<TKey, TValue>>
    {
        private ReaderWriterLock rwLock = new ReaderWriterLock();
        /// <summary>
        /// 建構子
        /// </summary>
        public RWLockDictionary()
            : base(new Dictionary<TKey, TValue>())
        {
        }
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="rwLock">指定使用外部所配置的ReaderWriterLock物件</param>
        public RWLockDictionary(ReaderWriterLock rwLock)
            : base(new Dictionary<TKey, TValue>(), rwLock)
        {
        }
    }

    public sealed class RWLockDictionaryUT
    {
        public static void UT_RWLockDictionary()
        {
            int readerCount = 3;
            int writerCount = 1;

            for (int i = 0; i < readerCount; ++i)
            {
                Thread tReader = new Thread(DataReader);
                tReader.Name = "DataReader_" + i.ToString();
                tReader.IsBackground = true;
                tReader.Start();
            }

            for (int i = 0; i < writerCount; ++i)
            {
                Thread tWriter = new Thread(DataWriter);
                tWriter.Name = "DataWriter_" + i.ToString();
                tWriter.IsBackground = true;
                tWriter.Start();
            }

            Console.ReadLine();
        }

        static RWLockDictionary<string, int> rwDictionary = new RWLockDictionary<string, int>();
        static int crtValue = 0;

        static void DataReader()
        {
            while (true)
            {
                rwDictionary.ExecuteReaderMode((x) =>
                {
                    if (x.ContainsKey("Data"))
                    {
                        int value = x["Data"];
                        Console.WriteLine("{0} read value {1}", Thread.CurrentThread.Name, value);
                    }
                });
                Thread.Sleep(10);
            }
        }

        static void DataWriter()
        {
            while (true)
            {
                rwDictionary.ExecuteWriterMode((x) =>
                {
                    int value = Interlocked.Increment(ref crtValue);
                    x["Data"] = value;
                    Console.WriteLine("{0} put value {1}", Thread.CurrentThread.Name, value);
                });
                Thread.Sleep(100);
            }
        }
    }
}
