using System.IO;

namespace SearchablePdfCreator
{
    class OcrPdfOutputFile : IOcrOutputFile
    {
        public OcrPdfOutputFile(string fileName)
        {
            PathWithoutExtension = Path.Combine(Path.GetTempPath(), $"{fileName}.searchable");
        }

        public string PathWithoutExtension { get; }

        public string Extension => "pdf";

        public string FullPath => $"{PathWithoutExtension}.{Extension}";
    }
}