using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace Whitespace
{
    public class WhitespaceConverter
    {
        private ConversionOptions configuration;

        public WhitespaceConverter(ConversionOptions configuration)
        {
            this.configuration = configuration;
        }

        public int FilesExamined { get; private set; }
        public int FilesUpdated { get; private set; }

        public async Task RunAsync()
        {
            var files = GetFiles();
            FilesExamined = files.Count;
            FilesUpdated = 0;

            if (configuration.Verbose)
            {
                Console.WriteLine("Processing {0:n0} file(s)", files.Count);
            }

            await Task.WhenAll(files.Select((file) => ConvertFileAsync(file)));
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

            /* Converting to spaces
             * when it's pure spaces that's obviously easy
             * when it's pure tabs -> # of spaces = tab count * tab width
             * When it's mixed tabs and spaces that's obviously more complicated
             * Assume tab-width=4
             * .\t (space followed by tab)
             * ..\t (2 spaces followed by tab)
             * ...\t
             * All of these instances would look like just a single tab in an editor
             * so that's what we need to turn it in to, a single tab worth of spaces (4 in this instance)
             *
             * ....\t
             * should however result in 8 spaces, as an editor would show that as two tabs worth of indent
             *
             * This doesn't just occur at the beginning of the line, but any time spaces are between tabs too
             * i.e.
             * \t..\t -> ........
             *
             * When converting to spaces, dealing with spaces following tabs is pretty simple.
             * Handling this when converting to tabs is more complicated.
             * For spaces, assume we have:
             * \t\t..
             * This should just be converted to
             * .......... (10 spaces)
             *
             * Converting to tabs
             * The same rules for converting to spaces actually apply when converting to tabs
             * We still need to handle mixed spaces and tabs, but the output is just different (tab rather than n spaces)
             * i.e.
             * .\t
             * ..\t
             * ...\t
             * would all result in:
             * \t
             *
             * ....\t -> \t\t
             *
             * What to do with spaces following tabs at the end of whitespace is more complicated
             * and may actually need to be configurable
             * i.e.
             * \t\t..
             * That could reasonably be converted to:
             * - \t\t       (drop the spaces and just have 2 tabs)
             * - \t\t\t     (treat the spaces as an extra tab, so have 3 tabs)
             * - \t\t..     (keep the spaces, 2 tabs are for indentation, 2 spaces for alignment)
             *
             * That final output would probably need to be configurable as it could be argued
             * we haven't actually sorted out whitespace and we still have mixed tabs/spaces.
             * But some people would like it
             */

            if (configuration.Indentation != IndentationStyle.Leave)
            {
                fileContents = leadingWhitespace.Replace(fileContents, match =>
                {
                    // match.Value is a string of tabs and/or spaces
                    // we want to remove any redundant spaces (spaces followed by a tab)
                    // then normalise the output to whatevers wanted
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

                    // we now have newSpaceCount set to the number of spaces
                    // we should have if converting to spaces.

                    // if we're doing spaces just return a string of that length
                    // if we're doing tabs, convert to a set of tabs and return that.
                    if (configuration.Indentation == IndentationStyle.Spaces)
                    {
                        return new string(' ', newSpaceCount);
                    }
                    else
                    {
                        // assuming tabwidth=4
                        // and we want equiv of 10 spaces,
                        // that's either
                        // \t\t
                        // \t\t\t
                        // \t\t..

                        // at the moment, we're going for \t\t but that may change (or become configurable)

                        // what about less than tab width of spaces at the beginning?
                        // that would have to go to a tab wouldn't? otherwise you're removing all indentiation?

                        // sod it, lets go for \t\t\t

                        // \t\t
                        /*
                        int tabCount = newSpaceCount / configuration.TabWidth;
                        return new string('\t', tabCount);
                        */

                        // \t\t\t
                        int tabCount = newSpaceCount / configuration.TabWidth;
                        if (newSpaceCount % configuration.TabWidth > 0)
                        {
                            tabCount++;
                        }
                        return new string('\t', tabCount);

                        // \t\t..
                        /*
                        int tabCount = newSpaceCount / configuration.TabWidth;
                        int extraSpaceCount = newSpaceCount % configuration.TabWidth;
                        var newLeadingWhitespace = new string('\t', tabCount);
                        if (extraSpaceCount > 0)
                        {
                            newLeadingWhitespace += new string(' ', extraSpaceCount);
                        }
                        return newLeadingWhitespace;
                        */
                    }
                });
            }

            return fileContents;
        }

        /// <summary>
        /// Returns true if the file is changed, false otherwise
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public async Task<bool> ConvertFileAsync(string filepath)
        {
            var fileChanged = false;
            try
            {
                // attempt to get current encoding of file so we can write it back with the same one
                var fileContents = "";
                Encoding fileEncoding;
                var defaultEncoding = Encoding.ASCII;
                using (var reader = new StreamReader(filepath, defaultEncoding, detectEncodingFromByteOrderMarks: true))
                {
                    fileContents = await reader.ReadToEndAsync();
                    fileEncoding = reader.CurrentEncoding;
                }

                var convertedFile = ConvertFileText(fileContents);

                // we only want to write back to the file if we actually change anything.
                // that's awkward to do from the regexs themselves as we'll know the number
                // of matches we get, but we might just replace it with the same thing.
                // so instead we'll check the result.
                if (!convertedFile.Equals(fileContents))
                {
                    // only write back to the file if this isn't a dry-run
                    if (!configuration.DryRun)
                    {
                        File.WriteAllText(filepath, convertedFile, fileEncoding);
                    }

                    fileChanged = true;
                    FilesUpdated++;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed processing {filepath}, err={ex.Message}");
            }

            if (configuration.Verbose)
            {
                Console.WriteLine("{0} - {1}", fileChanged ? "Updated" : "Untouched", filepath);
            }
            else if (fileChanged)
            {
                // not verbose, so only log files that we change (or would change in the case of --dry-run)
                Console.WriteLine("{0}", filepath);
            }

            return fileChanged;
        }

        public IList<string> GetFiles()
        {
            // might as well just pass in configuration at this point?
            var fileFinder = new Files(configuration);

            return fileFinder.Find();
        }
    }
}
