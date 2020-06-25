using CliWrap;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace TextRecognition
{
    class TesseractOcr
    {
        private readonly string _culture;
        private string _executablePath;

        public TesseractOcr(string culture)
        {
            var executablePath = GetExecutablePath();
            _executablePath = Path.Combine(executablePath, "tesseract.exe");
            _culture = culture;
        }

        public async Task Execute(string input, IOcrOutputFile output)
        {
            var cmd = Cli.Wrap(_executablePath)
                .WithArguments(args => args
                    .Add(new string[] {"-l", _culture })
                    .Add(input)
                    .Add(output.PathWithoutExtension)
                    .Add(output.Extension));

            await cmd.ExecuteAsync();
        }

        private static string GetExecutablePath()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var assemblyDir = Path.GetDirectoryName(assemblyLocation);
            return Path.Combine(assemblyDir, "tesseract");
        }

        public static IEnumerable<string> AvailableLanguages()
        {
            var executablePath = GetExecutablePath();
            var tessdata = Path.Combine(executablePath, "tessdata");

            var files = Directory.GetFiles(tessdata, "*.traineddata");
            foreach (var filePath in files)
            {
                if (filePath == "osd.traineddata")
                {
                    continue;
                }

                var fileName = Path.GetFileName(filePath);
                yield return fileName.Replace(".traineddata", string.Empty);
            }
        }
    }
}