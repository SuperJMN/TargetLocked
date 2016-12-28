namespace Identification
{
    using DotImaging;

    internal class IdentificationRequest
    {
        public byte[] Image { get; set; }
        public int Index { get; set; }

        public IdentificationRequest(byte[] image, int i)
        {
            Image = image;
            Index = i;
        }
    }
}