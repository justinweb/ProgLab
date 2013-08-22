using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Log
{
    public interface ILogTarget
    {
        LogLevelEnum MinLogLevel
        {
            get;
            set;
        }

        void WriteLog(LogLevelEnum logLevel, string msg);

        void Info(string msg);
        void Debug(string msg);
        void Warn(string msg);
        void Error(string msg);

    }
}
