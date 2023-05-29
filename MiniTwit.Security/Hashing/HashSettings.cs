namespace MiniTwit.Security.Hashing;

public class HashSettings
{
    public int DegreeOfParallelism { get; set; }
    public int Iterations { get; set; }
    public int MemorySizeMB { get; set; }
    public int TagLength { get; set; }
    public int SaltLength { get; set; }
}