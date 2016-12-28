namespace Desktop.Views
{
    using Avalonia.Controls;
    using Avalonia.Diagnostics;
    using OmniXaml.Avalonia;

    internal class ReviewWindow : Window
    {
        public ReviewWindow()
        {
            XamlService.Current.Load(this);
            DevTools.Attach(this);
        }
    }
}