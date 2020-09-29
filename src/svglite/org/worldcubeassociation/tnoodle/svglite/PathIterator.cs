using System;
using System.Collections.Generic;
using System.Text;

using static org.worldcubeassociation.tnoodle.svglite.Utils;

namespace org.worldcubeassociation.tnoodle.svglite
{
    public class PathIterator
    {
        public const int SEG_MOVETO = 0;
        public const int SEG_LINETO = 1;
        public const int SEG_CLOSE = 4;
        public const string SVG_LANGUAGE_COMMANDS = "MLTCZ";

        private int index;
            private List<Path.Command> commands;
            public PathIterator(Path p)
            {
                index = 0;
                commands = p.commands;
            }

            public bool isDone()
            {
                return index >= commands.Count;
            }

            public void next()
            {
                index++;
            }

            public int currentSegment(double[] coords)
            {
                Path.Command command = commands[index];
                azzert(coords.Length >= command.coords.Length);
                for (int i = 0; i < command.coords.Length; i++)
                {
                    coords[i] = command.coords[i];
                }
                return command.type;
            }
        }
}
