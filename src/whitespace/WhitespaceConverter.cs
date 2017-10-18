using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace whitespace
{

    /* TODO:
     * Async'ify it
     *
     * Refactor so that it's possible to test without actual files on disk
     */
    public class WhitespaceConverter
    {
        private ConversionOptions configuration;

        public WhitespaceConverter(ConversionOptions configuration)
        {
            this.configuration = configuration;
        }

        public void Run()
        {
            // get the files
            // foreach file
            //   open file
            //   read in each line
            //      optionally strip trailing space
            //      optionally convert line ending
            //      convert tabs|spaces at beginning of line to spaces|tabs
            //   write file back
            //   back up file?

            // what about tabs|spaces in the middle of a line
            // would need to do something where you don't touch whitespace inside " or '


            var files = GetFiles();
            foreach (var file in files)
            {
                Console.WriteLine(" {0}", file);
                ConvertFile(file).Wait();
            }
        }

        public string ConvertFileText(string fileContents)
        {
            // fileContents is a giant string with the whole file in it
            // run it through some/one regular expression(s) to convert it


            // need to run multiple regexes

            if (configuration.LineEndingStyle != LineEnding.None)
            {
                // normalize line endings
                var normalizeLineEndingsRegex = new Regex(@"(\n|\r\n?)", RegexOptions.Singleline);
                var lineEnding = configuration.LineEndingStyle == LineEnding.CRLF ? "\r\n" : "\n";
                fileContents = normalizeLineEndingsRegex.Replace(fileContents, lineEnding);
            }

            if (configuration.StripTrailingSpaces)
            {
                var stripTrailingWhitespaceRegex = new Regex(@"([\s]+)$", RegexOptions.Multiline);

                fileContents = stripTrailingWhitespaceRegex.Replace(fileContents, "");
            }

            if (configuration.Type == ConversionType.TabsToSpaces)
            {
                var tabAsSpaces = new String(' ', configuration.TabWidth);
                var tabsToSpacesRegex = new Regex(@"^([\s]+)", RegexOptions.Multiline);
                fileContents = tabsToSpacesRegex.Replace(fileContents, match => {
                    return match.Value.Replace("\t", tabAsSpaces);
                });
            }
            else
            {
                // is this right
                // do you want to turn '\t ' to '\t\t'
                // or should that just be '\t'
                // rather than rounding up the space count, maybe it should be normal rounding?
                // so '\t  ' => '\t'
                // and '\t   ' => '\t\t'
                // so only turn spaces to tabs if they're at least half of tabWidth?

                var spacecsToTabsRegex = new Regex(@"^([\s]+)", RegexOptions.Multiline);
                fileContents = spacecsToTabsRegex.Replace(fileContents, match => {
                    // match.Value contains spaces & tabs
                    // need to know how many spaces there are and how many tabs.
                    // then replace initial whitespace with x tabs
                    int tabCount = 0;
                    int spaceCount = 0;
                    foreach (var ch in match.Value)
                    {
                        if (ch == ' ')
                            spaceCount++;
                        if (ch == '\t')
                            tabCount++;
                    }
                    int replacementTabCount = tabCount;
                    if (spaceCount > 0)
                    {
                        replacementTabCount += (int)Math.Ceiling((double)spaceCount / (double)configuration.TabWidth);
                    }
                    return new String('\t', replacementTabCount);
                });
            }

            return fileContents;
        }

        public async Task<bool> ConvertFile(string filepath)
        {
            // first line, handle BOM

            try
            {
                var fileContents = await File.ReadAllTextAsync(filepath);

                var convertedFile = ConvertFileText(fileContents);

                await File.WriteAllTextAsync(filepath, convertedFile);

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed processing {filepath}, err={ex.Message}");
                return false;
            }
        }

        public IList<string> GetFiles()
        {
            var fileFinder = new Files(this.configuration.Paths,
                this.configuration.Recurse,
                this.configuration.IncludeExtensions,
                this.configuration.ExcludeExtensions);

            var files = fileFinder.Find();

            return files;
        }
    }
}
