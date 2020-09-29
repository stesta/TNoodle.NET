using System;

namespace org.worldcubeassociation.tnoodle.svglite
{
    public class Ellipse : Element
    {
        public Ellipse(double cx, double cy, double rx, double ry)
            : base("ellipse")
        {
            setAttribute("cx", "" + cx);
            setAttribute("cy", "" + cy);
            setAttribute("rx", "" + rx);
            setAttribute("ry", "" + ry);
        }

        public Ellipse(Ellipse e)
            : base(e)
        {
        }
    }
}