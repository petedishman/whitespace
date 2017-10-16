using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
                ConvertFile(file);
            }
        }

        public void ConvertFile(string filepath)
        {
            // first line, handle BOM

            try
            {
                var bytes = File.ReadAllBytes(filepath);

                var convertedFile = new List<byte>(bytes.Length);

                bool firstLine = true;
                foreach (var line in GetLines(bytes))
                {
                    convertedFile.AddRange(ConvertLine(line, firstLine));
                    firstLine = false;
                }

                File.WriteAllBytes($"{filepath}.new", convertedFile.ToArray());
                // split in to lines
                // convert lines
                // write to new file

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed processing {filepath}, err={ex.Message}");
            }
        }

        public byte[] ConvertLine(ArraySegment<byte> line, bool firstLine)
        {
            // if firstLine is set we should worry about BOM markers

            // otherwise we're looking for upfront whitespace
            // potentially trimming end whitespace.
            // and maybe doing other stuff

            // configuration.Type == ConversionType.SpacesToTabs;

            // . = space, _ = tab
            //
            // ............what
            // ___what
            // __..what

            int firstNonWhitespaceIndex = 0;
            int LastNonWhitespaceIndex = 0;

            int initialSpaceCount = 0;
            int initialTabCount = 0;

            if (firstLine)
            {
                // look for a BOM, we'll persist it and do everything else after it?
            }

            bool inWhitespace = true;
            int index = 0;
            while (inWhitespace && index < line.Count)
            {
                if (line[index] == ' ')
                {
                    initialSpaceCount++;
                }
                else if (line[index] == '\t')
                {
                    initialTabCount++;
                }
                else
                {
                    inWhitespace = false;
                    firstNonWhitespaceIndex = index;
                }
                index++;
            }

            // ........test...\r\n

            for (index = line.Count; index > 0; index--)
            {
                if (line[index] == '\r' || line[index] == '\n' || line[index] == ' ' || line[index] == '\t')
                {
                    continue;
                }
                else
                {
                    LastNonWhitespaceIndex = index;
                    break;
                }
            }

            


            if (configuration.StripTrailingSpaces)
            {

            }
            //abcd   4

            

            // convert line endings, need to know what it ends on.
            LineEnding lineEnding = LineEnding.None;
            int lineLength = line.Count;
            if (line.Count >= 2 && line[lineLength - 1] == '\n' && line[lineLength - 2] == '\r')
            {
                // \r\n
                lineEnding = LineEnding.CRLF;
            }
            else if (line.Count >= 2 && line[lineLength - 1] == '\n' && line[lineLength - 2] != '\r')
            {
                // \n
                lineEnding = LineEnding.LF;
            }
            else if (line.Count == 1 && line[0] == '\n')
            {
                // \n
                lineEnding = LineEnding.LF;
            }

            int newLineLength = 0;
            newLineLength += LastNonWhitespaceIndex - firstNonWhitespaceIndex;
            newLineLength += (lineEnding == LineEnding.CRLF ? 2 : (lineEnding == LineEnding.LF ? 1 : 0));

            int spacesToAdd = 0;
            int tabsToAdd = 0;
            if (configuration.Type == ConversionType.TabsToSpaces)
            {
                // step through initial whitepsace, any tab character converted to X spaces
                // any spaces passed through.

                spacesToAdd = initialSpaceCount + initialTabCount * configuration.TabWidth;
                newLineLength += spacesToAdd;
            }

            if (configuration.Type == ConversionType.SpacesToTabs)
            {
                // step through initial whitespace
                // collect all spaces/tabs and then add those
                tabsToAdd += initialTabCount;
                tabsToAdd += (int) Math.Ceiling((double) initialSpaceCount / (double) configuration.TabWidth);
                newLineLength += tabsToAdd * configuration.TabWidth;
            }

            // This is a bit mad surely?

            byte[] newLine = new byte[newLineLength];

            // add tabs/spaces
            if (configuration.Type == ConversionType.TabsToSpaces)
            {
                Fill(newLine, 0, spacesToAdd, (byte)' ');
            }
            if (configuration.Type == ConversionType.SpacesToTabs)
            {
                Fill(newLine, 0, tabsToAdd * configuration.TabWidth, (byte)'\t');
            }

            // add content
            Copy(newLine, line, firstNonWhitespaceIndex, LastNonWhitespaceIndex);

            // add line ending
            if (lineEnding == LineEnding.CRLF)
            {
                line[newLineLength - 2] = (byte)'\r';
                line[newLineLength - 1] = (byte)'\n';
            }
            else if (lineEnding == LineEnding.LF)
            {
                line[newLineLength - 1] = (byte)'\n';
            }

            return newLine;
        }

        private void Fill(byte[] line, int startIndex, int endIndex, byte toFill)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                line[i] = toFill;
            }
        }
        private void Copy(byte[] line, ArraySegment<byte> source, int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                line[i] = source[i];
            }
        }

        public IEnumerable<ArraySegment<byte>> GetLines(byte[] contents)
        {
            int startOfCurrentLine = 0;
            for (int i = 0; i < contents.Length; i++)
            {
                // looking for \r\n, or just \n
                // which means we actually just need to look for \n
                if (contents[i] == '\n')
                {
                    // almost guaranteed to be a +/- 1 error in this function
                    int currentLineLength = i - startOfCurrentLine;
                    yield return new ArraySegment<byte>(contents, startOfCurrentLine, currentLineLength);
                }
            }

            // cope with no new line at end of file.
            if (startOfCurrentLine < contents.Length)
            {
                yield return new ArraySegment<byte>(contents, startOfCurrentLine, contents.Length - startOfCurrentLine);
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

    public class PathResolver
    {
        // need to take a bunch of paths and sort out cwd, relative paths etc.
    }
}
