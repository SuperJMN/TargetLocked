using System.IO;
using System.Reflection;

namespace Desktop
{
    using System;
    using Avalonia;
    using Avalonia.DotNetFrameworkRuntime;
    using OmniXaml.Avalonia;
    using Views;

    internal class App : Application
    {
        public override void Initialize()
        {
            UriParser.Register(new ResourceManagerUriParser(), "resm", 0);
            XamlService.Current.Load(this);
        }

        private static void Main()
        {
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .Start<MainWindow>();            
        }
    }
}