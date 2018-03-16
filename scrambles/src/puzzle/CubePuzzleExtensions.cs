using System;
using System.Collections.Generic;
using System.Text;

using static puzzle.CubePuzzle;

namespace puzzle
{
    public static class CubePuzzleExtensions
    {
        public static Face OppositeFace(this Face f)
        {
            return (Face)(((int)f + 3) % 6);
        }
    }
}
