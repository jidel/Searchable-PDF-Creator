namespace SearchablePdfCreator
{
    public interface IOcrOutputFile
    {
        string PathWithoutExtension { get; }
        string Extension { get; }
    }
}