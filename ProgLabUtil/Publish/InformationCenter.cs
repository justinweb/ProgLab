using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ProgLab.Util.Extensions;
using System.Threading;
using ProgLab.Util.Log;

namespace ProgLab.Util.Publish
{
    /// <summary>
    /// 維護Client訂閱清單的資料提供中心
    /// </summary>
    /// <typeparam name="TInfoKey">資料的key型別</typeparam>
    /// <typeparam name="TInfo">資料型別</typeparam>
    public class InformationCenter<TInfoKey, TInfo>
    {
        #region Member variables
        protected Dictionary<TInfoKey, List<IInformationClient<TInfo>>> subClients = new Dictionary<TInfoKey,List<IInformationClient<TInfo>>>();
        protected ReaderWriterLock rwlSubClients = new ReaderWriterLock();
        #endregion

        #region inner classes
        protected class PublishInfo<TInfo>
        {
            public IInformationClient<TInfo> Client = null;
            public TInfo Info = default(TInfo);
        }
        #endregion

        /// <summary>
        /// Client跟資料提供中心訂閱指定的資訊
        /// </summary>
        /// <param name="client">實作<see cref="IInformationClient{TInfo}"/>的資料接收者</param>
        /// <param name="infoKey">要訂閱的Key</param>
        /// <returns>成功與否</returns>
        public bool ClientSubscribe( IInformationClient<TInfo> client, TInfoKey infoKey )
        {
            try
            {
                subClients.Add( infoKey, client, rwlSubClients );
                return true;
            }
            catch( Exception exp )
            {
                LogSystem.Instance.Error( exp.ToString() );

                return false;
            }
        }

        /// <summary>
        /// Client跟資料提供中心取消訂閱指定的資訊
        /// </summary>
        /// <param name="client">實作<see cref="IInformationClient{TInfo}"/>的資料接收者</param>
        /// <param name="infoKey">要取消訂閱的Key</param>
        /// <returns>成功與否</returns>
        public bool ClientUnSubscribe(IInformationClient<TInfo> client, TInfoKey infoKey)
        {
            try
            {
                subClients.Remove(infoKey, client, rwlSubClients );
                return true;
            }
            catch (Exception exp)
            {
                LogSystem.Instance.Error(exp.ToString());
                return false;
            }
        }

        /// <summary>
        /// 傳送資訊給訂閱的Client
        /// </summary>
        /// <param name="info">要傳送的資訊</param>
        /// <param name="key">資訊的Key</param>
        public void PublishInformation(TInfo info, TInfoKey key)
        {
            try
            {
                rwlSubClients.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    List<IInformationClient<TInfo>> subList = null;
                    if (subClients.TryGetValue(key, out subList))
                    {
                        foreach (IInformationClient<TInfo> client in subList)
                        {
                            try
                            {
                                SendInformation(client, info);
                            }
                            catch (Exception exp)
                            {
                                LogSystem.Instance.Error("[InfomationCenter]  publish information to client failed");
                            }
                        }
                    }
                }
                finally
                {
                    rwlSubClients.ReleaseReaderLock();
                }
            }
            catch (Exception exp)
            {
                LogSystem.Instance.Error(exp.ToString());
            }
        }

        /// <summary>
        /// 清除訂閱資訊
        /// </summary>
        public void Clear()
        {
            try
            {
                rwlSubClients.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    subClients.Clear();
                }
                finally
                {
                    rwlSubClients.ReleaseWriterLock();
                }
            }
            catch (Exception exp)
            {
                LogSystem.Instance.Error(exp.ToString());
            }
        }

        /// <summary>
        /// 預設實作是以ThreadPool的方式傳送資訊
        /// </summary>
        /// <param name="client">要接收資訊的<see cref="IInformationClient{TInfo}"/></param>
        /// <param name="info">要傳送的資訊</param>
        protected virtual void SendInformation(IInformationClient<TInfo> client, TInfo info)
        {
            ThreadPool.QueueUserWorkItem(PublishToClient, new PublishInfo<TInfo>() { Info = info, Client = client }); 
        }
      
        /// <summary>
        /// 傳送資料到Client端
        /// </summary>
        /// <param name="param"></param>
        protected void PublishToClient(object param)
        {
            try
            {
                if (param is PublishInfo<TInfo>)
                {
                    PublishInfo<TInfo> pubInfo = (PublishInfo<TInfo>)param;

                    pubInfo.Client.Receive(pubInfo.Info);
                }
            }
            catch (Exception exp)
            {
                LogSystem.Instance.Error("[InfomationCenter]  publish information to client failed");
                LogSystem.Instance.Error(exp.ToString());
            }
        }
    }
}
