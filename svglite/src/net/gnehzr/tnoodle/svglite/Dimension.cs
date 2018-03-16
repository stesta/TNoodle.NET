using System;
using System.Collections.Generic;
using System.Text;

namespace net.gnehzr.tnoodle.svglite
{
    public class Dimension
    {
        public int width, height;
        public Dimension(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public int getWidth()
        {
            return width;
        }

        public int getHeight()
        {
            return height;
        }

        public String toString()
        {
            return "<" + typeof(Dimension).Name+
               " width=" + width +
               " height=" + height +
               ">";
        }
    }
}
