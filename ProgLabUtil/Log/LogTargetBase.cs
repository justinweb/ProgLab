using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Log
{
    /// <summary>
    /// 實作<see cref="ILogTarget"/>介面的基本Log功能
    /// </summary>
    /// <remarks>
    /// 衍生類別只需再改寫<see cref="WriteLog"/>函式即可
    /// </remarks>
    public abstract class LogTargetBase : ILogTarget
    {
        ///<inheritdoc/>
        public LogLevelEnum MinLogLevel
        {
            get;
            set;
        }

        #region ILogTarget 成員
        ///<inheritdoc/>
        public abstract void WriteLog(LogLevelEnum logLevel, string msg);
        ///<inheritdoc/>
        public void Info(string msg)
        {
            WriteLog(LogLevelEnum.Info, MakeLogMessage(LogLevelEnum.Info, msg));
        }
        ///<inheritdoc/>
        public void Debug(string msg)
        {
            WriteLog(LogLevelEnum.Debug, MakeLogMessage(LogLevelEnum.Debug, msg));
        }
        ///<inheritdoc/>
        public void Warn(string msg)
        {
            WriteLog(LogLevelEnum.Warn, MakeLogMessage(LogLevelEnum.Warn, msg));
        }
        ///<inheritdoc/>
        public void Error(string msg)
        {
            WriteLog(LogLevelEnum.Error, MakeLogMessage(LogLevelEnum.Error, msg));
        }
        #endregion
        /// <summary>
        /// 組成Log訊息
        /// </summary>
        /// <param name="logLevel">Log層級</param>
        /// <param name="msg">原始Log訊息</param>
        /// <returns>加上Log其它資訊的完整Log訊息</returns>
        protected string MakeLogMessage(LogLevelEnum logLevel, string msg)
        {
            return string.Format("{0} [{1}] {2}", DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"), logLevel.ToString(), msg);
        }
    }
}
