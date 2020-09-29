using System;
using System.Collections.Generic;
using System.Text;

namespace org.worldcubeassociation.tnoodle.svglite
{
    public class Dimension
    {
        public int width { get; }
        public int height { get; }
        
        public Dimension(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public override string ToString()
        {
            return "<" + nameof(Dimension) +
               " width=" + width +
               " height=" + height +
               ">";
        }
    }
}
