using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using TextRecognition.FileTasks;
using TextRecognition.ScreenCapture;

namespace TextRecognition
{
    internal class MainViewModel : BindableBase
    {
        private string _selectedCulture;
        private bool _openCreatedFiles = true;

        public MainViewModel()
        {
            RunningTasks = new ObservableCollection<IOcrTask>();
            AvailableCultures = new ObservableCollection<string>();
            FillCultures();

            BrowseFileCommand = new DelegateCommand(BrowseFile);
            TakeScreenShotCommand = new DelegateCommand(TakeScreenshot);
            OpenFileCommand = new DelegateCommand<IOcrTask>(OpenFile, CanOpenOrSaveFile);
            SaveFileCommand = new DelegateCommand<IOcrTask>(SaveFile, CanOpenOrSaveFile);
        }

        private void FillCultures()
        {
            var langs = TesseractOcr.AvailableLanguages();
            foreach (var lang in langs)
            {
                AvailableCultures.Add(lang);
            }
        }

        public bool OpenCreatedFiles
        {
            get => _openCreatedFiles;
            set => SetProperty(ref _openCreatedFiles, value);
        }

        public DelegateCommand TakeScreenShotCommand { get; }

        public DelegateCommand<IOcrTask> OpenFileCommand { get; }

        public DelegateCommand<IOcrTask> SaveFileCommand { get; set; }

        public DelegateCommand BrowseFileCommand { get; }

        public ObservableCollection<IOcrTask> RunningTasks { get; }

        public ObservableCollection<string> AvailableCultures { get; }

        public string SelectedCulture
        {
            get => _selectedCulture;
            set => SetProperty(ref _selectedCulture, value);
        }

        private async void TakeScreenshot()
        {
            var screenSource = new ScreenSource();
            var success = screenSource.TryCapture(out var image);
            if (!success)
            {
                return;
            }
            
            var tempPath = Path.Combine(Path.GetTempPath(), $"Screenshot-{DateTime.Now:yyyy-dd-M HH-mm-ss}.png");
            using (var temp = new TempFile(tempPath))
            {
                image.Save(tempPath);
                var task = AddFile(tempPath);

                try
                {
                    await task.Completion;
                }
                catch { }
            }
        }


        private void BrowseFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.tiff)|*.png;*.jpg;*.jpeg;*.tiff";
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                AddFile(openFileDialog.FileName);
            }
        }

        public IOcrTask AddFile(string path)
        {
            var fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
            {
                Debug.WriteLine("File does not exist");
            }

            var ocrTask = CreateTask(fileInfo);
            RunningTasks.Add(ocrTask);
            ocrTask.Start();
            ocrTask.Completion.ContinueWith(task => { ReEvaluateCommands(); });
            return ocrTask;
        }

        private void ReEvaluateCommands()
        {
            OpenFileCommand.RaiseCanExecuteChanged();
            SaveFileCommand.RaiseCanExecuteChanged();
        }

        private IOcrTask CreateTask(FileInfo fileInfo)
        {
            if (ImageOcrTask.SupportsExtension(fileInfo.Extension))
            {
                return new ImageOcrTask(fileInfo, OpenCreatedFiles, SelectedCulture);
            }

            return new UnsupportedFileTask(fileInfo);
        }


        private void OpenFile(IOcrTask task)
        {
            if (!CanOpenOrSaveFile(task))
            {
                return;
            }

            task.OpenResultFile();
        }

        private void SaveFile(IOcrTask task)
        {
            if (!CanOpenOrSaveFile(task))
            {
                return;
            }

            var dialog = new SaveFileDialog();
            dialog.FileName = GetPdfFileName(task.Name);
            dialog.Filter = "Searchable PDF file (*.pdf)|*.pdf";
            if (dialog.ShowDialog() == true)
            {
                File.Copy(task.ResultFilePath, dialog.FileName);
            }
        }

        private static string GetPdfFileName(string originalFileName)
        {
            var extensionStart = originalFileName.LastIndexOf('.');
            var cleanFileName = originalFileName.Substring(0, extensionStart);
            return cleanFileName + ".pdf";
        }

        private bool CanOpenOrSaveFile(IOcrTask task)
        {
            if (task == null)
            {
                return false;
            }

            return task.CanOpenResultFile();
        }
    }
}