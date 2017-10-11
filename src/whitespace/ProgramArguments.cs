using System;
using McMaster.Extensions.CommandLineUtils;

namespace whitespace
{
    public class ProgramArguments
    {
        public CommandArgument Path { get; set; }
        public CommandOption TabWidth { get; set; }
        public CommandOption Recurse { get; set; }
        public CommandOption IncludeExtensions { get; set; }
        public CommandOption ExcludeExtensions { get; set; }
        public CommandOption ExcludeFolders { get; set; }
        public CommandOption StripTrailingSpaces { get; set; }

        public ConversionOptions GetConfiguration(ConversionType type)
        {
            var options = new ConversionOptions();
            options.Type = type;

            options.Paths = Path.Values.Count > 0 ? Path.Values : ConversionOptions.DefaultPaths;

            // need to convert to an int, should we refuse stupid values or let you go for it.
            if (TabWidth.HasValue())
            {
                if (!Int32.TryParse(TabWidth.Value(), out int tabWidth))
                {
                    throw new Exception("tabwidth must be a valid number");
                }
                options.TabWidth = tabWidth;
            }
            else
            {
                options.TabWidth = ConversionOptions.DefaultTabWidth;
            }

            if (Path.Values.Count > 0)
            {
                options.Paths = Path.Values;
            }
            else
            {
                options.Paths = ConversionOptions.DefaultPaths;
            }

            if (IncludeExtensions.HasValue())
            {
                options.IncludeExtensions = IncludeExtensions.Values;
            }
            else
            {
                options.IncludeExtensions = ConversionOptions.DefaultIncludeExtensions;
            }

            if (ExcludeExtensions.HasValue())
            {
                options.ExcludeExtensions = ExcludeExtensions.Values;
            }
            else
            {
                options.ExcludeExtensions = ConversionOptions.DefaultExcludeExtensions;
            }

            if (Recurse.HasValue())
            {
                // it's the presence we're interested in, it shouldn't have a value
                options.Recurse = true;
            }
            else
            {
                options.Recurse = ConversionOptions.DefaultRecurse;
            }

            return options;
        }
    }
}
