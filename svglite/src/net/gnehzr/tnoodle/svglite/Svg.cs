using System;
using System.Collections.Generic;
using System.Text;

namespace net.gnehzr.tnoodle.svglite
{
    public class Svg : Element
    {
        //private double originOffsetX, originOffsetY;
        public Svg(Dimension size)
            : base("svg")
        {
            setSize(size);
            setAttribute("version", "1.1");
            setAttribute("xmlns", "http://www.w3.org/2000/svg");
            //originOffsetX = 0;
            //originOffsetY = 0;
        }

        public void setSize(Dimension size)
        {
            setAttribute("width", "" + size.width + "px");
            setAttribute("height", "" + size.height + "px");
            setAttribute("viewBox", "0 0 " + size.width + " " + size.height);
        }

        public Dimension getSize()
        {
            int width = int.Parse(getAttribute("width").Replace("px", ""));
            int height = int.Parse(getAttribute("height").Replace("px", ""));
            return new Dimension(width, height);
        }
    }
}
