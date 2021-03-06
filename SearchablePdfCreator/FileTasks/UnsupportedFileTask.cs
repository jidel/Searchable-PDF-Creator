﻿using Prism.Mvvm;
using System.IO;
using System.Threading.Tasks;

namespace SearchablePdfCreator.FileTasks
{
    public class UnsupportedFileTask : BindableBase, IOcrTask
    {
        private FileInfo _fileInfo;

        public UnsupportedFileTask(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public string Name => _fileInfo.Name;

        public string StatusMessage => "Unsupported file type.";

        public OcrStatus Status => OcrStatus.Error;

        public bool IsProcessing => false;

        public string ResultFilePath => string.Empty;

        public Task Completion => Task.CompletedTask;

        public bool CanOpenResultFile()
        {
            return false;
        }

        public void OpenResultFile()
        {
            // Do nothing
        }

        public void Start()
        {
            // Do nothing
        }
    }
}