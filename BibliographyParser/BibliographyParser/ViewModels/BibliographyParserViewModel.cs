using BibliographyParser.Internals;
using Microsoft.Win32;
using MSOfficeBibliographySerializer;
using MSOfficeBibliographySerializer.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace BibliographyParser.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class BibliographyParserViewModel
    {
        public BibliographyParserViewModel()
        {
            this.BrowseInputCommand = new RelayCommand(BrowseInput);
            this.BrowseOutputCommand = new RelayCommand(BrowseOutput);
            this.StartCommand = new RelayCommand(Start);
        }

        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string ResultText { get; set; } = ":*";

        public ICommand BrowseInputCommand { get; set; }
        public ICommand BrowseOutputCommand { get; set; }
        public ICommand StartCommand { get; set; } 
      
        private void BrowseInput()
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Wybierz plik do przerobienia",
                Filter = "txt files (*.txt)|*.txt",
                DefaultExt = ".txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                ShowReadOnly = false,
                Multiselect = false,
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                ValidateNames = true
            };

            if (dialog.ShowDialog() == true)
            {
                InputPath = dialog.FileName;
            }
        }

        private void BrowseOutput()
        {
            var officeBibliographyFilesPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Microsoft",
                    "Bibliography");

            var dialog = new SaveFileDialog()
            {
                Title = "Gdzie ma trafić przerobiona bibliografia?",
                Filter = "xml files (*.xml)|*.xml",
                DefaultExt = ".xml",
                FileName = "Sources",
                InitialDirectory = officeBibliographyFilesPath,
                AddExtension = true,
                ValidateNames = true,
                CreatePrompt = true,
                OverwritePrompt = true
            };

            if (dialog.ShowDialog() == true)
            {
                OutputPath = dialog.FileName;
            }
        }

        private void Start()
        {
            if (PathsSelected())
            {
                ResultText = string.Empty;
                IList<JournalArticle> sources = new List<JournalArticle>();

                // Convert from text to objects:
                try
                {
                    var parser = new JournalArticleParser();
                    sources = parser.ParseFromFile(InputPath);
                }
                catch (Exception)
                {
                    ResultText = "BŁĄD podczas zczytywania pliku .txt";
                    return;
                }

                // Convert from objects to MS Office bibliography XML file:
                try
                {
                    var XMLCreator = new XMLBibliographyCreator();
                    XMLCreator.Initialize(OutputPath);
                    foreach (Source source in sources)
                    {
                        XMLCreator.AddSource(source);
                    }
                    XMLCreator.ApplyChanges();
                }
                catch (Exception)
                {
                    ResultText = "BŁĄD podczas zapisywania pliku .xml";
                    return;
                }
            }
            else
            {
                ResultText = "BŁĄD! Najpierw wybierz plik wejściowy i wyjściowy.";
                return;
            }

            ResultText = "Plik przekonwertowany pomyślnie! :)";

            bool PathsSelected() => !string.IsNullOrEmpty(InputPath) && !string.IsNullOrEmpty(OutputPath);
        }
    }
}
