using Xunit.Abstractions;

namespace UltraModding.Templates.Tests;
/// <summary>
/// Temporary directory class for creating and managing a temporary directory.
/// </summary>
public sealed class TempDir : IDisposable
{
    private readonly string path;
    public ITestOutputHelper? Output { get; init; }
    public TempDir()
    {
        path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(path);
    }
    public TempDir(string dirName)
    {
        path = Path.Combine(Path.GetTempPath(), dirName);
        Directory.CreateDirectory(path);
    }
    public static implicit operator string(TempDir tempDir) => tempDir.path;
    public override string ToString() => path;

    public void Dispose()
    {
        if (!Directory.Exists(path))
        {
            Output?.WriteLine($"Temporary directory '{path}' does not exist, nothing to delete.");
            return;
        }
        Directory.Delete(path, true);
        Output?.WriteLine($"Temporary directory '{path}' deleted successfully.");
    }
}
