using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace org.worldcubeassociation.tnoodle.svglite
{
    public class InvalidHexColorException : Exception
    {
        public InvalidHexColorException()
            : base()
        {
        }

        public InvalidHexColorException(string message)
            : base(message)
        {
        }

        public InvalidHexColorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidHexColorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
