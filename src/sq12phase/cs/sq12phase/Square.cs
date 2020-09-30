using System;
using Microsoft.Extensions.Logging;

namespace cs.sq12phase
{
    public class Square
    {
        public int edgeperm;       //number encoding the edge permutation 0-40319
        public int cornperm;       //number encoding the corner permutation 0-40319
        public bool topEdgeFirst;   //true if top layer starts with edge left of seam
        public bool botEdgeFirst;   //true if bottom layer starts with edge right of seam
        public int ml;         //shape of middle layer (+/-1, or 0 if ignored)

        public static sbyte[] SquarePrun = new sbyte[40320 * 2];         //pruning table; #twists to solve corner|edge permutation
        public static char[] TwistMove = new char[40320];          //transition table for twists
        public static char[] TopMove = new char[40320];            //transition table for top layer turns
        public static char[] BottomMove = new char[40320];         //transition table for bottom layer turns

        private static int[] fact = { 1, 1, 2, 6, 24, 120, 720, 5040 };

        public static void set8Perm(sbyte[] arr, int idx)
        {
            int val = 0x76543210;
            for (int i = 0; i < 7; i++)
            {
                int p = fact[7 - i];
                int v = idx / p;
                idx -= v * p;
                v <<= 2;
                arr[i] = (sbyte)((val >> v) & 07);
                int m = (1 << v) - 1;
                val = (val & m) + ((val >> 4) & ~m);
            }
            arr[7] = (sbyte)val;
        }

        public static char get8Perm(sbyte[] arr)
        {
            int idx = 0;
            int val = 0x76543210;
            for (int i = 0; i < 7; i++)
            {
                int v = arr[i] << 2;
                idx = (8 - i) * idx + ((val >> v) & 07);
                val -= 0x11111110 << v;
            }
            return (char)idx;
        }

        public static int[,] Cnk = new int[12, 12];

        public static int get8Comb(sbyte[] arr)
        {
            int idx = 0, r = 4;
            for (int i = 0; i < 8; i++)
            {
                if (arr[i] >= 4)
                {
                    idx += Cnk[7 - i, r--];
                }
            }
            return idx;
        }

        public static bool inited = false;

        public static void init()
        {
            if (inited)
            {
                return;
            }
            for (int i = 0; i < 12; i++)
            {
                Cnk[i, 0] = 1;
                Cnk[i, i] = 1;
                for (int j = 1; j < i; j++)
                {
                    Cnk[i, j] = Cnk[i - 1, j - 1] + Cnk[i - 1, j];
                }
            }
            sbyte[] pos = new sbyte[8];
            sbyte temp;

            for (int i = 0; i < 40320; i++)
            {
                //twist
                set8Perm(pos, i);

                temp = pos[2]; pos[2] = pos[4]; pos[4] = temp;
                temp = pos[3]; pos[3] = pos[5]; pos[5] = temp;
                TwistMove[i] = get8Perm(pos);

                //top layer turn
                set8Perm(pos, i);
                temp = pos[0]; pos[0] = pos[1]; pos[1] = pos[2]; pos[2] = pos[3]; pos[3] = temp;
                TopMove[i] = get8Perm(pos);

                //bottom layer turn
                set8Perm(pos, i);
                temp = pos[4]; pos[4] = pos[5]; pos[5] = pos[6]; pos[6] = pos[7]; pos[7] = temp;
                BottomMove[i] = get8Perm(pos);
            }

            for (int i = 0; i < 40320 * 2; i++)
            {
                SquarePrun[i] = -1;
            }
            SquarePrun[0] = 0;
            int depth = 0;
            int done = 1;
            while (done < 40320 * 2)
            {
                bool inv = depth >= 11;
                int find = inv ? -1 : depth;
                int check = inv ? depth : -1;
                ++depth;
                // OUT:
                for (int i = 0; i < 40320 * 2; i++)
                {
                    if (SquarePrun[i] != find)
                    {
                        continue;
                    }
                    int perm = i >> 1;
                    int ml = i & 1;

                    //try twist
                    int idx = TwistMove[perm] << 1 | (1 - ml);
                    if (SquarePrun[idx] == check)
                    {
                        ++done;
                        SquarePrun[inv ? i : idx] = (sbyte)(depth);
                        if (inv)
                        {
                            // TODO: these breaks are not the same
                            // maybe use a goto?
                            break;
                            //continue OUT;
                        }
                    }
                    //try turning top layer
                    for (int m = 0; m < 4; m++)
                    {
                        perm = TopMove[perm];
                        idx = perm << 1 | ml;
                        if (SquarePrun[idx] == check)
                        {
                            ++done;
                            SquarePrun[inv ? i : idx] = (sbyte)(depth);
                            if (inv)
                            {
                                break;
                                //continue OUT;
                            }
                        }
                    }
                    //try turning bottom layer
                    for (int m = 0; m < 4; m++)
                    {
                        perm = BottomMove[perm];
                        if (SquarePrun[perm << 1 | ml] == check)
                        {
                            ++done;
                            SquarePrun[inv ? i : (perm << 1 | ml)] = (sbyte)(depth);
                            if (inv)
                            {
                                break;
                                //continue OUT;
                            }
                        }
                    }
                }
            }
            inited = true;
        }
    }
}