using System.Text.RegularExpressions;

namespace Web
{
    /// <summary>
    /// Reads a .env file into process environment variables and expands ${VAR} placeholders.
    /// </summary>
    internal static partial class DotEnv
    {
        /// <summary>
        /// Loads the nearest .env file found by walking up from the app's base directory
        /// (or the current directory). Variables that are already set are not overwritten,
        /// so real environment variables take precedence over the file.
        /// </summary>
        public static void  Load(string fileName = ".env")
        {
            var path = FindFile(fileName, AppContext.BaseDirectory) ?? FindFile(fileName, Directory.GetCurrentDirectory());
            if (path is null)
            {
                return;
            }

            foreach (var rawLine in File.ReadAllLines(path))
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith('#'))
                {
                    continue;
                }

                var separatorIndex = line.IndexOf('=', StringComparison.Ordinal);
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = line[..separatorIndex].Trim();
                var value = line[(separatorIndex + 1)..].Trim().Trim('"', '\'');

                if (Environment.GetEnvironmentVariable(key) is null)
                {
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }

        /// <summary>
        /// Replaces every ${VAR} in <paramref name="template"/> with the value of the environment variable VAR.
        /// </summary>
        public static string Expand(string template) =>
            PlaceholderRegex.Replace(template, match =>
            {
                var name = match.Groups["name"].Value;
                return Environment.GetEnvironmentVariable(name)
                    ?? throw new InvalidOperationException($"Environment variable '{name}' is not set.");
            });

        private static string? FindFile(string fileName, string startDirectory)
        {
            for (var directory = new DirectoryInfo(startDirectory); directory is not null; directory = directory.Parent)
            {
                var candidate = Path.Combine(directory.FullName, fileName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }

        [GeneratedRegex(_PLACEHOLDER_PATTERN, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture, 100)]
        private static partial Regex PlaceholderRegex { get; }

        private const string _PLACEHOLDER_PATTERN = @"\$\{(?<name>\w+)\}";
    }
}
