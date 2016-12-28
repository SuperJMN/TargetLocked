namespace Identification
{
    using Microsoft.ProjectOxford.Face.Contract;

    public static class ModelExtensions
    {
        public static Rect ToRect(this FaceRectangle rectangle)
        {
            return new Rect(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
        }
    }
}