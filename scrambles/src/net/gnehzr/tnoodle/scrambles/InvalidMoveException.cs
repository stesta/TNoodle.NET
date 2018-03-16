using System;

namespace net.gnehzr.tnoodle.scrambles
{
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException(string move) 
            : base("Invalid move: " + move)
        {
        }
    }
}