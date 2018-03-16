using System;
using System.Collections.Generic;
using System.Text;

namespace net.gnehzr.tnoodle
{
    public class Logger
    {
        public static Logger getLogger(string name)
        {
            return new Logger();
        }

        public void log(Level level, string message, Exception e)
        {
            
        }
    }

    public enum Level
    {
        SEVERE
    }
}
