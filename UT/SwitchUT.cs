using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ProgLab.Util.Container;
using ProgLab.Util.Dispatcher;

namespace UT
{
    public class SwitchUT
    {
        #region UT -- IQueueContainer
        private static int testSeq = 0;
        private static QueueContainer<TestData> firstQueue = new QueueContainer<TestData>();
        private static QueueContainer<TestData> secondQueue = new QueueContainer<TestData>();
        private static SwitchContainer<TestData> switchQueue = null;
        
        public static void UT()
        {
            switchQueue = new SwitchContainer<TestData>(firstQueue,secondQueue); 

            int getCount = 5;
            Thread[] tGetData = new Thread[getCount];
            for( int indexGet = 0; indexGet < getCount; ++indexGet )
            {
                tGetData[indexGet] = new Thread(GetData);
                tGetData[indexGet].IsBackground = true;
                tGetData[indexGet].Name = "GetData#"+indexGet;
            }

            Thread tInsertData = new Thread(InsertData);
            tInsertData.IsBackground = true;
            tInsertData.Name = "InsertData";

            tInsertData.Start();
            for (int indexGet = 0; indexGet < getCount; ++indexGet)
                tGetData[indexGet].Start();

            Console.ReadKey(); 
        }

        private static void GetData()
        {
            int prevID = 0;
            while (true)
            {
                TestData result = switchQueue.Dequeue();
                if (result != null )
                {
                    Console.WriteLine( string.Format( "Get {0} from {1}", result.ID, Thread.CurrentThread.Name) );
                    //System.Diagnostics.Debug.Assert(result.ID == (prevID + 1));
                    prevID = result.ID;
                }
                else
                    Thread.Sleep(100);
            }
        }

        private static void InsertData()
        {
            while (true)
            {
                switchQueue.Enqueue(new TestData() { ID = Interlocked.Increment(ref testSeq) });
                if (testSeq % 1000 == 0)
                    Thread.Sleep(1000);
            }
        }
        #endregion

        //private static DataWorker<TestData, QueueContainer<TestData>> dataWorker = new DataWorker<TestData, QueueContainer<TestData>>(2); 
        private static ThreadPoolDataDispatcher<TestData> dataWorker = null;
        public static void UTDataWorker()
        {
            switchQueue = new SwitchContainer<TestData>(firstQueue, secondQueue); 
            dataWorker = new ThreadPoolDataDispatcher<TestData>(10,switchQueue);             

            dataWorker.OnDataIn += new Action<TestData>(dataWorker_OnDataIn);

            Thread tInsertData = new Thread(InsertDataForWorker);
            tInsertData.IsBackground = true;
            tInsertData.Name = "InsertDataForWorker";            
            tInsertData.Start();
            
            dataWorker.Start( true );

            Console.ReadKey();
 
            dataWorker.Stop();
        }

        static void dataWorker_OnDataIn(TestData obj)
        {
            Console.WriteLine("DataWork data = " + obj.ID); 
        }

        private static void InsertDataForWorker()
        {
            while (true)
            {
                dataWorker.Add(new TestData() { ID = Interlocked.Increment(ref testSeq) });
                if (testSeq % 10000 == 0)
                    Thread.Sleep(1000);
            }
        }
    }

    public class TestData
    {
        public int ID = 0;
    }
}
