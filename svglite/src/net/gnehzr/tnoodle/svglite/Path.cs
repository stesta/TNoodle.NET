using System;
using System.Collections.Generic;
using System.Text;

using static net.gnehzr.tnoodle.svglite.Utils;

namespace net.gnehzr.tnoodle.svglite
{
    public class Path : Element
    {
        public class Command
        {
            public int type;
            public double[] coords;
            public Command(int type, double[] coords)
            {
                this.type = type;
                this.coords = coords;
            }
            public String toString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(PathIterator.SVG_LANGUAGE_COMMANDS[type]);
                for (int i = 0; coords != null && i < coords.Length; i++)
                {
                    sb.Append(" ");
                    sb.Append(coords[i]);
                }
                return sb.ToString();
            }
        }

        public List<Command> commands = null;

        public Path()
            : base("path")
        {
        }

        public Path(Path p)
            : base(p)
        {
            if (p.commands != null)
            {
                this.commands = new List<Command>(p.commands);
            }
        }

        public PathIterator getPathIterator()
        {
            return new PathIterator(this);
        }

        public void moveTo(double x, double y)
        {
            if (commands == null)
            {
                commands = new List<Command>();
            }

            int type = PathIterator.SEG_MOVETO;
            double[] coords = new double[] { x, y };
            commands.Add(new Command(type, coords));
        }

        private void azzertMoveTo()
        {
            azzert(commands != null, "First command must be moveTo");
        }

        public void lineTo(double x, double y)
        {
            azzertMoveTo();

            int type = PathIterator.SEG_LINETO;
            double[] coords = new double[] { x, y };
            commands.Add(new Command(type, coords));
        }

        public void closePath()
        {
            azzertMoveTo();

            int type = PathIterator.SEG_CLOSE;
            double[] coords = null;
            commands.Add(new Command(type, coords));
        }

        public override void translate(double x, double y)
        {
            foreach (Command c in commands)
            {
                switch (c.type)
                {
                    case PathIterator.SEG_MOVETO:
                    case PathIterator.SEG_LINETO:
                        c.coords[0] += x;
                        c.coords[1] += y;
                        break;
                    case PathIterator.SEG_CLOSE:
                        break;
                    default:
                        azzert(false);
                        break;
                }
            }
        }

        public String getD()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Command c in commands)
            {
                sb.Append(" " + c.toString());
            }
            if (sb.Length == 0)
            {
                return "";
            }
            return sb.ToString().Substring(1);
        }

        public override void buildString(StringBuilder sb, int level)
        {
            // We're about to get dumped to a string, lets update
            // our "d" attribute first.
            setAttribute("d", getD());
            base.buildString(sb, level);
        }
    }
}
