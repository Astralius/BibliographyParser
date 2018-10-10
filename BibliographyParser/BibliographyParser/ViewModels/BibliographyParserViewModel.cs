using PropertyChanged;
using System.Threading;

namespace BibliographyParser.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class BibliographyParserViewModel
    {
        public string TestString { get; set; } = string.Empty;

        public BibliographyParserViewModel()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.Sleep(5000);
                TestString = "Hello bindings!";
            }).Start();
        }
    }
}
