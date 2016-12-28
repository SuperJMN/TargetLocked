namespace Identification
{
    using System.Collections.Generic;

    public class Identification
    {
        public Identification(int ordinal, IEnumerable<Face> faces)
        {
            Ordinal = ordinal;
            Faces = faces;
        }

        public int Ordinal { get; set; }
        public IEnumerable<Face> Faces { get; set; }
    }
}