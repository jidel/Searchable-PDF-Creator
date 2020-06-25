using System.IO;

namespace TextRecognition
{
    class OcrPdfOutputFile : IOcrOutputFile
    {
        public OcrPdfOutputFile(DirectoryInfo directory, string fileName)
        {
            PathWithoutExtension = Path.Combine(directory.FullName, $"{fileName}.searchable");
        }

        public string PathWithoutExtension { get; }

        public string Extension => "pdf";

        public string FullPath => $"{PathWithoutExtension}.{Extension}";
    }
}