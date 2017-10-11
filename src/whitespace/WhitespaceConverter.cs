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


            return null;
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

    public class Files
    {
        public Files(IList<string> paths, bool recurse, IList<string> includeExtensions, IList<string> excludeExtensions)
        {
            this.paths = paths;
            this.includeExtensions = includeExtensions;
            this.excludeExtensions = excludeExtensions;
            this.recurse = recurse;
        }

        public Files(string path, bool recurse, IList<string> includeExtensions, IList<string> excludeExtensions)
        {
            this.paths = new List<string>() { path };
            this.includeExtensions = includeExtensions;
            this.excludeExtensions = excludeExtensions;
            this.recurse = recurse;
        }

        protected bool ShouldExcludeFile(string path)
        {
            if (excludeExtensions != null)
            {
                foreach (var extension in excludeExtensions)
                {
                    if (path.EndsWith("." + extension, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public IList<string> Find()
        {
            var files = new List<string>();

            foreach (var path in paths)
            {
                foreach (var includeExtension in includeExtensions)
                {
                    var searchOption = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    var searchPattern = "*."+includeExtension;

                    foreach (var file in Directory.EnumerateFiles(path, searchPattern, searchOption))
                    {
                        // might need to exclude it
                        if (ShouldExcludeFile(file))
                        {
                            continue;
                        }

                        files.Add(file);
                    }
                }
            }

            return files;
        }

        private IList<string> paths;
        private IList<string> includeExtensions;
        private IList<string> excludeExtensions;
        private bool recurse;
    }
}
