using System;
using McMaster.Extensions.CommandLineUtils;
using System.Reflection;

namespace whitespace
{
    class Program
    {
        // don't run on current path by default
        // that makes the options very hard to discover

        static ProgramArguments AddConversionOptions(CommandLineApplication command)
        {
            var options = new ProgramArguments() {
                Path = command.Argument("path", "the file path containing files to process", true),
                TabWidth = command.Option("-t|--tabwidth", $"number of spaces per tab (default={ConversionOptions.DefaultTabWidth})", CommandOptionType.SingleValue),
                Recurse = command.Option("-r|--recurse", "recurse through child folders (default=false)", CommandOptionType.NoValue),
                IncludeExtensions = command.Option("-i|--include", "file extensions to include (default=<all>)", CommandOptionType.MultipleValue),
                ExcludeExtensions = command.Option("-e|--exclude", "extensions to exclude (default=<none>)", CommandOptionType.MultipleValue),
                ExcludeFolders = command.Option("-x|--eXclude-folders", "exclude folders (default=<none>)", CommandOptionType.MultipleValue),
                StripTrailingSpaces = command.Option("-s|--strip-trailing-spaces", "strip trailing whitespace from end of lines (default=false)", CommandOptionType.SingleValue),
                LineEndings = command.Option("-l|--line-endings", "convert line endings to crlf|lf (default=leave alone)", CommandOptionType.SingleValue),
                DryRun = command.Option("-d|--dry-run", "just show files that would be changed", CommandOptionType.NoValue)
                // add verbose option to list each file changed or inspected?
            };
            command.HelpOption("-h|--help");
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
            /*
            whitespace tabify -tabwidth=4
            whitespace spacify -tabwidth=2
            whitespace report
            specify paths - default to current path
            file extensions to include
            file extensions to exclude
            folders to exclude
            */
            app.Command("tabs", (command) => {
                var options = AddConversionOptions(command);
                command.OnExecute(() => {
                    try
                    {
                        var configuration = options.GetConfiguration(ConversionType.SpacesToTabs);
                        return RunConverter(configuration);
                    }
                    catch (ConfigurationException ex)
                    {
                        Console.WriteLine("Invalid option: {0}\n", ex.Message);
                        app.ShowHelp(command.Name);
                        return 1;
                    }
                });
            });
            app.Command("spaces", (command) => {
                var options = AddConversionOptions(command);
                command.OnExecute(() => {
                    try
                    {
                        var configuration = options.GetConfiguration(ConversionType.TabsToSpaces);
                        return RunConverter(configuration);
                    }
                    catch (ConfigurationException ex)
                    {
                        Console.WriteLine("Invalid option: {0}\n", ex.Message);
                        app.ShowHelp(command.Name);
                        return 1;
                    }
                });
            });
            // will fire when nothing else does
            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });
            return app.Execute(args);
        }

        static int RunConverter(ConversionOptions configuration)
        {
            var converter = new WhitespaceConverter(configuration);
            converter.RunAsync().Wait();
            return 0;
        }
    }
}
