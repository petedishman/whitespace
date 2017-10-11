using System;
using McMaster.Extensions.CommandLineUtils;
using System.Reflection;

namespace whitespace
{
    class Program
    {
        static ProgramArguments AddConversionOptions(CommandLineApplication command)
        {
            var options = new ProgramArguments() {
                Path = command.Argument("path", "path description", true),
                TabWidth = command.Option("-t|--tabwidth", $"number of spaces per tab (default={ConversionOptions.DefaultTabWidth})", CommandOptionType.SingleValue),
                Recurse = command.Option("-r|--recurse", "recurse through child folders (default=false)", CommandOptionType.NoValue),
                IncludeExtensions = command.Option("-i|--include", "file extensions to include (default=<all>)", CommandOptionType.MultipleValue),
                ExcludeExtensions = command.Option("-e|--exclude", "extensions to exclude (default=<none>)", CommandOptionType.MultipleValue),
                ExcludeFolders = command.Option("-x|--eXclude-folders", "exclude folders (default=<none>)", CommandOptionType.MultipleValue),
                StripTrailingSpaces = command.Option("-s|--strip-trailing-spaces", "strip trailing whitespace from end of lines (default=false)", CommandOptionType.SingleValue)

                // add verbose option to list each file changed or inspected?
                // should add option to fix line-endings (i.e. LF or CRLF)
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
                Description = "Convert tabs to spaces, or spaces to tabs, that's it."
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
                    // could throw an exception which we just report directly
                    var configuration = options.GetConfiguration(ConversionType.SpacesToTabs);
                    var converter = new WhitespaceConverter(configuration);

                    converter.Run();

                    return 0;
                });
            });


            app.Command("spaces", (command) => {

                var options = AddConversionOptions(command);

                command.OnExecute(() => {
                    var configuration = options.GetConfiguration(ConversionType.SpacesToTabs);
                    var converter = new WhitespaceConverter(configuration);

                    converter.Run();

                    return 0;
                });
            });

            app.Command("report", (command) => {

                command.OnExecute(() => {
                    Console.WriteLine("Doing report");
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
    }
}
