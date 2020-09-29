using System;
using System.Collections.Generic;
using System.Text;

namespace org.worldcubeassociation.tnoodle.svglite
{
    public static class Utils
    {
        public static void azzert(bool expr)
        {
            if (!expr)
            {
                throw new Exception();
            }
        }

        public static void azzert(bool expr, string message)
        {
            if (!expr)
            {
                throw new Exception(message);
            }
        }

        public static void azzert(bool expr, Exception t)
        {
            if (!expr)
            {
                throw t; 
            }
        }
    }
}
