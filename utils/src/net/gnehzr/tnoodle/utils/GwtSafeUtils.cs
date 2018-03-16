using System;
using System.Collections.Generic;
using System.Text;

using net.gnehzr.tnoodle.svglite;

namespace net.gnehzr.tnoodle.utils
{
    public static class GwtSafeUtils
    {
        private static Color timPurple = new Color(98, 50, 122);
        private static Color orangeHeraldicTincture = new Color(255, 128, 0);
        private static readonly LinkedHashMap<string, Color> WCA_COLORS = new LinkedHashMap<string, Color>()
        {
            { "y", Color.YELLOW },
            { "yellow", Color.YELLOW },
            { "b", Color.BLUE },
            { "blue", Color.BLUE },
            { "r", Color.RED },
            { "red", Color.RED },
            { "w", Color.WHITE },
            { "white", Color.WHITE },
            { "g", Color.GREEN },
            { "green", Color.GREEN },
            { "o", orangeHeraldicTincture },
            { "orange", orangeHeraldicTincture },
            { "p", timPurple },
            { "purple", timPurple },
            { "0", Color.GRAY },
            { "grey", Color.GRAY },
            { "gray", Color.GRAY }
        };
        public static Color toColor(string s)
        {
            if (WCA_COLORS.ContainsKey(s))
            {
                return WCA_COLORS[s];
            }
            try
            {
                return new Color(s);
            }
            catch //(Exception e)
            {
                return null;
            }
        }

        private static void azzertEquals(Object a, Object b, bool assertEquals)
        {
            bool equal;
            if (a == null)
            {
                equal = a == b;
            }
            else
            {
                equal = a.Equals(b);
            }
            string msg = assertEquals ? " should be equal to " :
                                        " should not be equal to ";
            azzert(equal == assertEquals, a + msg + b);
        }
        public static void azzertNotEquals(Object a, Object b)
        {
            azzertEquals(a, b, false);
        }
        public static void azzertEquals(Object a, Object b)
        {
            azzertEquals(a, b, true);
        }
        public static void azzertSame(Object a, Object b)
        {
            azzert(a == b, a + " is not == to " + b);
        }

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

        public static void azzert(bool expr, string message, Exception t)
        {
            if (!expr)
            {
                throw t;
            }
        }

        public static int ceil(double d)
        {
            return (int)Math.Ceiling(d);
        }

        public static string join(Object[] arr, string separator)
        {
            if (separator == null)
            {
                separator = ",";
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(arr[i].ToString());
            }
            return sb.ToString();
        }

        public static string join<H>(List<H> arr, string separator)
        {
            if (separator == null)
            {
                separator = ",";
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < arr.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(arr[i].ToString());
            }
            return sb.ToString();
        }

        public static H choose<H>(Random r, IEnumerable<H> keySet)
        {
            H chosen = default(H);
            int count = 0;
            foreach (H element in keySet)
            {
                if (r.Next(++count) == 0)
                {
                    chosen = element;
                }
            }
            azzert(count > 0);
            return chosen;
        }

        public static string[] parseExtension(string filename)
        {
            int lastDot = filename.LastIndexOf('.');
            string name, extension;
            if (lastDot == -1)
            {
                name = filename;
                extension = null;
            }
            else
            {
                name = filename.Substring(0, lastDot);
                extension = filename.Substring(lastDot + 1).ToLower();
            }
            return new string[] { name, extension };
        }

        public static int toInt(string s, int def)
        {
            try
            {
                return int.Parse(s);
            }
            catch //(Exception e)
            {
                return def;
            }
        }

        public static long tolong(string s, long def)
        {
            try
            {
                return long.Parse(s);
            }
            catch //(Exception e)
            {
                return def;
            }
        }

        /*
        * This is here because gwt doesn't implement clone() or
        * Arrays.copyOf()
        */
        public static int[] clone(int[] src)
        {
            int[] dest = new int[src.Length];
            Array.Copy(src, 0, dest, 0, src.Length);
            return dest;
        }

        public static double[] clone(double[] src)
        {
            double[] dest = new double[src.Length];
            Array.Copy(src, 0, dest, 0, src.Length);
            return dest;
        }

        public static void deepCopy(int[][] src, int[][] dest)
        {
            for (int i = 0; i < src.Length; i++)
            {
                Array.Copy(src[i], 0, dest[i], 0, src[i].Length);
            }
        }

        public static void deepCopy(int[][][] src, int[][][] dest)
        {
            for (int i = 0; i < src.Length; i++)
            {
                deepCopy(src[i], dest[i]);
            }
        }

        public static void deepCopy<T>(T[][] src, T[][] dest)
        {
            for (int i = 0; i < src.Length; i++)
            {
                Array.Copy(src[i], 0, dest[i], 0, src[i].Length);
            }
        }

        public static int indexOf(Object o, Object[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Equals(o))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int modulo(int x, int m)
        {
            azzert(m > 0, "m must be > 0");
            int y = x % m;
            if (y < 0)
            {
                y += m;
            }
            return y;
        }

        public static int[] copyOfRange(int[] src, int from, int to)
        {
            int[] dest = new int[to - from];
            Array.Copy(src, from, dest, 0, dest.Length);
            return dest;
        }

        // TODO: implement fullyReadInputStream
        //public static void fullyReadInputStream(InputStream is, ByteArrayOutputStream bytes)
        //{
        //    final byte[]
        //    buffer = new byte[0x10000];
        //    try
        //    {
        //        for (; ; )
        //        {
        //            int read = is.read(buffer);
        //            if (read < 0)
        //            {
        //                break;
        //            }
        //            bytes.write(buffer, 0, read);
        //        }
        //    }
        //    finally
        //    {
        //            is.close();
        //    }
        //}

        public static LinkedHashMap<B, A> reverseHashMap<A, B>(LinkedHashMap<A, B> map)
        {
            LinkedHashMap<B, A> reverseMap = new LinkedHashMap<B, A>();
            foreach (A a in map.Keys)
            {
                B b = map[a];
                reverseMap.Add(b, a);
            }
            return reverseMap;
        }
    }
}
