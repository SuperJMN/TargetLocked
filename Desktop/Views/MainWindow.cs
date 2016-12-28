using System.IO;

namespace Desktop.Views
{
    using Avalonia.Controls;
    using Identification;
    using Microsoft.ProjectOxford.Face;
    using OmniXaml.Avalonia;
    using ViewModels;

    internal class MainWindow : Window
    {
        public MainWindow()
        {
            XamlService.Current.Load(this);

            var apiKey = File.ReadAllText("ApiKey.txt");
            DataContext = new MainViewModel(new BatchIdentificator(new FaceServiceClient(apiKey)));            
        }
    }
}