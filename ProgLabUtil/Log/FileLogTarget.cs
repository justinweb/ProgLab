using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ProgLab.Util.Log
{
    /// <summary>
    /// 以檔案方式記錄Log的Log接收器
    /// </summary>
    public class FileLogTarget : LogTargetBase
    {
        private StreamWriter sw = null;
        private bool isAutoFlush = false;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="filename">檔案名稱</param>
        /// <param name="isAutoFlush">是否自動輸出buffer</param>
        public FileLogTarget( string filename, bool isAutoFlush )
        {
            try{
                this.isAutoFlush = isAutoFlush;
                sw = new StreamWriter( filename, false, System.Text.Encoding.Default );
            }
            catch
            {
                sw = null;
                throw;
            }
        }

        /// <summary>
        /// 關閉檔案
        /// </summary>
        public void Close()
        {
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
        }
        ///<inheritdoc/>
        public override void WriteLog(LogLevelEnum logLevel, string msg)
        {
            if( sw != null )
            {
                if (logLevel >= MinLogLevel)
                {
                    sw.WriteLine(msg);
                    if (!isAutoFlush)
                        sw.Flush();
                }
            }
        }
    }
}
