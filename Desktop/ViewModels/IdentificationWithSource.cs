namespace Desktop.ViewModels
{
    using Identification;

    public class IdentificationWithSource
    {
        public IdentificationWithSource(string source, Identification identification)
        {
            Source = source;
            Identification = identification;
        }

        public Identification Identification { get; set; }
        public string Source { get; set; }
    }
}