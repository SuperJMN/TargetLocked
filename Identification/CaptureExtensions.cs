namespace Identification
{
    using System.Collections.Generic;
    using System.Linq;
    using DotImaging;

    public static class CaptureExtensions
    {
        public static IEnumerable<IndexedImage> ExtractIndexedImages(this IEnumerable<IImage> enumerable, CaptureOptions options)
        {
            return enumerable.Skip(options.Skip)
                .WhereIndex(i => i % options.CaptureEvery == 0)
                .Select((im, i) => new IndexedImage { Image = im, Index = i })
                .Take(options.Take);
        }
    }
}