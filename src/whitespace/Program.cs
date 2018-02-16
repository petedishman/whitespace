using System;
using McMaster.Extensions.CommandLineUtils;
using System.Reflection;

namespace Whitespace
{
    class Program
    {
        static ProgramArguments AddConversionOptions(CommandLineApplication command)
        {
            var options = new ProgramArguments()
            {
                Path = command.Argument("path", "the file path containing files to process", multipleValues: true),
                IndentStyle = command.Option("--indent", "spaces, tabs or leave (default=leave)", CommandOptionType.SingleValue),
                TabWidth = command.Option("-t|--tabwidth", $"number of spaces per tab (default={ConversionOptions.DefaultTabWidth})", CommandOptionType.SingleValue),
                Recurse = command.Option("-r|--recurse", "recurse through sub-folders when finding files (default=false)", CommandOptionType.NoValue),
                IncludeExtensions = command.Option("-i|--include", "file extensions to include, e.g. --include=cpp --include=c,cpp,h,hpp (default=<all>)", CommandOptionType.MultipleValue),
                ExcludeExtensions = command.Option("-e|--exclude", "file extensions to exclude (default=<none>)", CommandOptionType.MultipleValue),
                ExcludeFolders = command.Option("-x|--exclude-folders", "exclude folders (default=<none>)", CommandOptionType.MultipleValue),
                StripTrailingSpaces = command.Option("-s|--strip-trailing-spaces", "strip trailing whitespace from end of lines (default=false)", CommandOptionType.SingleValue),
                LineEndings = command.Option("-l|--line-endings", "convert line endings to crlf|lf (default=leave alone)", CommandOptionType.SingleValue),
                DryRun = command.Option("-d|--dry-run", "just show files that would be changed but don't do anything", CommandOptionType.NoValue)
            };

            return options;
        }

        static int Main(string[] args)
        {
            var app = new CommandLineApplication()
            {
                Name = "Whitespace",
                FullName = "Whitespace",
                Description = "Fix whitespace in a bunch of files: change to tabs/spaces, trim trailing whitespace, normalize line endings etc."
            };
            app.HelpOption("-h|--help");
            app.VersionOptionFromAssemblyAttributes(Assembly.GetExecutingAssembly());
            var options = AddConversionOptions(app);

            app.OnExecute(() =>
            {
                try
                {
                    var configuration = options.GetConfiguration();
                    var converter = new WhitespaceConverter(configuration);
                    converter.RunAsync().Wait();
                    return 0;
                }
                catch (ConfigurationException ex)
                {
                    Console.WriteLine("Invalid option: {0}\n", ex.Message);
                    app.ShowHelp();
                    return 1;
                }
            });

            try
            {
                return app.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                // thrown on unexpected argument
                Console.Error.WriteLine(ex.Message);
                Console.WriteLine("");
                app.ShowHelp();
                return 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error ({ex.Message})");
                return 1;
            }
        }
    }
}
