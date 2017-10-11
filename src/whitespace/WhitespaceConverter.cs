using System;
using System.Collections.Generic;
using System.IO;

namespace whitespace
{
    internal class WhitespaceConverter
    {
        private ConversionOptions configuration;

        public WhitespaceConverter(ConversionOptions configuration)
        {
            this.configuration = configuration;
        }

        public void Run()
        {
            // first thing we need to do is get a list of files to process
            // based on a set of paths and extensions to include/exclude

            var fileFinder = new Files(this.configuration.Paths,
                this.configuration.Recurse,
                this.configuration.IncludeExtensions,
                this.configuration.ExcludeExtensions);

            var files = fileFinder.Find();

            Console.WriteLine("Files");
            foreach (var file in files)
            {
                Console.WriteLine(" {0}", file);
            }
        }
    }

    class PathResolver
    {
        // need to take a bunch of paths and sort out cwd, relative paths etc.
    }

    class Files
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
