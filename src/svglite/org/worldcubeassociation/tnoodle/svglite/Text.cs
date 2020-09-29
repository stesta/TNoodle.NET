using System;
using System.Collections.Generic;
using System.Text;

namespace org.worldcubeassociation.tnoodle.svglite
{
    public class Text : Element
    {
        public Text(string text, double x, double y)
            : base("text")
        {
            setContent(text);
            setAttribute("x", "" + x);
            setAttribute("y", "" + y);
        }
    }
}
