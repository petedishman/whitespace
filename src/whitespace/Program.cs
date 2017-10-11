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
                TabWidth = command.Option("-t|--tabwidth", "tab width", CommandOptionType.SingleValue),
                Recurse = command.Option("-r|--recurse", "recurse or not", CommandOptionType.NoValue),
                IncludeExtensions = command.Option("-i|--include", "extensions to include", CommandOptionType.MultipleValue),
                ExcludeExtensions = command.Option("-e|--exclude", "extensions to exclude", CommandOptionType.MultipleValue),
                ExcludeFolders = command.Option("-x|--eXclude-folders", "exclude folders", CommandOptionType.MultipleValue),
                StripTrailingSpaces = command.Option("-s|--strip-trailing-spaces", "", CommandOptionType.SingleValue)
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

            app.Command("tabify", (command) => {

                var options = AddConversionOptions(command);

                command.OnExecute(() => {
                    // could throw an exception which we just report directly
                    var configuration = options.GetConfiguration(ConversionType.SpacesToTabs);
                    var converter = new WhitespaceConverter(configuration);

                    converter.Run();

                    return 0;
                });
            });


            app.Command("spacify", (command) => {

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
