namespace TextRecognition
{
    public interface IOcrOutputFile
    {
        string PathWithoutExtension { get; }
        string Extension { get; }
    }
}