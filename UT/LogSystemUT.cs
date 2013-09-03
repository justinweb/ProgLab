using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgLab.Util.Log;

namespace UT
{
    public class LogSystemUT
    {
        public static void UT()
        {
            try
            {
                FileLogTarget fileLog = new FileLogTarget("LogSystem_UT.txt", false );
                ConsoleLogTarget consoleLog = new ConsoleLogTarget();
                
                LogSystem.Instance.AttachLogTarget(fileLog);
                LogSystem.Instance.AttachLogTarget(consoleLog);

                // Test Log
                LogSystem.Instance.Warn("Test warn log");
                LogSystem.Instance.Debug("Test debug log");
                fileLog.MinLogLevel = LogLevelEnum.Info;
                LogSystem.Instance.Info("Min LogLevel is set to Info");
                LogSystem.Instance.Debug("If you still could read this line of log, please report bug");
 

                fileLog.Close();
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString()); 
            }
        }
    }
}
