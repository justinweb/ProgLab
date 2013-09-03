using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Log
{
    /// <summary>
    /// 可接收Log資訊的目標
    /// </summary>
    public interface ILogTarget
    {
        /// <summary>
        /// 設定所要接收的Log的最小層級
        /// </summary>
        LogLevelEnum MinLogLevel
        {
            get;
            set;
        }

        /// <summary>
        /// 寫入Log
        /// </summary>
        /// <param name="logLevel">Log層級</param>
        /// <param name="msg">Log訊息</param>
        void WriteLog(LogLevelEnum logLevel, string msg);

        /// <summary>
        /// 寫入Log層級為Info的Log資訊
        /// </summary>
        /// <param name="msg">Log資訊</param>
        void Info(string msg);
        /// <summary>
        /// 寫入Log層級為Debug的Log資訊
        /// </summary>
        /// <param name="msg">Log資訊</param>
        void Debug(string msg);
        /// <summary>
        /// 寫入Log層級為Warn的Log資訊
        /// </summary>
        /// <param name="msg">Log資訊</param>
        void Warn(string msg);
        /// <summary>
        /// 寫入Log層級為Error的Log資訊
        /// </summary>
        /// <param name="msg">Log資訊</param>
        void Error(string msg);

    }
}
