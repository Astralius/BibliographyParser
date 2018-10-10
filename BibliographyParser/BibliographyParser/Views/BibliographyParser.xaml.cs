using BibliographyParser.ViewModels;
using System.Windows;

namespace BibliographyParser.Views
{
    /// <summary>
    /// Interaction logic for BibliographyParser.xaml
    /// </summary>
    public partial class BibliographyParser : Window
    {
        public BibliographyParser()
        {
            var viewModel = new BibliographyParserViewModel();
            this.DataContext = viewModel;
            InitializeComponent();
        }
    }
}
