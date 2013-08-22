using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgLab.Util.Log
{
    public class ConsoleLogTarget : LogTargetBase
    {
        public override void WriteLog(LogLevelEnum logLevel, string msg)
        {
            if( logLevel >= MinLogLevel )
                Console.WriteLine( MakeLogMessage( logLevel, msg ) ); 
        }
    }
}
