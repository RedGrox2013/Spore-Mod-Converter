using System.ComponentModel;
using System.Windows.Input;
using System.IO;
using System;
using System.Windows;
using Microsoft.Win32;

namespace Spore_Mod_Converter
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const string OUTPUT_DIRECTORY = "Converted packages";

        private string _packagePath;
        public string PackagePath
        {
            get => _packagePath;
            set
            {
                _packagePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackagePath)));
            }
        }

        private bool _enable = true;
        public bool Enable
        {
            get => _enable;
            private set
            {
                _enable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Enable)));
            }
        }

        public ICommand ConvertCommand { get; private set; }
        public ICommand OpenOutputDirectoryCommand { get; private set; }
        public ICommand BrowseFileCommand { get; private set; }

        public MainWindowViewModel()
        {
            ConvertCommand = new RelayCommand(Convert);
            OpenOutputDirectoryCommand = new RelayCommand(OpenOutputDirectory);
            BrowseFileCommand = new RelayCommand(Browse);
        }

        private static void CheckOutput()
        {
            if (!Directory.Exists(OUTPUT_DIRECTORY))
                Directory.CreateDirectory(OUTPUT_DIRECTORY);
        }

        private async void Convert(object parameter)
        {
            Enable = false;
            CheckOutput();

            try
            {
                string path = PackagePath.Trim('"');
                string outPath = Path.Combine(OUTPUT_DIRECTORY, Path.GetFileName(path));

                PackageConverter converter = new PackageConverter(path);
                await converter.ToPrototype2008PackageAsync(outPath);

                MessageBox.Show($"Converted file:\n\"{outPath}\"", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Enable = true;
            }
        }

        private void OpenOutputDirectory(object parameter)
        {
            CheckOutput();
            System.Diagnostics.Process.Start("explorer", OUTPUT_DIRECTORY);
        }

        private void Browse(object parameter)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Spore package (*.package)|*.package|All files (*.*)|*.*"
            };
            if (dialog.ShowDialog() == true)
                PackagePath = dialog.FileName;
        }
    }
}
