using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Container
{
    public interface ILatestQueueKey<TKey>
    {
        TKey GetKey();
    }

    public class MyData
    {
        public int ID = 0;
    }

    public class LatestQueueKeyWrapper<TData, TKey> : ILatestQueueKey<TKey>
    {
        #region ILatestQueueKey<TKey> 成員

        public TKey GetKey()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
