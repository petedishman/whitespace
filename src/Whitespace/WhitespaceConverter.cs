using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace Whitespace
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
            await Task.WhenAll(GetFiles().Select((file) =>
            {
                // should we have a quiet mode where we don't do this?
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
            if (configuration.LineEndingStyle != LineEnding.Leave)
            {
                var desiredLineEnding = configuration.LineEndingStyle == LineEnding.CRLF ? "\r\n" : "\n";
                fileContents = lineEndings.Replace(fileContents, desiredLineEnding);
            }

            if (configuration.StripTrailingSpaces)
            {
                // Has to be done via 2 regexs, first gets most lines
                // second gets the last line in a file which may not end in newline

                // $2 is the newline style that the line ends in
                fileContents = trailingWhitespace.Replace(fileContents, "$2");
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

            if (configuration.Indentation == IndentationStyle.Spaces)
            {
                fileContents = leadingWhitespace.Replace(fileContents, match =>
                {
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
            else if (configuration.Indentation == IndentationStyle.Tabs)
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
                fileContents = leadingWhitespace.Replace(fileContents, match =>
                {
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
            try
            {
                // In .net core this is super simpe
                // as we have File.ReadAllTextAsync and File.WriteAllTextAsync
                // in standard .net we need a bit more effort

                var fileContents = "";
                using (var reader = File.OpenText(filepath))
                {
                    fileContents = await reader.ReadToEndAsync();
                }

                var convertedFile = ConvertFileText(fileContents);

                // we only want to write back to the file if we actually change anything.
                // that's awkward to do from the regexs themselves as we'll know the number
                // of matches we get, but we might just replace it with the same thing.
                // so instead we'll check the result.
                if (!convertedFile.Equals(fileContents))
                {
                    File.WriteAllText(filepath, convertedFile);
                }

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
            // might as well just pass in configuration at this point?
            var fileFinder = new Files(configuration);
            return fileFinder.Find();
        }
    }
}
