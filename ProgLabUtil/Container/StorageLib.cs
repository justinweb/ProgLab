using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Container
{
    public interface IStorage<TData>
    {
        int Count
        {
            get;
        }

        void Clear();
        void Insert(TData data);
        void Update(TData data);
        void Delete(TData data);

        TData[] ToArray();        
    }

    //#region ObjectStorage
    ///// <summary>
    ///// 放置單一物件用的容器
    ///// </summary>
    ///// <typeparam name="TData"></typeparam>
    //public class ObjectStorage<TData> : IStorage<TData>
    //{
    //    private object lock_element = new object();
    //    private TData element = default(TData);

    //    #region IStorage<TData> 成員

    //    public int Count
    //    {
    //        get { return element == null ? 0 : 1; }
    //    }

    //    public void Clear()
    //    {
    //        lock (lock_element)
    //        {
    //            element = default(TData);
    //        }
    //    }

    //    public void Insert(TData data)
    //    {
    //        lock (lock_element)
    //        {
    //            element = data;
    //        }
    //    }

    //    public void Update(TData data)
    //    {
    //        lock (lock_element)
    //        {
    //            element = data;
    //        }
    //    }

    //    public void Delete(TData data)
    //    {
    //        Clear();
    //    }

    //    #endregion
    //}
    //#endregion

    public class ObjectStorage<TData> : IStorage<TData> where TData : class
    {
        private TData innerData = default(TData);
        private object lock_innerData = new object();

        #region IStorage<TData> 成員

        public int Count
        {
            get { return innerData == default(TData) ? 0 : 1; }
        }

        public void Clear()
        {
            lock (lock_innerData)
            {
                innerData = default(TData);
            }
        }

        public void Insert(TData data)
        {
            lock (lock_innerData)
            {
                innerData = data;
            }
        }

        public void Update(TData data)
        {
            lock (lock_innerData)
            {
                innerData = data;
            }
        }

        public void Delete(TData data)
        {
            lock (lock_innerData)
            {
                innerData = default(TData);
            }
        }

        public TData[] ToArray()
        {
            lock( lock_innerData )
            {
                return new TData[1]{ innerData };
            }
        }

        #endregion
    }

    public class NativeStorage<TData> : IStorage<TData> where TData : struct
    {
        private TData? innerData = null;
        private object lock_innerData = new object();

        #region IStorage<TData> 成員

        public int Count
        {
            get { return innerData == null ? 0 : 1; }
        }

        public void Clear()
        {
            lock (lock_innerData)
            {
                innerData = null;
            }
        }

        public void Insert(TData data)
        {
            lock (lock_innerData)
            {
                innerData = data;
            }
        }

        public void Update(TData data)
        {
            lock (lock_innerData)
            {
                innerData = data;
            }
        }

        public void Delete(TData data)
        {
            lock (lock_innerData)
            {
                innerData = null;
            }
        }

        public TData[] ToArray()
        {
            lock (lock_innerData)
            {
                return new TData[1] { innerData.Value };
            }
        }

        #endregion
    }

    /// <summary>
    /// 包裝成<see cref="IStorage{TData}"/>用來儲存原始型別的Storage
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class DicObjectStorage<TKey, TData> : IStorage<TData>
    {
        private Dictionary<TKey, TData> storage = new Dictionary<TKey, TData>();
        private Func<TData, TKey> funcGetKey = null;

        public DicObjectStorage(Func<TData, TKey> funcGetKey)
        {
            this.funcGetKey = funcGetKey; 
        }

        #region IStorage<TData> 成員

        public int Count
        {
            get { return storage.Count; }
        }

        public void Clear()
        {
            lock (storage)
            {
                storage.Clear();
            }
        }

        public void Insert(TData data)
        {
            lock (storage)
            {
                TKey key = funcGetKey(data);
                storage[key] = data;
            }
        }

        public void Update(TData data)
        {
            lock (storage)
            {
                TKey key = funcGetKey(data);
                storage[key] = data;
            }
        }

        public void Delete(TData data)
        {
            lock (storage)
            {
                TKey key = funcGetKey(data);
                storage.Remove(key); 
            }
        }

        public TData[] ToArray()
        {
            lock (storage)
            {
                return storage.Values.ToArray();
            }
        }

        #endregion
    }


    #region DicStorage
    /// <summary>
    /// 以Dictionary方式儲存的Storage，Key所對應儲存的內容也是一個<see cref="IStorage{TData}"/>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class DicStorage<TKey, TData> : IStorage<TData>
    {
        protected Dictionary<TKey, IStorage<TData>> storage = new Dictionary<TKey, IStorage<TData>>();
        protected Func<TData, TKey> funcGetKey = null;
        protected Func<IStorage<TData>> funcCreateStorage = null;

        public DicStorage(Func<TData, TKey> funcGetKey, Func<IStorage<TData>> funcCreateStorage)
        {
            this.funcGetKey = funcGetKey;
            this.funcCreateStorage = funcCreateStorage;
        }

        #region IStorage<TData> 成員

        public int Count
        {
            get { return storage.Count; }
        }

        public void Clear()
        {
            storage.Clear();
        }

        public void Insert(TData data)
        {
            TKey key = funcGetKey(data);
            lock (storage)
            {
                if (storage.ContainsKey(key) == false)
                {
                    IStorage<TData> secondStorage = funcCreateStorage();
                    storage.Add(key, secondStorage);
                }

                storage[key].Insert(data);
            }
        }

        public void Update(TData data)
        {
            TKey key = funcGetKey(data);
            lock (storage)
            {
                if (storage.ContainsKey(key))
                {
                    storage[key].Update(data);
                }
            }
        }

        public void Delete(TData data)
        {
            TKey key = funcGetKey(data);
            lock (storage)
            {
                if (storage.ContainsKey(key))
                {
                    storage[key].Delete(data);
                    if (storage[key].Count <= 0)
                        storage.Remove(key);
                }
            }
        }

        public TData[] ToArray()
        {
            lock (storage)
            {
                // calculate total count
                int total = 0;
                foreach (KeyValuePair<TKey, IStorage<TData>> pair in storage)
                {
                    total += pair.Value.Count; 
                }
                                
                TData[] result = new TData[total];
                total = 0;
                foreach (KeyValuePair<TKey, IStorage<TData>> pair in storage)
                {
                    Array.Copy(pair.Value.ToArray(), 0, result, total, pair.Value.Count);
                    total += pair.Value.Count;
                }

                return result;
            }
        }

        #endregion
    }
    #endregion

    public sealed class StorageUT
    {
        public static void UT1()
        {
            DicObjectStorage<int, int> storage = new DicObjectStorage<int, int>(
                (a) => { return a; } );
            storage.Insert(10);
            System.Diagnostics.Debug.Assert(storage.Count == 1);
            storage.Insert(20);
            System.Diagnostics.Debug.Assert(storage.Count == 2);
            storage.Insert(10);
            System.Diagnostics.Debug.Assert(storage.Count == 2);

            storage.Delete(10);
            System.Diagnostics.Debug.Assert(storage.Count == 1);

            storage.Insert(20);
            storage.Clear();
            System.Diagnostics.Debug.Assert(storage.Count == 0);
        }

        class LayerData
        {
            public int Key1 = 0;
            public int Key2 = 0;
            public int Data = 0;
        }

        public static void UT2()
        {
            DicStorage<int, LayerData> storage = new DicStorage<int, LayerData>(
                (a) => { return a.Key1; },
                () =>
                {
                    return new DicObjectStorage<int, LayerData>((a) => { return a.Key2; });
                });

            LayerData ldA = new LayerData() { Key1 = 1, Key2 = 2 };
            storage.Insert(ldA);
            System.Diagnostics.Debug.Assert(storage.Count == 1 );

            LayerData ldB = new LayerData() { Key1 = 2, Key2 = 3 };
            storage.Insert(ldB);
            System.Diagnostics.Debug.Assert(storage.Count == 2);

            LayerData ldC = new LayerData() { Key1 = 2, Key2 = 3, Data = 100 };
            storage.Update(ldC);
        }

        public static void UT3()
        {
            Dictionary<int, LayerData> dic = new Dictionary<int, LayerData>();
            LayerData ldA = new LayerData() { Key1 = 1, Key2 = 2 };
            LayerData ldB = new LayerData() { Key1 = 1, Key2 = 2, Data = 100 };

            dic.Add(1, ldA);

            UpdateDictionary_1(dic, 1, ldB); // update failed
            UpdateDictionary_2(dic, 1, ldB); // update successfully 
            
        }

        public static void UpdateDictionary_1<TKey, TData>(Dictionary<TKey,TData> dic, TKey key, TData data)
        {
            TData org = default(TData);
            if (dic.TryGetValue(key, out org))
            {
                org = data;
            }
        }

        public static void UpdateDictionary_2<TKey, TData>(Dictionary<TKey, TData> dic, TKey key, TData data)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = data;
            }
        }


        public static void UT4()
        {
            DicStorage<int, int> storage = new DicStorage<int, int>(
                (a) => { return a; }, () => { return new NativeStorage<int>(); } );
            storage.Insert(10);
            System.Diagnostics.Debug.Assert(storage.Count == 1);
            storage.Insert(20);
            System.Diagnostics.Debug.Assert(storage.Count == 2);
            storage.Insert(10);
            System.Diagnostics.Debug.Assert(storage.Count == 2);

            storage.Delete(10);
            System.Diagnostics.Debug.Assert(storage.Count == 1);

            int[] allData = storage.ToArray();

            storage.Insert(20);
            storage.Clear();
            System.Diagnostics.Debug.Assert(storage.Count == 0);
        }
    }
}
