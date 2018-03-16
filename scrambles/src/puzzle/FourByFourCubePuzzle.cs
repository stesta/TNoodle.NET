using System;

using net.gnehzr.tnoodle;
using net.gnehzr.tnoodle.scrambles;
using static net.gnehzr.tnoodle.utils.GwtSafeUtils;
using static net.gnehzr.tnoodle.scrambles.AlgorithmBuilder;
using cs.threephase;

namespace puzzle
{
    public class FourByFourCubePuzzle : CubePuzzle
    {
        private readonly Search _threePhaseSearcher = new Search();

        public FourByFourCubePuzzle() : base(4)
        {
        }

        public override double GetInitializationStatus() => Edge3.InitStatus();

        public override PuzzleStateAndGenerator GenerateRandomMoves(Random r)
        {
            var scramble = _threePhaseSearcher.RandomState(r);
            var ab = new AlgorithmBuilder(MergingMode.CanonicalizeMoves, GetSolvedState());
            try
            {
                ab.AppendAlgorithm(scramble);
            }
            catch (InvalidMoveException e)
            {
                azzert(false, e.Message); //, new InvalidScrambleException(scramble, e));
            }
            return ab.GetStateAndGenerator();
        }
    }
}