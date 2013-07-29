using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProgLab.Util.Dispatcher
{
    /// <summary>
    /// 資料派送器
    /// </summary>
    /// <typeparam name="TData">要派送的資料型別</typeparam>
    public interface IDispatcher<TData>
    {
        #region Events
        /// <summary>
        /// 通知外部處理資料
        /// </summary>
        event Action<TData> OnDataIn;
        #endregion

        #region 屬性
        /// <summary>
        /// 是否正在執行中
        /// </summary>
        bool IsRunning
        {
            get;
        }
        #endregion

        /// <summary>
        /// 加入資料
        /// </summary>
        /// <param name="data">資料</param>
        void Add(TData data);
        /// <summary>
        /// 啟動派送器
        /// </summary>
        /// <param name="isClearData">是否要清空目前資料</param>
        void Start(bool isClearData);
        /// <summary>
        /// 停止派送器
        /// </summary>
        void Stop();
    }

    
}
