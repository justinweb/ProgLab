using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProgLab.Util.Container
{
    //#region SwitchContainer
    ///// <summary>
    ///// 實作Double Buffer概念的資料儲存體
    ///// </summary>
    ///// <typeparam name="TContainer"></typeparam>
    ///// <typeparam name="TData"></typeparam>
    //public class SwitchContainer<TContainer, TData> : IQueueContainer<TData>
    //    where TContainer : IQueueContainer<TData>, new()
    //{
    //    #region Member variables
    //    private TContainer firstContainer = new TContainer();
    //    private TContainer secondContainer = new TContainer();

    //    private TContainer inContainer = default(TContainer);
    //    private TContainer outContainer = default(TContainer);

    //    private ReaderWriterLock rwLock = new ReaderWriterLock();   // when switching the writer permission must be requested
    //    private object lock_out = new object();                     // lock for reterive data
    //    #endregion

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    public SwitchContainer()
    //    {
    //        // set in/out container
    //        inContainer = firstContainer;
    //        outContainer = secondContainer;
    //    }

    //    #region IQueueContainer<TData> 成員
    //    /// <inheritdoc/>
    //    public int Count
    //    {
    //        get
    //        {
    //            try
    //            {
    //                rwLock.AcquireReaderLock(Timeout.Infinite);
    //                try
    //                {
    //                    return outContainer.Count + inContainer.Count;
    //                }
    //                finally
    //                {
    //                    rwLock.ReleaseReaderLock();
    //                }
    //            }
    //            catch
    //            {
    //                return 0;
    //            }
    //        }
    //    }
    //    /// <inheritdoc/>
    //    public void Enqueue(TData data)
    //    {
    //        try
    //        {
    //            rwLock.AcquireReaderLock(Timeout.Infinite);
    //            try
    //            {
    //                inContainer.Enqueue(data);
    //            }
    //            finally
    //            {
    //                rwLock.ReleaseReaderLock();
    //            }
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //    }
    //    /// <inheritdoc/>
    //    public TData Dequeue()
    //    {
    //        try
    //        {
    //            rwLock.AcquireReaderLock(Timeout.Infinite);
    //            try
    //            {
    //                // 先由目前的out取值
    //                TData result = outContainer.GetIfAny();
    //                if (result != null && result.Equals(default(TData)) == false)
    //                    return result;

    //                // 目前out裏沒資料的話，就切換
    //                SwitchQueue();

    //                // 再由新的out取值
    //                result = outContainer.GetIfAny();
    //                // 要先比是否為null，是因為如果為非原生型別(ex. int)的話，要先確認是否為null
    //                // 原生型別的話則要用第二段的Equals()來確認是否為預設值
    //                if (result != null && result.Equals(default(TData)) == false)
    //                    return result;

    //                return default(TData);
    //            }
    //            finally
    //            {
    //                rwLock.ReleaseReaderLock();
    //            }
    //        }
    //        catch
    //        {
    //            return default(TData);
    //        }
    //    }
        
    //    /// <summary>
    //    /// 交換輸入及輸出的容器
    //    /// </summary>
    //    private void SwitchQueue()
    //    {
    //        try
    //        {
    //            // 在同一時間可能有一個以上的執行緒因為執行Dequeue()發現沒有資料，而觸發了SwitchQueue()，但這時要確保只有一個執行緒完成交換。
    //            int seqNum = rwLock.WriterSeqNum;
 
    //            LockCookie lc = rwLock.UpgradeToWriterLock(Timeout.Infinite);
    //            try
    //            {
    //                if (rwLock.AnyWritersSince(seqNum) == false)
    //                {
    //                    TContainer tmp = inContainer;
    //                    inContainer = outContainer;
    //                    outContainer = tmp;
    //                }
    //                else
    //                {
    //                    System.Diagnostics.Trace.WriteLine( string.Format("thread #{0} try to switch queue, but has been switched", Thread.CurrentThread.ManagedThreadId) ); 
    //                }
    //            }
    //            finally
    //            {
    //                rwLock.DowngradeFromWriterLock(ref lc);
    //            }
    //            System.Diagnostics.Trace.WriteLine("[SwitchContainer] queue switched");
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    /// 確認是否有資料，有的話就傳回，沒的話就傳回null
    //    /// </summary>
    //    /// <returns></returns>
    //    public TData GetIfAny()
    //    {
    //        lock (lock_out)
    //        {
    //            if (outContainer.Count > 0)
    //            {
    //                return outContainer.Dequeue();
    //            }
    //            else
    //                return default(TData);
    //        }
    //    }
    //    /// <inheritdoc/>
    //    public void Clear()
    //    {
    //        try
    //        {
    //            rwLock.AcquireWriterLock(Timeout.Infinite);
    //            try
    //            {
    //                inContainer.Clear();
    //                outContainer.Clear();
    //            }
    //            finally
    //            {
    //                rwLock.ReleaseWriterLock();
    //            }
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //    }
    //    #endregion
    //}
    //#endregion

    #region SwitchContainer
    /// <summary>
    /// 實作Double Buffer概念的資料儲存體
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class SwitchContainer<TData> : IQueueContainer<TData>        
    {
        #region Member variables
        private IQueueContainer<TData> firstContainer = null;
        private IQueueContainer<TData> secondContainer = null;

        private IQueueContainer<TData> inContainer = null;
        private IQueueContainer<TData> outContainer = null;

        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();   // when switching the writer permission must be requested
        private object lock_out = new object();                     // lock for reterive data
        private int queueSwitchCount = 0;                           // for debug, check the number of switch happened
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SwitchContainer(IQueueContainer<TData> first, IQueueContainer<TData> second )
        {
            firstContainer = first;
            secondContainer = second;
 
            // set in/out container
            inContainer = firstContainer;
            outContainer = secondContainer;
        }

        #region IQueueContainer<TData> 成員
        /// <inheritdoc/>
        public int Count
        {
            get
            {
                try
                {
                    rwLock.EnterReadLock();
                    try
                    {
                        return outContainer.Count + inContainer.Count;
                    }
                    finally
                    {
                        rwLock.ExitReadLock();
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <inheritdoc/>
        public void Enqueue(TData data)
        {
            try
            {
                rwLock.EnterReadLock();
                try
                {
                    inContainer.Enqueue(data);
                }
                finally
                {
                    rwLock.ExitReadLock();
                }
            }
            catch
            {
                throw;
            }
        }
        /// <inheritdoc/>
        public TData Dequeue()
        {
            try
            {
                rwLock.EnterUpgradeableReadLock();
                try
                {
                    // 先由目前的out取值
                    TData result = outContainer.GetIfAny();
                    if (result != null && result.Equals(default(TData)) == false)
                        return result;

                    // 目前out裏沒資料的話，就切換
                    SwitchQueue();

                    // 再由新的out取值
                    result = outContainer.GetIfAny();
                    // 要先比是否為null，是因為如果為非原生型別(ex. int)的話，要先確認是否為null
                    // 原生型別的話則要用第二段的Equals()來確認是否為預設值
                    if (result != null && result.Equals(default(TData)) == false)
                        return result;

                    return default(TData);
                }
                finally
                {
                    rwLock.ExitUpgradeableReadLock();
                }
            }
            catch
            {
                return default(TData);
            }
        }

        /// <summary>
        /// 交換輸入及輸出的容器
        /// </summary>
        private void SwitchQueue()
        {
            try
            {
                rwLock.EnterWriteLock();
                try
                {
                    IQueueContainer<TData> tmp = inContainer;
                    inContainer = outContainer;
                    outContainer = tmp;
                }
                finally
                {
                    rwLock.ExitWriteLock();
                }
                System.Diagnostics.Trace.WriteLine("[SwitchContainer] queue switched ");
            }
            catch(Exception exp )
            {
                throw( new TimeoutException("SwitchContainer::SwitchQueue() timeout", exp)  );
            }
        }

        /// <summary>
        /// 確認是否有資料，有的話就傳回，沒的話就傳回null
        /// </summary>
        /// <returns></returns>
        public TData GetIfAny()
        {
            lock (lock_out)
            {
                if (outContainer.Count > 0)
                {
                    return outContainer.Dequeue();
                }
                else
                    return default(TData);
            }
        }
        /// <inheritdoc/>
        public void Clear()
        {
            try
            {
                rwLock.EnterWriteLock();
                try
                {
                    inContainer.Clear();
                    outContainer.Clear();
                }
                finally
                {
                    rwLock.ExitWriteLock();
                }
            }
            catch(Exception exp)
            {
                throw (new TimeoutException("SwitchContainer::Clear() timeout", exp));
            }
        }
        #endregion
    }
    #endregion
}
