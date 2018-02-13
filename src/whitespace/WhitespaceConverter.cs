using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace whitespace
{
    public class WhitespaceConverter
    {
        private ConversionOptions configuration;

        public WhitespaceConverter(ConversionOptions configuration)
        {
            this.configuration = configuration;
        }

        public async Task RunAsync()
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

            await Task.WhenAll(GetFiles().Select((file) => {
                Console.WriteLine(" {0}", file);

                if (configuration.DryRun)
                {
                    return Task.CompletedTask;
                }

                return ConvertFileAsync(file);
            }));
        }

        // detect all line endings LF or CRLF ready to normalize them
        Regex lineEndings = new Regex(@"(\n|\r\n?)", RegexOptions.Singleline | RegexOptions.Compiled);
        // find trailing whitespace before a lf|crlf - need to do this as Multiline and $ only matches lf
        Regex trailingWhitespace = new Regex(@"([\t ]+)(\r?\n)", RegexOptions.Singleline | RegexOptions.Compiled);
        // the above regex will miss trailing whitespace at the end of a file, so find that with this
        Regex endOfFileWhitespace = new Regex(@"([\t ]+)$", RegexOptions.Singleline | RegexOptions.Compiled);
        // find leading whitespace ready to normalize to tabs or spaces
        Regex leadingWhitespace = new Regex(@"^([\t ]+)", RegexOptions.Multiline | RegexOptions.Compiled);

        public string ConvertFileText(string fileContents)
        {
            // fileContents is a giant string with the whole file in it
            // run it through some/one regular expression(s) to convert it
            // need to run multiple regexes
            if (configuration.LineEndingStyle != LineEnding.None)
            {
                // normalize line endings
                var lineEnding = configuration.LineEndingStyle == LineEnding.CRLF ? "\r\n" : "\n";
                fileContents = lineEndings.Replace(fileContents, lineEnding);
            }

            if (configuration.StripTrailingSpaces)
            {
                // Multiline mode means $ only recognises \n
                // so we need to look for \r?$ instead
                fileContents = trailingWhitespace.Replace(fileContents, "$2");

                // the above won't detect trailing space on the last line of a file
                // when it doesn't have a newline following it
                // but we need to do it like this to avoid issues with different
                // line endings.

                // can we do a second regex that just does a replace for the end of file?
                fileContents = endOfFileWhitespace.Replace(fileContents, "");
            }

            // neither of these are actually that simple

            // converting tabs to spaces, tabwidth=4
            // " \t"    => "    "          1 space + 1 tab => 4 spaces
            // "  \t"   => "    "          2 space + 1 tab => 4 spaces
            // "   \t"  => "    "          3 space + 1 tab => 4 spaces
            // "    \t" => "        "      4 space + 1 tab => 8 spaces
            // "\t "    => "     "         1 tab + 1 space => 5 spaces
            // "\t  "   => "      "        1 tab + 2 space => 6 spaces
            // "\t   "  => "       "       1 tab + 3 space => 7 spaces
            // "\t    " => "        "      1 tab + 4 space => 8 spaces
            // " \t "   => "     "
            // "  \t "  => "     "
            // "   \t " => "     "
            // "" => ""
            // "" => ""
            // "" => ""
            // "" => ""
            // "" => ""
            //
            // so spaces should be ignored when grouped with a tab
            // " \t" will look just like a "\t" in an editor, so it must
            // be converted to just "    ", i.e one tabs worth of spaces.
            //
            // that may be the easiest way of converting as well.
            // first you clean it up, to tabs only, or tabs at the beginning
            // then do the conversion
            //
            // to clean up mixed tabs/spaces with tabwidth T
            // loop through
            //   if
            //
            //
            // if the spaces come before a tab they get ignored
            //
            //
            // 01234567890
            // ...........

            if (configuration.Type == ConversionType.TabsToSpaces)
            {
                fileContents = leadingWhitespace.Replace(fileContents, match => {
                    // match.Value is a string of tabs and/or spaces
                    // we want to remove any redundant spaces then convert
                    // tabs to spaces, then return that.
                    var whitespace = match.Value;
                    int spaceCount = 0;
                    int newSpaceCount = 0;
                    for (int i = 0; i < match.Value.Length; i++)
                    {
                        if (whitespace[i] == ' ')
                        {
                            spaceCount++;
                            if (spaceCount == configuration.TabWidth)
                            {
                                spaceCount = 0;
                                newSpaceCount += configuration.TabWidth;
                            }
                        }
                        else // tab
                        {
                            spaceCount = 0;
                            newSpaceCount += configuration.TabWidth;
                        }
                    }
                    newSpaceCount += spaceCount;
                    return new string(' ', newSpaceCount);
                });
            }
            else // ConversionType.SpacesToTabs
            {
                // is this right
                // do you want to turn '\t ' to '\t\t'
                // or should that just be '\t'
                // rather than rounding up the space count, maybe it should be normal rounding?
                // so '\t  ' => '\t'
                // and '\t   ' => '\t\t'
                // so only turn spaces to tabs if they're at least half of tabWidth?
                //
                // definitely need to turn " \t" to just "\t"
                // even just "  \t" => "\t"
                // and "   \t" => "\t"
                // as that's what they'll all appear like in an editor
                // assuming a tab width of 4
                //
                // "\t " => ""
                fileContents = leadingWhitespace.Replace(fileContents, match => {
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

        public async Task<bool> ConvertFileAsync(string filepath)
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
            var fileFinder = new Files(
                this.configuration.Paths,
                this.configuration.Recurse,
                this.configuration.IncludeExtensions,
                this.configuration.ExcludeExtensions,
                this.configuration.ExcludeFolders);

            var files = fileFinder.Find();

            return files;
        }
    }
}
