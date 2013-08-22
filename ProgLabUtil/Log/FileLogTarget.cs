using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ProgLab.Util.Log
{
    public class FileLogTarget : LogTargetBase
    {
        private StreamWriter sw = null;

        public FileLogTarget( string filename )
        {
            try{
                sw = new StreamWriter( filename, false, System.Text.Encoding.Default );
            }
            catch
            {
                sw = null;
                throw;
            }
        }

        public void Close()
        {
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
        }

        public override void WriteLog(LogLevelEnum logLevel, string msg)
        {
            if( sw != null )
            {
                sw.WriteLine( MakeLogMessage( logLevel, msg ) ); 
            }
        }
    }
}
