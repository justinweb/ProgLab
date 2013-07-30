using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgLab.Util.Container;

namespace UT
{
    public class LatestUT
    {
        public static void UT()
        {
            LatestContainer<int, int> latestQueue = new LatestContainer<int, int>( PrimitiveIntKey );
            latestQueue.Enqueue(100);
            latestQueue.Enqueue(101);
            latestQueue.Enqueue(111);
        }

        public static int PrimitiveIntKey(int data)
        {
            return data % 10;
        }
    }
}
