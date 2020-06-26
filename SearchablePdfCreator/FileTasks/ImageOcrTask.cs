using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SearchablePdfCreator.FileTasks
{
    internal class ImageOcrTask : BindableBase, IOcrTask, IDisposable
    {
        private FileInfo _fileInfo;
        private string _statusMessage;
        private string _resultFilePath;
        private bool _isProcessing;
        private OcrStatus _status;
        private bool _openCreatedFiles;
        private readonly string _culture;

        public ImageOcrTask(FileInfo fileInfo, bool openCreatedFiles, string culture)
        {
            _openCreatedFiles = openCreatedFiles;
            _culture = culture;
            _fileInfo = fileInfo;
            Status = OcrStatus.Created;

            Application.Current.Exit += OnExit;
        }

        public string FullPath => _fileInfo.FullName;

        public string Name => _fileInfo.Name;

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public OcrStatus Status { get => _status; private set => SetProperty(ref _status, value); }

        public bool IsProcessing { get => _isProcessing; private set => SetProperty(ref _isProcessing, value); }

        public string ResultFilePath { get => _resultFilePath; private set => SetProperty(ref _resultFilePath, value); }

        public Task Completion { get; private set; }

        public async void Start()
        {

            IsProcessing = true;
            Status = OcrStatus.Running;
            try
            {
                StatusMessage = "Running text recognition...";
                Completion = ExecuteSearchablePdfCreatorAsync();
                await Completion;
                Status = OcrStatus.Finished;
                StatusMessage = "Text recognition finished!";
                OpenResultFile();
            }
            catch (Exception e)
            {
                Status = OcrStatus.Error;
                StatusMessage = "An error occured: " + e.Message;
                Debug.WriteLine("On error occured during execution: " + e.Message);
                Debug.WriteLine(e.StackTrace);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task ExecuteSearchablePdfCreatorAsync()
        {
            var tesseractOcr = new TesseractOcr(_culture);

            var outputFile = new OcrPdfOutputFile(_fileInfo.Name);
            await tesseractOcr.Execute(_fileInfo.FullName, outputFile);

            ResultFilePath = outputFile.FullPath;
        }

        public void OpenResultFile()
        {
            if (!_openCreatedFiles)
            {
                return;
            }

            var p = new Process();
            p.StartInfo = new ProcessStartInfo(ResultFilePath)
            {
                UseShellExecute = true
            };
            try
            {
                p.Start();
            }
            catch (Exception e)
            {
                StatusMessage = "Could not open generated file: " + e.Message;
            }
        }

        public static bool SupportsExtension(string extension)
        {
            if (extension.StartsWith('.'))
            {
                extension = extension.Substring(1);
            }

            var fileExtension = extension.ToLowerInvariant();
            var supportedExtensions = new string[] { "tiff", "jpg", "jpeg", "png" };
            if (supportedExtensions.Contains(fileExtension))
            {
                return true;
            }

            return false;
        }

        ~ImageOcrTask() { Dispose(false); }

        public void Dispose() { Dispose(true); }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            if (_resultFilePath != null)
            {
                try { File.Delete(_resultFilePath); }
                catch { } // best effort
                _resultFilePath = null;
            }
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            Dispose();
        }

        public bool CanOpenResultFile()
        {
            return !string.IsNullOrEmpty(ResultFilePath);
        }
    }
}