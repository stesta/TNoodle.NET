﻿using System;
using System.Collections.Generic;
using System.Text;

namespace org.worldcubeassociation.tnoodle.svglite
{
    public class Circle : Ellipse
    {
        public Circle(double cx, double cy, double r)
            : base(cx, cy, r, r)
        {   
        }

        public Circle(Circle c)
            : base(c)
        {
        }
    }
}
