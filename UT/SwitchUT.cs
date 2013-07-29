using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ProgLab.Util.Container;
using ProgLab.Util.Dispatcher;

namespace TestUT.Switch
{
    public class SwitchUT
    {
        #region UT -- IQueueContainer
        private static int testSeq = 0;
        private static SwitchContainer<QueueContainer<TestData>, TestData> switchQueue = new SwitchContainer<QueueContainer<TestData>, TestData>(); 
        
        public static void UT()
        {
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
        private static ThreadPoolDataDispatcher<TestData, SwitchContainer<QueueContainer<TestData>, TestData>> dataWorker =
            new ThreadPoolDataDispatcher<TestData, SwitchContainer<QueueContainer<TestData>, TestData>>(10); 
        public static void UTDataWorker()
        {
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
