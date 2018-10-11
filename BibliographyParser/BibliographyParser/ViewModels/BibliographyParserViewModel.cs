using BibliographyParser.Internals;
using PropertyChanged;
using System;
using System.Windows.Input;

namespace BibliographyParser.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class BibliographyParserViewModel
    {
        public string InpputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = $@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\Bibliography.xsl";
        public string ResultText { get; set; } = ":*";

        public ICommand BrowseInputCommand { get; set; } = new RelayCommand(BrowseInput);   
        public ICommand BrowseOutputCommand { get; set; } = new RelayCommand(BrowseOutput);
        public ICommand StartCommand { get; set; } = new RelayCommand(Start);
      
        private static void BrowseInput()
        {
            throw new NotImplementedException();
        }

        private static void BrowseOutput()
        {
            throw new NotImplementedException();
        }

        private static void Start()
        {
            throw new NotImplementedException();
        }
    }
}
