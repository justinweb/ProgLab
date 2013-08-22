using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Log
{
    public abstract class LogTargetBase : ILogTarget
    {
        public LogLevelEnum MinLogLevel
        {
            get;
            set;
        }

        #region ILogTarget 成員

        public abstract void WriteLog(LogLevelEnum logLevel, string msg);

        public void Info(string msg)
        {
            WriteLog(LogLevelEnum.Info, msg);
        }

        public void Debug(string msg)
        {
            WriteLog(LogLevelEnum.Debug, msg);
        }

        public void Warn(string msg)
        {
            WriteLog(LogLevelEnum.Warn, msg);
        }

        public void Error(string msg)
        {
            WriteLog(LogLevelEnum.Error, msg);
        }

        #endregion

        protected string MakeLogMessage(LogLevelEnum logLevel, string msg)
        {
            return string.Format("{0} {1} {2}", DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"), logLevel, msg);
        }
    }
}
