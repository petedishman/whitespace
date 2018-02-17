using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;

namespace Whitespace
{
    public class ProgramArguments
    {
        public CommandArgument Path { get; set; }
        public CommandOption IndentStyle { get; set; }
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
                parsedValues.AddRange(value.Split(','));
            }
            return parsedValues;
        }

        public ConversionOptions GetConfiguration()
        {
            var options = new ConversionOptions();

            options.Paths = Path.Values.Count > 0 ? Path.Values : ConversionOptions.DefaultPaths;

            if (options.Paths.Count == 0)
            {
                throw new ConfigurationException("A path must be specified");
            }

            if (IndentStyle.HasValue())
            {
                var style = IndentStyle.Value().ToLower();
                if (style == "tabs")
                {
                    options.Indentation = IndentationStyle.Tabs;
                }
                else if (style == "spaces")
                {
                    options.Indentation = IndentationStyle.Spaces;
                }
                else if (style == "leave")
                {
                    options.Indentation = IndentationStyle.Leave;
                }
                else
                {
                    throw new ConfigurationException($"'{style}' is an invalid indentation style");
                }
            }

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

            options.StripTrailingSpaces = StripTrailingSpaces.HasValue();

            // no point going any further if one of the change options isn't actually specified
            if (options.StripTrailingSpaces == false &&
                options.Indentation == IndentationStyle.Leave &&
                options.LineEndingStyle == LineEnding.Leave)
            {
                throw new ConfigurationException("Nothing to do, you must specify one of --strip-trailing-spaces, --line-endings or --indent");
            }

            if (TabWidth.HasValue())
            {
                if (!Int32.TryParse(TabWidth.Value(), out int tabWidth))
                {
                    throw new ConfigurationException("tabwidth must be a valid number");
                }
                options.TabWidth = tabWidth;
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

            return options;
        }
    }
}
