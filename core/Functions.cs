using System;
using System.Collections.Generic;
using System.Text;

namespace net.gnehzr.tnoodle
{
    public static class Functions
    {
        public static int BitCount(int value)
        {
            var v = (uint)value;
            uint c;

            c = v - ((v >> 1) & 0x55555555);
            c = ((c >> 2) & 0x33333333) + (c & 0x33333333);
            c = ((c >> 4) + c) & 0x0F0F0F0F;
            c = ((c >> 8) + c) & 0x00FF00FF;
            c = ((c >> 16) + c) & 0x0000FFFF;

            return (int)c;
        }

        //public static Face OppositeFace(this Face f)
        //{
        //    return (Face)(((int)f + 3) % 6);
        //}

        //// TODO We could rename faces so we can just do +6 mod 12 here instead.
        //public static MegaminxPuzzle.Face OppositeFace(this MegaminxPuzzle.Face face)
        //{
        //    switch (face)
        //    {
        //        case MegaminxPuzzle.Face.U:
        //            return MegaminxPuzzle.Face.D;
        //        case MegaminxPuzzle.Face.Bl:
        //            return MegaminxPuzzle.Face.Dr;
        //        case MegaminxPuzzle.Face.Br:
        //            return MegaminxPuzzle.Face.Dl;
        //        case MegaminxPuzzle.Face.R:
        //            return MegaminxPuzzle.Face.Dbl;
        //        case MegaminxPuzzle.Face.F:
        //            return MegaminxPuzzle.Face.B;
        //        case MegaminxPuzzle.Face.L:
        //            return MegaminxPuzzle.Face.Dbr;
        //        case MegaminxPuzzle.Face.D:
        //            return MegaminxPuzzle.Face.U;
        //        case MegaminxPuzzle.Face.Dr:
        //            return MegaminxPuzzle.Face.Bl;
        //        case MegaminxPuzzle.Face.Dbr:
        //            return MegaminxPuzzle.Face.L;
        //        case MegaminxPuzzle.Face.B:
        //            return MegaminxPuzzle.Face.F;
        //        case MegaminxPuzzle.Face.Dbl:
        //            return MegaminxPuzzle.Face.R;
        //        case MegaminxPuzzle.Face.Dl:
        //            return MegaminxPuzzle.Face.Br;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(face), face, null);
        //    }
        //}
    }
}
