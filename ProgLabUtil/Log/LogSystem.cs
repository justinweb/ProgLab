using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProgLab.Util.Log
{
    public sealed class LogSystem : ILogTarget
    {
        #region Member variables
        private static LogSystem instance = new LogSystem();
        private List<ILogTarget> allLogTarget = new List<ILogTarget>();
        private ReaderWriterLock rwlLogTargetList = new ReaderWriterLock();
        #endregion

        #region Porperties
        public static LogSystem Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region constructor
        private LogSystem()
        {
        }
        #endregion

        public bool AttachLogTarget(ILogTarget logTarget)
        {
            try
            {
                rwlLogTargetList.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    allLogTarget.Add(logTarget);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    rwlLogTargetList.ReleaseWriterLock();
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DetachLogTarget(ILogTarget logTarget)
        {
            try
            {
                rwlLogTargetList.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    allLogTarget.Remove(logTarget);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    rwlLogTargetList.ReleaseWriterLock();
                }
            }
            catch
            {
                return false;
            }
        }

        #region ILogTarget 成員

        public LogLevelEnum MinLogLevel
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void WriteLog(LogLevelEnum logLevel, string msg)
        {
            try
            {
                rwlLogTargetList.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    foreach (ILogTarget logTarget in allLogTarget)
                    {
                        logTarget.WriteLog(logLevel, msg); 
                    }
                }
                finally
                {
                    rwlLogTargetList.ReleaseReaderLock();
                }
            }
            catch
            {
                throw;
            }
        }

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
    }
}
