namespace Identification
{
    using System;

    public class Face
    {
        public Face(Guid faceId, Rect rect, Guid? personId)
        {
            FaceId = faceId;
            Rect = rect;
            PersonId = personId;
        }

        public Guid FaceId { get; set; }
        public Rect Rect { get; set; }
        public Guid? PersonId { get; set; }
    }
}