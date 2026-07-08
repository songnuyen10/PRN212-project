namespace RestaurantPOS.DataAccessObjects;

// Centralized error logging for DAOs — plain text file, no logging framework
// dependency (matches this project's "stdlib first" precedent, see ADR-0002).
public static class AppLogger
{
    private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "error.log");
    private static readonly object WriteLock = new();

    public static void LogError(string context, Exception ex)
    {
        // A broken logger (disk full, permission denied, ...) must never throw and
        // replace the caller's own catch-block contract (return false/null).
        try
        {
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{context}] {ex}{Environment.NewLine}";
            lock (WriteLock)
            {
                File.AppendAllText(LogFilePath, line);
            }
        }
        catch
        {
            // Nowhere left to report this — logging is best-effort.
        }
    }
}
