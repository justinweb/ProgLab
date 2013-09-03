using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Log
{
    /// <summary>
    /// 輸出到Console視窗中的Log接收器
    /// </summary>
    public class ConsoleLogTarget : LogTargetBase
    {
        ///<inheritdoc/>
        public override void WriteLog(LogLevelEnum logLevel, string msg)
        {
            if( logLevel >= MinLogLevel )
                Console.WriteLine( msg ); 
        }
    }
}
