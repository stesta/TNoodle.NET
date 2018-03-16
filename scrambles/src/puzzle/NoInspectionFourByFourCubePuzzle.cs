using System;

using net.gnehzr.tnoodle;
using net.gnehzr.tnoodle.scrambles;
using static net.gnehzr.tnoodle.utils.GwtSafeUtils;
using static net.gnehzr.tnoodle.scrambles.AlgorithmBuilder;

namespace puzzle
{
    public class NoInspectionFourByFourCubePuzzle : FourByFourCubePuzzle
    {
        public override PuzzleStateAndGenerator GenerateRandomMoves(Random r)
        {
            var randomOrientationMoves = GetRandomOrientationMoves(Size - 1);
            var randomOrientation = randomOrientationMoves[r.Next(randomOrientationMoves.Length)];
            var psag = base.GenerateRandomMoves(r);
            psag = ApplyOrientation(this, randomOrientation, psag, true);
            return psag;
        }

        public static PuzzleStateAndGenerator ApplyOrientation(CubePuzzle puzzle, CubeMove[] randomOrientation,
            PuzzleStateAndGenerator psag, bool discardRedundantMoves)
        {
            if (randomOrientation.Length == 0)
                return psag;

            // Append reorientation to scramble.
            try
            {
                var ab = new AlgorithmBuilder(MergingMode.NoMerging, puzzle.GetSolvedState());
                ab.AppendAlgorithm(psag.Generator);
                foreach (var cm in randomOrientation)
                    ab.AppendMove(cm.ToString());

                psag = ab.GetStateAndGenerator();
                return psag;
            }
            catch (InvalidMoveException e)
            {
                azzert(false, e.Message);
                return null;
            }
        }

        public override string GetShortName()
        {
            return "444ni";
        }

        public override string GetLongName()
        {
            return "4x4x4 no inspection";
        }
    }
}