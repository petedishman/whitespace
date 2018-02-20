using System;
using McMaster.Extensions.CommandLineUtils;
using System.Reflection;
using System.Diagnostics;

namespace Whitespace
{
    class Program
    {
        static ProgramArguments AddConversionOptions(CommandLineApplication command)
        {
            var options = new ProgramArguments()
            {
                Path = command.Option("--path", "file paths to search for files in", CommandOptionType.MultipleValue),
                File = command.Option("--file", "a specific file to process", CommandOptionType.MultipleValue),
                List = command.Option("--list", "a text file containing the filenames to process", CommandOptionType.SingleValue),
                IndentStyle = command.Option("--indent", "spaces, tabs or leave (default=leave)", CommandOptionType.SingleValue),
                TabWidth = command.Option("--tabwidth", $"number of spaces per tab (default={ConversionOptions.DefaultTabWidth})", CommandOptionType.SingleValue),
                Recurse = command.Option("--recurse", "recurse through sub-folders when finding files (default=false)", CommandOptionType.NoValue),
                IncludeExtensions = command.Option("--include", "file extensions to include, e.g. --include=cpp --include=c,cpp,h,hpp (default=<all>)", CommandOptionType.MultipleValue),
                ExcludeExtensions = command.Option("--exclude", "file extensions to exclude (default=<none>)", CommandOptionType.MultipleValue),
                ExcludeFolders = command.Option("--exclude-folders", "exclude folders (default=<none>)", CommandOptionType.MultipleValue),
                StripTrailingSpaces = command.Option("--strip-trailing-spaces", "strip trailing whitespace from end of lines (default=false)", CommandOptionType.NoValue),
                LineEndings = command.Option("--line-endings", "convert line endings to crlf|lf (default=leave alone)", CommandOptionType.SingleValue),
                DryRun = command.Option("--dry-run", "just show files that would be changed but don't do anything", CommandOptionType.NoValue),
                Verbose = command.Option("--verbose", "Show all files inspected and generally more information", CommandOptionType.NoValue)
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

            if (args.Length == 0)
            {
                app.ShowHelp();
                return 0;
            }

            app.OnExecute(() =>
            {
                try
                {
                    var configuration = options.GetConfiguration();
                    var converter = new WhitespaceConverter(configuration);
                    var timer = Stopwatch.StartNew();

                    converter.RunAsync().Wait();

                    timer.Stop();
                    if (configuration.Verbose)
                    {
                        Console.WriteLine("\nAll done (took {0:n0} ms)", timer.ElapsedMilliseconds);
                        Console.WriteLine("Examined {0:n0} files, updated {1:n0}", converter.FilesExamined, converter.FilesUpdated);
                    }
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
