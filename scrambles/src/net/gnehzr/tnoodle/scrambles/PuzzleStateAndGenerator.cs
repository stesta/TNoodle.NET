using static net.gnehzr.tnoodle.scrambles.Puzzle;

namespace net.gnehzr.tnoodle.scrambles
{
    public class PuzzleStateAndGenerator
    {
        public PuzzleStateAndGenerator(PuzzleState state, string generator)
        {
            State = state;
            Generator = generator;
        }

        public PuzzleState State { get; }
        public string Generator { get; }
    }
}