using System.Diagnostics;
using System.Text;

namespace AITrivia;

public static class AppLog
{
    private static readonly object Sync = new();
    private static readonly string LogDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "AITrivia.WinUI");
    private static readonly string LogPathValue = Path.Combine(LogDirectory, "debug.log");

    public static string LogPath => LogPathValue;

    public static void Info(string message) => Write("INFO", message);

    public static void Error(string message, Exception? ex = null)
        => Write("ERROR", ex is null ? message : $"{message}{Environment.NewLine}{ex}");

    private static void Write(string level, string message)
    {
        var line = $"{DateTime.Now:O} [{level}] {message}";

        lock (Sync)
        {
            Directory.CreateDirectory(LogDirectory);
            File.AppendAllText(LogPathValue, line + Environment.NewLine, Encoding.UTF8);
        }

        Debug.WriteLine(line);
    }
}
