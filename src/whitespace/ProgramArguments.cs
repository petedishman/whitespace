using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;

namespace whitespace
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message)
            : base(message) { }
    }

    public class ProgramArguments
    {
        public CommandArgument Path { get; set; }
        public CommandOption TabWidth { get; set; }
        public CommandOption Recurse { get; set; }
        public CommandOption IncludeExtensions { get; set; }
        public CommandOption ExcludeExtensions { get; set; }
        public CommandOption ExcludeFolders { get; set; }
        public CommandOption StripTrailingSpaces { get; set; }
        public CommandOption LineEndings { get; set; }
        public CommandOption DryRun { get; set; }

        protected List<string> ParseFileExtensionsOption(List<string> values)
        {
            List<string> parsedValues = new List<string>();
            foreach (var value in values)
            {
                parsedValues.AddRange(value.Split(","));
            }
            return parsedValues;
        }

        public ConversionOptions GetConfiguration(ConversionType type)
        {
            var options = new ConversionOptions();
            options.Type = type;
            options.Paths = Path.Values.Count > 0 ? Path.Values : ConversionOptions.DefaultPaths;

            if (options.Paths.Count == 0)
            {
                throw new ConfigurationException("A path must be specified");
            }

            // need to convert to an int, should we refuse stupid values or let you go for it.
            if (TabWidth.HasValue())
            {
                if (!Int32.TryParse(TabWidth.Value(), out int tabWidth))
                {
                    throw new ConfigurationException("tabwidth must be a valid number");
                }
                options.TabWidth = tabWidth;
            }

            if (Path.Values.Count > 0)
            {
                options.Paths = Path.Values;
            }

            if (IncludeExtensions.HasValue())
            {
                options.IncludeExtensions = ParseFileExtensionsOption(IncludeExtensions.Values);
            }

            if (ExcludeExtensions.HasValue())
            {
                options.ExcludeExtensions = ParseFileExtensionsOption(ExcludeExtensions.Values);
            }

            if (ExcludeFolders.HasValue())
            {
                options.ExcludeFolders = ExcludeFolders.Values;
            }

            // the presence of recurse|dryrun means it's on, there is no value
            options.Recurse = Recurse.HasValue();
            options.DryRun = DryRun.HasValue();

            if (LineEndings.HasValue())
            {
                var lineEndingStyle = LineEndings.Value().ToLower();
                if (lineEndingStyle == "crlf")
                {
                    options.LineEndingStyle = LineEnding.CRLF;
                }
                else if (lineEndingStyle == "lf")
                {
                    options.LineEndingStyle = LineEnding.LF;
                }
                else
                {
                    throw new ConfigurationException("Line Endings must be crlf or lf");
                }
            }

            return options;
        }
    }
}
