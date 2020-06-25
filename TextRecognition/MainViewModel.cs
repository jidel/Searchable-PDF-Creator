using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
            
            var tempPath = Path.GetTempFileName() + ".png";
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
            return ocrTask;
        }

        public IOcrTask CreateTask(FileInfo fileInfo)
        {
            if (ImageOcrTask.SupportsExtension(fileInfo.Extension))
            {
                return new ImageOcrTask(fileInfo, OpenCreatedFiles, SelectedCulture);
            }

            return new UnsupportedFileTask(fileInfo);
        }
    }
}