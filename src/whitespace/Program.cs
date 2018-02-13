using System;
using McMaster.Extensions.CommandLineUtils;
using System.Reflection;

namespace Whitespace
{
    class Program
    {
        static ProgramArguments AddConversionOptions(CommandLineApplication command)
        {
            var options = new ProgramArguments() {
                Path = command.Argument("path", "the file path containing files to process", true),
                IndentStyle = command.Option("--indent", "spaces, tabs, leave (default=leave)", CommandOptionType.SingleValue),
                TabWidth = command.Option("-t|--tabwidth", $"number of spaces per tab (default={ConversionOptions.DefaultTabWidth})", CommandOptionType.SingleValue),
                Recurse = command.Option("-r|--recurse", "recurse through child folders (default=false)", CommandOptionType.NoValue),
                IncludeExtensions = command.Option("-i|--include", "file extensions to include (default=<all>)", CommandOptionType.MultipleValue),
                ExcludeExtensions = command.Option("-e|--exclude", "extensions to exclude (default=<none>)", CommandOptionType.MultipleValue),
                ExcludeFolders = command.Option("-x|--exclude-folders", "exclude folders (default=<none>)", CommandOptionType.MultipleValue),
                StripTrailingSpaces = command.Option("-s|--strip-trailing-spaces", "strip trailing whitespace from end of lines (default=false)", CommandOptionType.SingleValue),
                LineEndings = command.Option("-l|--line-endings", "convert line endings to crlf|lf (default=leave alone)", CommandOptionType.SingleValue),
                DryRun = command.Option("-d|--dry-run", "just show files that would be changed", CommandOptionType.NoValue)
                // add verbose option to list each file changed or inspected?
            };
            //command.HelpOption("-h|--help");
            return options;
        }

        static int Main(string[] args)
        {
            var app = new CommandLineApplication()
            {
                Name = "Whitespace",
                FullName = "Whitespace",
                Description = "Convert tabs to spaces, or spaces to tabs, normalize line endings and trim trailing white space that's it."
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
