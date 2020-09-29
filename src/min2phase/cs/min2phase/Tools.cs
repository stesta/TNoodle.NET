using System;

namespace cs.min2phase
{
    /**
	 * Some useful functions.
	 */
    public class Tools
    {
        private static Random gen = new Random();

        protected Tools() { }

        /**
		 * Set Random Source.
		 * @param gen new random source.
		 */
        public static void setRandomSource(Random gen)
        {
            Tools.gen = gen;
        }

        /**
		 * Generates a random cube.<br>
		 *
		 * The random source can be set by {@link cs.min2phase.Tools#setRandomSource(java.util.Random)}
		 *
		 * @return A random cube in the string representation. Each cube of the cube space has almost (depends on randomSource) the same probability.
		 *
		 * @see cs.min2phase.Tools#setRandomSource(java.util.Random)
		 * @see cs.min2phase.Search#solution(java.lang.String facelets, int maxDepth, long timeOut, long timeMin, int verbose)
		 */
        public static string randomCube()
        {
            return randomState(STATE_RANDOM, STATE_RANDOM, STATE_RANDOM, STATE_RANDOM, gen);
        }

        public static string randomCube(Random gen)
        {
            return randomState(STATE_RANDOM, STATE_RANDOM, STATE_RANDOM, STATE_RANDOM, gen);
        }

        private static int resolveOri(sbyte[] arr, int _base)
        {
            int sum = 0, idx = 0, lastUnknown = -1;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == -1)
                {
                    arr[i] = (sbyte)gen.Next(_base);
                    lastUnknown = i;
                }
                sum += arr[i];
            }
            if (sum % _base != 0 && lastUnknown != -1)
            {
                arr[lastUnknown] = (sbyte)((30 + arr[lastUnknown] - sum) % _base);
            }
            for (int i = 0; i < arr.Length - 1; i++)
            {
                idx *= _base;
                idx += arr[i];
            }
            return idx;
        }

        private static int countUnknown(sbyte[] arr)
        {
            if (arr == STATE_SOLVED)
            {
                return 0;
            }
            int cnt = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == -1)
                {
                    cnt++;
                }
            }
            return cnt;
        }

        private static int resolvePerm(sbyte[] arr, int cntU, int parity)
        {
            if (arr == STATE_SOLVED)
            {
                return 0;
            }
            else if (arr == STATE_RANDOM)
            {
                return parity == -1 ? gen.Next(2) : parity;
            }
            sbyte[] val = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != -1)
                {
                    val[arr[i]] = -1;
                }
            }
            int idx = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (val[i] != -1)
                {
                    int j = gen.Next(idx + 1);
                    sbyte temp = val[i];
                    val[idx++] = val[j];
                    val[j] = temp;
                }
            }
            int last = -1;
            for (idx = 0; idx < arr.Length && cntU > 0; idx++)
            {
                if (arr[idx] == -1)
                {
                    if (cntU == 2)
                    {
                        last = idx;
                    }
                    arr[idx] = val[--cntU];
                }
            }
            int p = Util.getNParity(getNPerm(arr, arr.Length), arr.Length);
            if (p == 1 - parity && last != -1)
            {
                sbyte temp = arr[idx - 1];
                arr[idx - 1] = arr[last];
                arr[last] = temp;
            }
            return p;
        }

        static int getNPerm(sbyte[] arr, int n)
        {
            int idx = 0;
            for (int i = 0; i < n; i++)
            {
                idx *= (n - i);
                for (int j = i + 1; j < n; j++)
                {
                    if (arr[j] < arr[i])
                    {
                        idx++;
                    }
                }
            }
            return idx;
        }

        protected static readonly sbyte[] STATE_RANDOM = null;
        protected static readonly sbyte[] STATE_SOLVED = new sbyte[0];

        protected static string randomState(sbyte[] cp, sbyte[] co, sbyte[] ep, sbyte[] eo, Random gen)
        {
            int parity;
            int cntUE = ep == STATE_RANDOM ? 12 : countUnknown(ep);
            int cntUC = cp == STATE_RANDOM ? 8 : countUnknown(cp);
            int cpVal, epVal;
            if (cntUE < 2)
            {    //ep != STATE_RANDOM
                if (ep == STATE_SOLVED)
                {
                    epVal = parity = 0;
                }
                else
                {
                    parity = resolvePerm(ep, cntUE, -1);
                    epVal = getNPerm(ep, 12);
                }
                if (cp == STATE_SOLVED)
                {
                    cpVal = 0;
                }
                else if (cp == STATE_RANDOM)
                {
                    do
                    {
                        cpVal = gen.Next(40320);
                    } while (Util.getNParity(cpVal, 8) != parity);
                }
                else
                {
                    resolvePerm(cp, cntUC, parity);
                    cpVal = getNPerm(cp, 8);
                }
            }
            else
            {    //ep != STATE_SOLVED
                if (cp == STATE_SOLVED)
                {
                    cpVal = parity = 0;
                }
                else if (cp == STATE_RANDOM)
                {
                    cpVal = gen.Next(40320);
                    parity = Util.getNParity(cpVal, 8);
                }
                else
                {
                    parity = resolvePerm(cp, cntUC, -1);
                    cpVal = getNPerm(cp, 8);
                }
                if (ep == STATE_RANDOM)
                {
                    do
                    {
                        epVal = gen.Next(479001600);
                    } while (Util.getNParity(epVal, 12) != parity);
                }
                else
                {
                    resolvePerm(ep, cntUE, parity);
                    epVal = getNPerm(ep, 12);
                }
            }
            return Util.toFaceCube(
                       new CubieCube(
                           cpVal,
                           co == STATE_RANDOM ? gen.Next(2187) : (co == STATE_SOLVED ? 0 : resolveOri(co, 3)),
                           epVal,
                           eo == STATE_RANDOM ? gen.Next(2048) : (eo == STATE_SOLVED ? 0 : resolveOri(eo, 2))));
        }


        public static string randomLastLayer()
        {
            return randomState(
                       new sbyte[] { -1, -1, -1, -1, 4, 5, 6, 7 },
                       new sbyte[] { -1, -1, -1, -1, 0, 0, 0, 0 },
                       new sbyte[] { -1, -1, -1, -1, 4, 5, 6, 7, 8, 9, 10, 11 },
                       new sbyte[] { -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0 }, gen);
        }

        public static string randomLastSlot()
        {
            return randomState(
                       new sbyte[] { -1, -1, -1, -1, -1, 5, 6, 7 },
                       new sbyte[] { -1, -1, -1, -1, -1, 0, 0, 0 },
                       new sbyte[] { -1, -1, -1, -1, 4, 5, 6, 7, -1, 9, 10, 11 },
                       new sbyte[] { -1, -1, -1, -1, 0, 0, 0, 0, -1, 0, 0, 0 }, gen);
        }

        public static string randomZBLastLayer()
        {
            return randomState(
                       new sbyte[] { -1, -1, -1, -1, 4, 5, 6, 7 },
                       new sbyte[] { -1, -1, -1, -1, 0, 0, 0, 0 },
                       new sbyte[] { -1, -1, -1, -1, 4, 5, 6, 7, 8, 9, 10, 11 },
                       STATE_SOLVED, gen);
        }

        public static string randomCornerOfLastLayer()
        {
            return randomState(
                       new sbyte[] { -1, -1, -1, -1, 4, 5, 6, 7 },
                       new sbyte[] { -1, -1, -1, -1, 0, 0, 0, 0 },
                       STATE_SOLVED,
                       STATE_SOLVED, gen);
        }

        public static string randomEdgeOfLastLayer()
        {
            return randomState(
                       STATE_SOLVED,
                       STATE_SOLVED,
                       new sbyte[] { -1, -1, -1, -1, 4, 5, 6, 7, 8, 9, 10, 11 },
                       new sbyte[] { -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0 }, gen);
        }

        public static string randomCrossSolved()
        {
            return randomState(
                       STATE_RANDOM,
                       STATE_RANDOM,
                       new sbyte[] { -1, -1, -1, -1, 4, 5, 6, 7, -1, -1, -1, -1 },
                       new sbyte[] { -1, -1, -1, -1, 0, 0, 0, 0, -1, -1, -1, -1 }, gen);
        }

        public static string randomEdgeSolved()
        {
            return randomState(
                       STATE_RANDOM,
                       STATE_RANDOM,
                       STATE_SOLVED,
                       STATE_SOLVED, gen);
        }

        public static string randomCornerSolved()
        {
            return randomState(
                       STATE_SOLVED,
                       STATE_SOLVED,
                       STATE_RANDOM,
                       STATE_RANDOM, gen);
        }

        public static string superFlip()
        {
            return Util.toFaceCube(new CubieCube(0, 0, 0, 2047));
        }


        public static string fromScramble(int[] scramble)
        {
            CubieCube c1 = new CubieCube();
            CubieCube c2 = new CubieCube();
            CubieCube tmp;
            for (int i = 0; i < scramble.Length; i++)
            {
                CubieCube.CornMult(c1, CubieCube.moveCube[scramble[i]], c2);
                CubieCube.EdgeMult(c1, CubieCube.moveCube[scramble[i]], c2);
                tmp = c1; c1 = c2; c2 = tmp;
            }
            return Util.toFaceCube(c1);
        }

        /**
		 * Convert a scramble string to its cube definition string.
		 *
		 * @param s  scramble string. Only outer moves (U, R, F, D, L, B, U2, R2, ...) are recognized.
		 * @return cube definition string, which represent the state generated by the scramble<br>
		 */
        public static string fromScramble(string s)
        {
            int[] arr = new int[s.Length];
            int j = 0;
            int axis = -1;
            for (int i = 0, length = s.Length; i < length; i++)
            {
                switch (s[i])
                {
                    case 'U': axis = 0; break;
                    case 'R': axis = 3; break;
                    case 'F': axis = 6; break;
                    case 'D': axis = 9; break;
                    case 'L': axis = 12; break;
                    case 'B': axis = 15; break;
                    case ' ':
                        if (axis != -1)
                        {
                            arr[j++] = axis;
                        }
                        axis = -1;
                        break;
                    case '2': axis++; break;
                    case '\'': axis += 2; break;
                    default: continue;
                }

            }
            if (axis != -1) arr[j++] = axis;
            int[] ret = new int[j];
            while (--j >= 0)
            {
                ret[j] = arr[j];
            }
            return fromScramble(ret);
        }

        /**
		 * Check whether the cube definition string s represents a solvable cube.
		 *
		 * @param facelets is the cube definition string , see {@link cs.min2phase.Search#solution(java.lang.String facelets, int maxDepth, long timeOut, long timeMin, int verbose)}
		 * @return 0: Cube is solvable<br>
		 *         -1: There is not exactly one facelet of each colour<br>
		 *         -2: Not all 12 edges exist exactly once<br>
		 *         -3: Flip error: One edge has to be flipped<br>
		 *         -4: Not all 8 corners exist exactly once<br>
		 *         -5: Twist error: One corner has to be twisted<br>
		 *         -6: Parity error: Two corners or two edges have to be exchanged
		 */
        public static int verify(string facelets)
        {
            return new Search().verify(facelets);
        }
    }
}