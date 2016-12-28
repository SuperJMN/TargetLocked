namespace Desktop.ViewModels
{
    using System;

    public class VideoMetadata
    {
        public float Fps { get; set; }
        public TimeSpan Duration { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public long TotalFrames { get; set; }
    }
}