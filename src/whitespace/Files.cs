using System;
using System.Collections.Generic;
using System.IO;

namespace Whitespace
{
    public class Files
    {
        public Files(IList<string> paths, bool recurse, IList<string> includeExtensions, IList<string> excludeExtensions, IList<string> excludeFolders)
        {
            this.paths = paths;
            this.includeExtensions = includeExtensions;
            this.excludeExtensions = excludeExtensions;
            this.excludeFolders = excludeFolders;
            this.recurse = recurse;
        }

        public Files(string path, bool recurse, IList<string> includeExtensions, IList<string> excludeExtensions, IList<string> excludeFolders)
        {
            this.paths = new List<string>() { path };
            this.includeExtensions = includeExtensions;
            this.excludeExtensions = excludeExtensions;
            this.excludeFolders = excludeFolders;
            this.recurse = recurse;
        }

        protected bool ShouldExcludeFile(string rootPath, string file)
        {
            if (excludeFolders.Count > 0)
            {
                // rootPath is something like c:\build\liberty\tputils
                // file is: c:\build\liberty\tputils\fileconverter\main.cpp
                // we want fileconverter\main.cpp

                // .net core has Path.GetRelativePath() which is very handy
                //var relativePath = Path.GetRelativePath(rootPath, Path.GetDirectoryName(file));
                // instead we bodge it with this:
                var relativePath = Path.GetDirectoryName(file).Replace(rootPath, "");

                foreach (var folder in excludeFolders)
                {
                    if ($"\\{relativePath}\\".Contains($"\\{folder}\\"))
                    {
                        return true;
                    }
                }
            }

            if (excludeExtensions != null)
            {
                foreach (var extension in excludeExtensions)
                {
                    if (file.EndsWith("." + extension, StringComparison.CurrentCultureIgnoreCase))
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
                    var searchPattern = "*." + includeExtension;
                    foreach (var file in Directory.EnumerateFiles(path, searchPattern, searchOption))
                    {
                        if (ShouldExcludeFile(path, file))
                        {
                            continue;
                        }
                        files.Add(file);
                    }
                }
            }

            files.Sort();

            return files;
        }

        private IList<string> paths;
        private IList<string> includeExtensions;
        private IList<string> excludeExtensions;
        private readonly IList<string> excludeFolders;
        private bool recurse;
    }
}
