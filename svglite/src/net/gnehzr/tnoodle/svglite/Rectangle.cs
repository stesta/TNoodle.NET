using System;
using System.Collections.Generic;
using System.Text;

namespace net.gnehzr.tnoodle.svglite
{
    public class Rectangle : Element
    {
        public Rectangle(double x, double y, double width, double height)
            : base("rect")
        {   
            setAttribute("x", "" + x);
            setAttribute("y", "" + y);
            setAttribute("width", "" + width);
            setAttribute("height", "" + height);
        }

        public Rectangle(Rectangle r)
            : base(r)
        {
        }
    }
}
