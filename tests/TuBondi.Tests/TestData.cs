namespace TuBondi.Tests;

internal static class TestData
{
    public static string ReadExample(string fileName)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        return File.ReadAllText(Path.Combine(root, "api-examples", fileName));
    }
}
