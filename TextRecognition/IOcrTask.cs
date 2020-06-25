using System.Threading.Tasks;
using TextRecognition.FileTasks;

namespace TextRecognition
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
    }
}