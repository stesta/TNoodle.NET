/*
    Copyright (C) 2015  Shuang Chen

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace cs.min2phase
{
    /**
	 * Rubik's Cube Solver.<br>
	 * A much faster and smaller implemention of Two-Phase Algorithm.<br>
	 * Symmetry is used to reduce memory used.<br>
	 * Total Memory used is about 1MB.<br>
	 * @author Shuang Chen
	 */
    public class Search
    {

        public static readonly bool USE_TWIST_FLIP_PRUN = true;

        //Options for research purpose.
        public static readonly int MAX_PRE_MOVES = 20;
        public static readonly bool TRY_INVERSE = true;
        public static readonly bool TRY_THREE_AXES = true;

        public static readonly bool USE_COMBP_PRUN = USE_TWIST_FLIP_PRUN;
        public static readonly bool USE_CONJ_PRUN = USE_TWIST_FLIP_PRUN;
        protected static int MIN_P1LENGTH_PRE = 7;
        protected static int MAX_DEPTH2 = 12;

        static bool inited = false;

        protected int[] move = new int[31];
        protected int[] moveSol = new int[31];

        protected CoordCube[] nodeUD = new CoordCube[21];
        protected CoordCube[] nodeRL = new CoordCube[21];
        protected CoordCube[] nodeFB = new CoordCube[21];

        protected long selfSym;
        protected int conjMask;
        protected int urfIdx;
        protected int length1;
        protected int depth1;
        protected int maxDep2;
        protected int sol;
        protected string _solution;
        protected long probe;
        protected long probeMax;
        protected long probeMin;
        protected int verbose;
        protected int valid1;
        protected bool allowShorter = false;
        protected CubieCube cc = new CubieCube();
        protected CubieCube[] urfCubieCube = new CubieCube[6];
        protected CoordCube[] urfCoordCube = new CoordCube[6];
        protected CubieCube[] phase1Cubie = new CubieCube[21];

        CubieCube[] preMoveCubes = new CubieCube[MAX_PRE_MOVES + 1];
        int[] preMoves = new int[MAX_PRE_MOVES];
        public int preMoveLen = 0;
        public int maxPreMoves = 0;

        protected bool isRec = false;

        /**
		 *     Verbose_Mask determines if a " . " separates the phase1 and phase2 parts of the solver string like in F' R B R L2 F .
		 *     U2 U D for example.<br>
		 */
        public static readonly int USE_SEPARATOR = 0x1;

        /**
		 *     Verbose_Mask determines if the solution will be inversed to a scramble/state generator.
		 */
        public static readonly int INVERSE_SOLUTION = 0x2;

        /**
		 *     Verbose_Mask determines if a tag such as "(21f)" will be appended to the solution.
		 */
        public static readonly int APPEND_LENGTH = 0x4;

        /**
		 *     Verbose_Mask determines if guaranteeing the solution to be optimal.
		 */
        public static readonly int OPTIMAL_SOLUTION = 0x8;


        public Search()
        {
            for (int i = 0; i < 21; i++)
            {
                nodeUD[i] = new CoordCube();
                nodeRL[i] = new CoordCube();
                nodeFB[i] = new CoordCube();
                phase1Cubie[i] = new CubieCube();
            }
            for (int i = 0; i < 6; i++)
            {
                urfCubieCube[i] = new CubieCube();
                urfCoordCube[i] = new CoordCube();
            }
            for (int i = 0; i < MAX_PRE_MOVES; i++)
            {
                preMoveCubes[i + 1] = new CubieCube();
            }
        }

        /**
		 * Computes the solver string for a given cube.
		 *
		 * @param facelets
		 *      is the cube definition string format.<br>
		 * The names of the facelet positions of the cube:
		 * <pre>
		 *             |************|
		 *             |*U1**U2**U3*|
		 *             |************|
		 *             |*U4**U5**U6*|
		 *             |************|
		 *             |*U7**U8**U9*|
		 *             |************|
		 * ************|************|************|************|
		 * *L1**L2**L3*|*F1**F2**F3*|*R1**R2**R3*|*B1**B2**B3*|
		 * ************|************|************|************|
		 * *L4**L5**L6*|*F4**F5**F6*|*R4**R5**R6*|*B4**B5**B6*|
		 * ************|************|************|************|
		 * *L7**L8**L9*|*F7**F8**F9*|*R7**R8**R9*|*B7**B8**B9*|
		 * ************|************|************|************|
		 *             |************|
		 *             |*D1**D2**D3*|
		 *             |************|
		 *             |*D4**D5**D6*|
		 *             |************|
		 *             |*D7**D8**D9*|
		 *             |************|
		 * </pre>
		 * A cube definition string "UBL..." means for example: In position U1 we have the U-color, in position U2 we have the
		 * B-color, in position U3 we have the L color etc. For example, the "super flip" state is represented as <br>
		 * <pre>UBULURUFURURFRBRDRFUFLFRFDFDFDLDRDBDLULBLFLDLBUBRBLBDB</pre>
		 * and the state generated by "F U' F2 D' B U R' F' L D' R' U' L U B' D2 R' F U2 D2" can be represented as <br>
		 * <pre>FBLLURRFBUUFBRFDDFUULLFRDDLRFBLDRFBLUUBFLBDDBUURRBLDDR</pre>
		 * You can also use {@link cs.min2phase.Tools#fromScramble(java.lang.String s)} to convert the scramble string to the
		 * cube definition string.
		 *
		 * @param maxDepth
		 *      defines the maximal allowed maneuver length. For random cubes, a maxDepth of 21 usually will return a
		 *      solution in less than 0.02 seconds on average. With a maxDepth of 20 it takes about 0.1 seconds on average to find a
		 *      solution, but it may take much longer for specific cubes.
		 *
		 * @param probeMax
		 *      defines the maximum number of the probes of phase 2. If it does not return with a solution, it returns with
		 *      an error code.
		 *
		 * @param probeMin
		 *      defines the minimum number of the probes of phase 2. So, if a solution is found within given probes, the
		 *      computing will continue to find shorter solution(s). Btw, if probeMin &gt; probeMax, probeMin will be set to probeMax.
		 *
		 * @param verbose
		 *      determins the format of the solution(s). see USE_SEPARATOR, INVERSE_SOLUTION, APPEND_LENGTH, OPTIMAL_SOLUTION
		 *
		 * @return The solution string or an error code:<br>
		 *      Error 1: There is not exactly one facelet of each colour<br>
		 *      Error 2: Not all 12 edges exist exactly once<br>
		 *      Error 3: Flip error: One edge has to be flipped<br>
		 *      Error 4: Not all corners exist exactly once<br>
		 *      Error 5: Twist error: One corner has to be twisted<br>
		 *      Error 6: Parity error: Two corners or two edges have to be exchanged<br>
		 *      Error 7: No solution exists for the given maxDepth<br>
		 *      Error 8: Probe limit exceeded, no solution within given probMax
		 */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual string solution(string facelets, int maxDepth, long probeMax, long probeMin, int verbose)
        {
            int check = verify(facelets);
            if (check != 0)
            {
                return "Error " + Math.Abs(check);
            }
            this.sol = maxDepth + 1;
            this.probe = 0;
            this.probeMax = probeMax;
            this.probeMin = Math.Min(probeMin, probeMax);
            this.verbose = verbose;
            this._solution = null;
            this.isRec = false;

            CoordCube.init(false);
            initSearch();

            return (verbose & OPTIMAL_SOLUTION) == 0 ? search() : searchopt();
        }

        protected virtual void initSearch()
        {
            conjMask = (TRY_INVERSE ? 0 : 0x38) | (TRY_THREE_AXES ? 0 : 0x36);
            selfSym = cc.selfSymmetry();
            conjMask |= (selfSym >> 16 & 0xffff) != 0 ? 0x12 : 0;
            conjMask |= (selfSym >> 32 & 0xffff) != 0 ? 0x24 : 0;
            conjMask |= (selfSym >> 48 & 0xffff) != 0 ? 0x38 : 0;
            selfSym &= 0xffffffffffffL;
            maxPreMoves = conjMask > 7 ? 0 : MAX_PRE_MOVES;

            for (int i = 0; i < 6; i++)
            {
                urfCubieCube[i].copy(cc);
                urfCoordCube[i].setWithPrun(urfCubieCube[i], 20);
                cc.URFConjugate();
                if (i % 3 == 2)
                {
                    cc.invCubieCube();
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string next(long probeMax, long probeMin, int verbose)
        {
            this.probe = 0;
            this.probeMax = probeMax;
            this.probeMin = Math.Min(probeMin, probeMax);
            this._solution = null;
            this.isRec = (this.verbose & OPTIMAL_SOLUTION) == (verbose & OPTIMAL_SOLUTION);
            this.verbose = verbose;
            return (verbose & OPTIMAL_SOLUTION) == 0 ? search() : searchopt();
        }

        public static bool isInited()
        {
            return inited;
        }

        public long numberOfProbes()
        {
            return probe;
        }

        public int length()
        {
            return sol;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void init()
        {
            if (!inited)
            {
                CoordCube.init(true);
                inited = true;
            }
        }

        public int verify(string facelets)
        {
            int count = 0x000000;
            sbyte[] f = new sbyte[54];
            try
            {
                string center = new string(
                    new char[] {
                    facelets[Util.U5],
                    facelets[Util.R5],
                    facelets[Util.F5],
                    facelets[Util.D5],
                    facelets[Util.L5],
                    facelets[Util.B5]
                    }
                );
                for (int i = 0; i < 54; i++)
                {
                    f[i] = (sbyte)center.IndexOf(facelets[i]);
                    if (f[i] == -1)
                    {
                        return -1;
                    }
                    count += 1 << (f[i] << 2);
                }
            }
            catch (Exception e)
            {
                return -1;
            }
            if (count != 0x999999)
            {
                return -1;
            }
            Util.toCubieCube(f, cc);
            return cc.verify();
        }

        protected virtual int phase1PreMoves(int maxl, int lm, CubieCube cc, int ssym)
        {
            preMoveLen = maxPreMoves - maxl;
            if (isRec ? depth1 == length1 - preMoveLen
                    : (preMoveLen == 0 || (0x36FB7 >> lm & 1) == 0))
            {
                depth1 = length1 - preMoveLen;
                phase1Cubie[0] = cc;
                allowShorter = depth1 == MIN_P1LENGTH_PRE && preMoveLen != 0;

                if (nodeUD[depth1 + 1].setWithPrun(cc, depth1)
                        && phase1(nodeUD[depth1 + 1], ssym, depth1, -1) == 0)
                {
                    return 0;
                }
            }

            if (maxl == 0 || preMoveLen + MIN_P1LENGTH_PRE >= length1)
            {
                return 1;
            }

            int skipMoves = CubieCube.getSkipMoves(ssym);
            if (maxl == 1 || preMoveLen + 1 + MIN_P1LENGTH_PRE >= length1)
            { //last pre move
                skipMoves |= 0x36FB7; // 11 0110 1111 1011 0111
            }

            lm = lm / 3 * 3;
            for (int m = 0; m < 18; m++)
            {
                if (m == lm || m == lm - 9 || m == lm + 9)
                {
                    m += 2;
                    continue;
                }
                if (isRec && m != preMoves[maxPreMoves - maxl] || (skipMoves & 1 << m) != 0)
                {
                    continue;
                }
                CubieCube.CornMult(CubieCube.moveCube[m], cc, preMoveCubes[maxl]);
                CubieCube.EdgeMult(CubieCube.moveCube[m], cc, preMoveCubes[maxl]);
                preMoves[maxPreMoves - maxl] = m;
                int ret = phase1PreMoves(maxl - 1, m, preMoveCubes[maxl], ssym & (int)CubieCube.moveCubeSym[m]);
                if (ret == 0)
                {
                    return 0;
                }
            }
            return 1;
        }

        protected string search()
        {
            for (length1 = isRec ? length1 : 0; length1 < sol; length1++)
            {
                maxDep2 = Math.Min(MAX_DEPTH2, sol - length1 - 1);
                for (urfIdx = isRec ? urfIdx : 0; urfIdx < 6; urfIdx++)
                {
                    if ((conjMask & 1 << urfIdx) != 0)
                    {
                        continue;
                    }
                    if (phase1PreMoves(maxPreMoves, -30, urfCubieCube[urfIdx], (int)(selfSym & 0xffff)) == 0)
                    {
                        return _solution == null ? "Error 8" : _solution;
                    }
                }
            }
            return _solution == null ? "Error 7" : _solution;
        }

        /**
		 * @return
		 *      0: Found or Probe limit exceeded
		 *      1: at least 1 + maxDep2 moves away, Try next power
		 *      2: at least 2 + maxDep2 moves away, Try next axis
		 */
        protected virtual int initPhase2Pre()
        {
            isRec = false;
            if (probe >= (_solution == null ? probeMax : probeMin))
            {
                return 0;
            }
            ++probe;

            for (int i = valid1; i < depth1; i++)
            {
                CubieCube.CornMult(phase1Cubie[i], CubieCube.moveCube[move[i]], phase1Cubie[i + 1]);
                CubieCube.EdgeMult(phase1Cubie[i], CubieCube.moveCube[move[i]], phase1Cubie[i + 1]);
            }
            valid1 = depth1;

            int p2corn = phase1Cubie[depth1].getCPermSym();
            int p2csym = p2corn & 0xf;
            p2corn >>= 4;
            int p2edge = phase1Cubie[depth1].getEPermSym();
            int p2esym = p2edge & 0xf;
            p2edge >>= 4;
            int p2mid = phase1Cubie[depth1].getMPerm();
            int edgei = CubieCube.getPermSymInv(p2edge, p2esym, false);
            int corni = CubieCube.getPermSymInv(p2corn, p2csym, true);

            int lastMove = depth1 == 0 ? -1 : move[depth1 - 1];
            int lastPre = preMoveLen == 0 ? -1 : preMoves[preMoveLen - 1];

            long ret = 0;
            int p2switchMax = (preMoveLen == 0 ? 1 : 2) * (depth1 == 0 ? 1 : 2);
            for (int p2switch = 0, p2switchMask = (1 << p2switchMax) - 1;
                    p2switch < p2switchMax; p2switch++)
            {
                // 0 normal; 1 lastmove; 2 lastmove + premove; 3 premove
                if ((p2switchMask >> p2switch & 1) != 0)
                {
                    p2switchMask &= ~(1 << p2switch);
                    ret = initPhase2(p2corn, p2csym, p2edge, p2esym, p2mid, edgei, corni);
                    if (ret == 0 || ret > 2)
                    {
                        break;
                    }
                    else if (ret == 2)
                    {
                        p2switchMask &= 0x4 << p2switch; // 0->2; 1=>3; 2=>N/A
                    }
                }
                if (p2switchMask == 0)
                {
                    break;
                }
                if ((p2switch & 1) == 0 && depth1 > 0)
                {
                    int m = Util.std2ud[lastMove / 3 * 3 + 1];
                    move[depth1 - 1] = Util.ud2std[m] * 2 - move[depth1 - 1];

                    p2mid = CoordCube.MPermMove[p2mid, m];
                    p2corn = CoordCube.CPermMove[p2corn, CubieCube.SymMoveUD[p2csym, m]];
                    p2csym = CubieCube.SymMult[p2corn & 0xf, p2csym];
                    p2corn >>= 4;
                    p2edge = CoordCube.EPermMove[p2edge, CubieCube.SymMoveUD[p2esym, m]];
                    p2esym = CubieCube.SymMult[p2edge & 0xf, p2esym];
                    p2edge >>= 4;
                    corni = CubieCube.getPermSymInv(p2corn, p2csym, true);
                    edgei = CubieCube.getPermSymInv(p2edge, p2esym, false);
                }
                else if (preMoveLen > 0)
                {
                    int m = Util.std2ud[lastPre / 3 * 3 + 1];
                    preMoves[preMoveLen - 1] = Util.ud2std[m] * 2 - preMoves[preMoveLen - 1];

                    p2mid = CubieCube.MPermInv[CoordCube.MPermMove[CubieCube.MPermInv[p2mid], m]];
                    p2corn = CoordCube.CPermMove[corni >> 4, CubieCube.SymMoveUD[corni & 0xf, m]];
                    corni = p2corn & ~0xf | CubieCube.SymMult[p2corn & 0xf, corni & 0xf];
                    p2corn = CubieCube.getPermSymInv(corni >> 4, corni & 0xf, true);
                    p2csym = p2corn & 0xf;
                    p2corn >>= 4;
                    p2edge = CoordCube.EPermMove[edgei >> 4, CubieCube.SymMoveUD[edgei & 0xf, m]];
                    edgei = p2edge & ~0xf | CubieCube.SymMult[p2edge & 0xf, edgei & 0xf];
                    p2edge = CubieCube.getPermSymInv(edgei >> 4, edgei & 0xf, false);
                    p2esym = p2edge & 0xf;
                    p2edge >>= 4;
                }
            }
            if (depth1 > 0)
            {
                move[depth1 - 1] = lastMove;
            }
            if (preMoveLen > 0)
            {
                preMoves[preMoveLen - 1] = lastPre;
            }
            return ret == 0 ? 0 : 2;
        }

        protected long initPhase2(int p2corn, int p2csym, int p2edge, int p2esym, int p2mid, int edgei, int corni)
        {
            long prun = Math.Max(
                           CoordCube.getPruning(CoordCube.EPermCCombPPrun,
                                                (edgei >> 4) * CoordCube.N_COMB + CoordCube.CCombPConj[CubieCube.Perm2CombP[corni >> 4] & 0xff, CubieCube.SymMultInv[edgei & 0xf, corni & 0xf]]),
                           Math.Max(
                               CoordCube.getPruning(CoordCube.EPermCCombPPrun,
                                                    p2edge * CoordCube.N_COMB + CoordCube.CCombPConj[CubieCube.Perm2CombP[p2corn] & 0xff, CubieCube.SymMultInv[p2esym, p2csym]]),
                               CoordCube.getPruning(CoordCube.MCPermPrun,
                                                    p2corn * CoordCube.N_MPERM + CoordCube.MPermConj[p2mid, p2csym])));

            if (prun > maxDep2)
            {
                return prun - maxDep2;
            }

            long depth2;
            for (depth2 = maxDep2; depth2 >= prun; depth2--)
            {
                long ret = phase2(p2edge, p2esym, p2corn, p2csym, p2mid, depth2, depth1, 10);
                if (ret < 0)
                {
                    break;
                }
                depth2 -= ret;
                sol = 0;
                for (int i = 0; i < depth1 + depth2; i++)
                {
                    appendSolMove(move[i]);
                }
                for (int i = preMoveLen - 1; i >= 0; i--)
                {
                    appendSolMove(preMoves[i]);
                }
                _solution = solutionToString();
            }

            if (depth2 != maxDep2)
            { //At least one solution has been found.
                maxDep2 = Math.Min(MAX_DEPTH2, sol - length1 - 1);
                return probe >= probeMin ? 0 : 1;
            }
            return 1;
        }

        /**
		 * @return
		 *      0: Found or Probe limit exceeded
		 *      1: Try Next Power
		 *      2: Try Next Axis
		 */
        protected virtual int phase1(CoordCube node, int ssym, int maxl, int lm)
        {
            if (node.prun == 0 && maxl < 5)
            {
                if (allowShorter || maxl == 0)
                {
                    depth1 -= maxl;
                    int ret = initPhase2Pre();
                    depth1 += maxl;
                    return ret;
                }
                else
                {
                    return 1;
                }
            }

            int skipMoves = CubieCube.getSkipMoves(ssym);

            for (int axis = 0; axis < 18; axis += 3)
            {
                if (axis == lm || axis == lm - 9)
                {
                    continue;
                }
                for (int power = 0; power < 3; power++)
                {
                    int m = axis + power;

                    if (isRec && m != move[depth1 - maxl]
                            || skipMoves != 0 && (skipMoves & 1 << m) != 0)
                    {
                        continue;
                    }

                    long prun = nodeUD[maxl].doMovePrun(node, m, true);
                    if (prun > maxl)
                    {
                        break;
                    }
                    else if (prun == maxl)
                    {
                        continue;
                    }

                    if (USE_CONJ_PRUN)
                    {
                        prun = nodeUD[maxl].doMovePrunConj(node, m);
                        if (prun > maxl)
                        {
                            break;
                        }
                        else if (prun == maxl)
                        {
                            continue;
                        }
                    }

                    move[depth1 - maxl] = m;
                    valid1 = Math.Min(valid1, depth1 - maxl);
                    int ret = phase1(nodeUD[maxl], ssym & (int)CubieCube.moveCubeSym[m], maxl - 1, axis);
                    if (ret == 0)
                    {
                        return 0;
                    }
                    else if (ret >= 2)
                    {
                        break;
                    }
                }
            }
            return 1;
        }

        protected string searchopt()
        {
            long maxprun1 = 0;
            long maxprun2 = 0;
            for (int i = 0; i < 6; i++)
            {
                urfCoordCube[i].calcPruning(false);
                if (i < 3)
                {
                    maxprun1 = Math.Max(maxprun1, urfCoordCube[i].prun);
                }
                else
                {
                    maxprun2 = Math.Max(maxprun2, urfCoordCube[i].prun);
                }
            }
            urfIdx = maxprun2 > maxprun1 ? 3 : 0;
            phase1Cubie[0] = urfCubieCube[urfIdx];
            for (length1 = isRec ? length1 : 0; length1 < sol; length1++)
            {
                CoordCube ud = urfCoordCube[0 + urfIdx];
                CoordCube rl = urfCoordCube[1 + urfIdx];
                CoordCube fb = urfCoordCube[2 + urfIdx];

                if (ud.prun <= length1 && rl.prun <= length1 && fb.prun <= length1
                        && phase1opt(ud, rl, fb, selfSym, length1, -1) == 0)
                {
                    return _solution == null ? "Error 8" : _solution;
                }
            }
            return _solution == null ? "Error 7" : _solution;
        }

        /**
		 * @return
		 *      0: Found or Probe limit exceeded
		 *      1: Try Next Power
		 *      2: Try Next Axis
		 */
        protected int phase1opt(CoordCube ud, CoordCube rl, CoordCube fb, long ssym, int maxl, int lm)
        {
            if (ud.prun == 0 && rl.prun == 0 && fb.prun == 0 && maxl < 5)
            {
                maxDep2 = maxl;
                depth1 = length1 - maxl;
                return initPhase2Pre() == 0 ? 0 : 1;
            }

            int skipMoves = CubieCube.getSkipMoves(ssym);

            for (int axis = 0; axis < 18; axis += 3)
            {
                if (axis == lm || axis == lm - 9)
                {
                    continue;
                }
                for (int power = 0; power < 3; power++)
                {
                    int m = axis + power;

                    if (isRec && m != move[length1 - maxl]
                            || skipMoves != 0 && (skipMoves & 1 << m) != 0)
                    {
                        continue;
                    }

                    // UD Axis
                    long prun_ud = Math.Max(nodeUD[maxl].doMovePrun(ud, m, false),
                                           USE_CONJ_PRUN ? nodeUD[maxl].doMovePrunConj(ud, m) : 0);
                    if (prun_ud > maxl)
                    {
                        break;
                    }
                    else if (prun_ud == maxl)
                    {
                        continue;
                    }

                    // RL Axis
                    m = CubieCube.urfMove[2, m];

                    long prun_rl = Math.Max(nodeRL[maxl].doMovePrun(rl, m, false),
                                           USE_CONJ_PRUN ? nodeRL[maxl].doMovePrunConj(rl, m) : 0);
                    if (prun_rl > maxl)
                    {
                        break;
                    }
                    else if (prun_rl == maxl)
                    {
                        continue;
                    }

                    // FB Axis
                    m = CubieCube.urfMove[2, m];

                    long prun_fb = Math.Max(nodeFB[maxl].doMovePrun(fb, m, false),
                                           USE_CONJ_PRUN ? nodeFB[maxl].doMovePrunConj(fb, m) : 0);
                    if (prun_ud == prun_rl && prun_rl == prun_fb && prun_fb != 0)
                    {
                        prun_fb++;
                    }

                    if (prun_fb > maxl)
                    {
                        break;
                    }
                    else if (prun_fb == maxl)
                    {
                        continue;
                    }

                    m = CubieCube.urfMove[2, m];

                    move[length1 - maxl] = m;
                    valid1 = Math.Min(valid1, length1 - maxl);
                    int ret = phase1opt(nodeUD[maxl], nodeRL[maxl], nodeFB[maxl], ssym & CubieCube.moveCubeSym[m], maxl - 1, axis);
                    if (ret == 0)
                    {
                        return 0;
                    }
                }
            }
            return 1;
        }

        void appendSolMove(int curMove)
        {
            if (sol == 0)
            {
                moveSol[sol++] = curMove;
                return;
            }
            int axisCur = curMove / 3;
            int axisLast = moveSol[sol - 1] / 3;
            if (axisCur == axisLast)
            {
                int pow = (curMove % 3 + moveSol[sol - 1] % 3 + 1) % 4;
                if (pow == 3)
                {
                    sol--;
                }
                else
                {
                    moveSol[sol - 1] = axisCur * 3 + pow;
                }
                return;
            }
            if (sol > 1
                    && axisCur % 3 == axisLast % 3
                    && axisCur == moveSol[sol - 2] / 3)
            {
                int pow = (curMove % 3 + moveSol[sol - 2] % 3 + 1) % 4;
                if (pow == 3)
                {
                    moveSol[sol - 2] = moveSol[sol - 1];
                    sol--;
                }
                else
                {
                    moveSol[sol - 2] = axisCur * 3 + pow;
                }
                return;
            }
            moveSol[sol++] = curMove;
        }

        //-1: no solution found
        // X: solution with X moves shorter than expectation. Hence, the length of the solution is  depth - X
        protected virtual long phase2(int edge, int esym, int corn, int csym, int mid, long maxl, int depth, int lm)
        {
            if (edge == 0 && corn == 0 && mid == 0)
            {
                return maxl;
            }
            int moveMask = Util.ckmv2bit[lm];
            for (int m = 0; m < 10; m++)
            {
                if ((moveMask >> m & 1) != 0)
                {
                    m += 0x42 >> m & 3;
                    continue;
                }
                int midx = CoordCube.MPermMove[mid, m];
                int cornx = CoordCube.CPermMove[corn, CubieCube.SymMoveUD[csym, m]];
                int csymx = CubieCube.SymMult[cornx & 0xf, csym];
                cornx >>= 4;
                int edgex = CoordCube.EPermMove[edge, CubieCube.SymMoveUD[esym, m]];
                int esymx = CubieCube.SymMult[edgex & 0xf, esym];
                edgex >>= 4;
                int edgei = CubieCube.getPermSymInv(edgex, esymx, false);
                int corni = CubieCube.getPermSymInv(cornx, csymx, true);

                long prun = CoordCube.getPruning(CoordCube.EPermCCombPPrun,
                                                (edgei >> 4) * CoordCube.N_COMB + CoordCube.CCombPConj[CubieCube.Perm2CombP[corni >> 4] & 0xff, CubieCube.SymMultInv[edgei & 0xf, corni & 0xf]]);
                if (prun > maxl + 1)
                {
                    return maxl - prun + 1;
                }
                else if (prun >= maxl)
                {
                    m += 0x42 >> m & 3 & (int)(maxl - prun);
                    continue;
                }
                prun = Math.Max(
                           CoordCube.getPruning(CoordCube.MCPermPrun,
                                                cornx * CoordCube.N_MPERM + CoordCube.MPermConj[midx, csymx]),
                           CoordCube.getPruning(CoordCube.EPermCCombPPrun,
                                                edgex * CoordCube.N_COMB + CoordCube.CCombPConj[CubieCube.Perm2CombP[cornx] & 0xff, CubieCube.SymMultInv[esymx, csymx]]));
                if (prun >= maxl)
                {
                    m += 0x42 >> m & 3 & (int)(maxl - prun);
                    continue;
                }
                long ret = phase2(edgex, esymx, cornx, csymx, midx, maxl - 1, depth + 1, m);
                if (ret >= 0)
                {
                    move[depth] = Util.ud2std[m];
                    return ret;
                }
            }
            return -1;
        }

        protected virtual string solutionToString()
        {
            StringBuilder sb = new StringBuilder();
            int urf = (verbose & INVERSE_SOLUTION) != 0 ? (urfIdx + 3) % 6 : urfIdx;
            if (urf < 3)
            {
                for (int s = 0; s < sol; s++)
                {
                    if ((verbose & USE_SEPARATOR) != 0 && s == depth1)
                    {
                        sb.Append(".  ");
                    }
                    sb.Append(Util.move2str[CubieCube.urfMove[urf, moveSol[s]]]).Append(' ');
                }
            }
            else
            {
                for (int s = sol - 1; s >= 0; s--)
                {
                    sb.Append(Util.move2str[CubieCube.urfMove[urf, moveSol[s]]]).Append(' ');
                    if ((verbose & USE_SEPARATOR) != 0 && s == depth1)
                    {
                        sb.Append(".  ");
                    }
                }
            }
            if ((verbose & APPEND_LENGTH) != 0)
            {
                sb.Append("(").Append(sol).Append("f)");
            }
            return sb.ToString();
        }
    }
}