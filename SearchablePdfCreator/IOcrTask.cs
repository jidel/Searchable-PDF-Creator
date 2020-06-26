using System.Threading.Tasks;
using SearchablePdfCreator.FileTasks;

namespace SearchablePdfCreator
{
    public interface IOcrTask
    {
        string Name { get; }

        string ResultFilePath { get; }

        void Start();

        string StatusMessage { get; }

        OcrStatus Status { get; }

        bool IsProcessing { get; }

        public Task Completion { get; }

        void OpenResultFile();

        bool CanOpenResultFile();
    }
}