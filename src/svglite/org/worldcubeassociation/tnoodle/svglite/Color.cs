using System;
using System.Collections.Generic;
using System.Text;

namespace org.worldcubeassociation.tnoodle.svglite
{
    public class Color
    {
        public static readonly Color RED = new Color(255, 0, 0);
        public static readonly Color GREEN = new Color(0, 255, 0);
        public static readonly Color BLUE = new Color(0, 0, 255);
        public static readonly Color WHITE = new Color(255, 255, 255);
        public static readonly Color BLACK = new Color(0, 0, 0);
        public static readonly Color GRAY = new Color(128, 128, 128);
        public static readonly Color YELLOW = new Color(255, 255, 0);

        private int r, g, b, a;
        public Color(int r, int g, int b, int a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public Color(int r, int g, int b)
            : this(r, g, b, 255)
        {
        }

        public Color(int rgba)
            : this((int)((uint)rgba >> 8*2) & 0xff,
                    (int)((uint)rgba >> 8) & 0xff,
                    rgba & 0xff,
                    (int)((uint)rgba >> 8*3) & 0xff)
        {
        }

        private static int hexToRGB(string htmlHex)
        {
            if (htmlHex.StartsWith("#"))
            {
                htmlHex = htmlHex.Substring(1);
            }

            switch (htmlHex.Length)
            {
                case 3:
                    char c0 = htmlHex[0];
                    char c1 = htmlHex[1];
                    char c2 = htmlHex[2];
                    htmlHex = "" + c0 + c0 + c1 + c1 + c2 + c2;
                    break;
                case 6:
                    break;
                default:
                    throw new InvalidHexColorException(htmlHex);
            }

            return int.Parse(htmlHex);
        }

        public Color(string htmlHex)
            : this(hexToRGB(htmlHex))
        {
        }

        public Color invertColor()
        {
            return new Color(255 - r, 255 - g, 255 - b);
        }

        // TODO: double check the toHex implementation
        public string toHex()
        {
            return (0x1000000 | (getRGB() & 0xffffff)).ToString("X2");
        }

        public int getRed()
        {
            return r;
        }

        public int getGreen()
        {
            return g;
        }

        public int getBlue()
        {
            return b;
        }

        public int getRGB()
        {
            return (a << 8*3) | (r << 8*2) | (g << 8) | b;
        }

        public override string ToString()
        {
            return "<color #" + toHex() + ">";
        }
    }
}
