using System;
using System.IO;
using System.Linq;
using Avalonia.Markup;
using Avalonia.Media.Imaging;
using Desktop.ViewModels;

namespace Desktop.ViewSupport
{
    public static class Converters
    {
        public static readonly IValueConverter FromPathToBitmap = new FuncValueConverter<string, IBitmap>(
            s =>
            {
                if (s != null)
                {
                    using (var stream = new FileStream(s, FileMode.Open, FileAccess.Read))
                    {
                        var streamLength = stream.Length;
                        var array = new byte[streamLength];
                        stream.Read(array, 0, (int) streamLength);
                        return new Bitmap(new MemoryStream(array));
                    }
                }

                return null;
            });

        public static readonly IMultiValueConverter FromFrameIdToBitmap = new FuncMultiValueConverter<object, IBitmap>(
            s =>
            {
                var ar = s.ToArray();
                var frame = (int) ar[0];
                var path = (string) ar[1];

                var cache = CacheStore.GetCache(new Uri(path));
                var bytes = (byte[]) cache[frame.ToString()];

                using (var memoryStream = new MemoryStream(bytes))
                {
                    return new Bitmap(memoryStream);
                }
            });

        public static readonly IMultiValueConverter FromFrameToTimeSpan = new FuncMultiValueConverter<object, string>(
            s =>
            {
                var ar = s.ToArray();
                var frame = (int) ar[0];
                var fps = (float) ar[1];

                return TimeSpan.FromSeconds(frame/fps).ToString(@"hh\:mm\:ss");
            });
    }
}