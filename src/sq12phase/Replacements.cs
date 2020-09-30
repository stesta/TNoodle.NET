using System;
using System.Collections.Generic;
using System.Text;

namespace cs.sq12phase
{
    public static class Replacements
    {
        public static int BitCount(int n)
        {
            var count = 0;
            while (n != 0)
            {
                count++;
                n &= (n - 1); //walking through all the bits which are set to one
            }

            return count;
        }

        public static void assert(bool assertion)
        {
            if (!assertion)
            {
                throw new Exception();
            }
        }
    }
}
