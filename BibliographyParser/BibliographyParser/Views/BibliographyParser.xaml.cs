using BibliographyParser.ViewModels;
using System.Windows;
using System.Windows.Input;

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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
    }
}
