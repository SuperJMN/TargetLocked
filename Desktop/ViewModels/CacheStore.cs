namespace Desktop.ViewModels
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Caching;

    public static class CacheStore
    {
        public static FileCache GetCache(Uri uri)
        {
            var rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var folder = Path.GetFileNameWithoutExtension(uri.OriginalString.Replace(" ", ""));

            var cache = new FileCache(Path.Combine(rootFolder, folder));
            return cache;
        }
    }
}