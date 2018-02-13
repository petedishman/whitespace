using System;
using System.Collections.Generic;
using System.IO;

namespace whitespace
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
                var relativePath = Path.GetRelativePath(rootPath, Path.GetDirectoryName(file));
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

        // for unit testing we'd need to be able to replace Directory.EnumerateFiles
        public IList<string> Find()
        {
            // need to actually support --exclude-folders
            // exclude a path if any component matches a given value
            // probably ignore parts of the path before where we're looking if that makes sense

            var files = new List<string>();
            foreach (var path in paths)
            {
                foreach (var includeExtension in includeExtensions)
                {
                    var searchOption = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    var searchPattern = "*."+includeExtension;
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

            return files;
        }

        private IList<string> paths;
        private IList<string> includeExtensions;
        private IList<string> excludeExtensions;
        private readonly IList<string> excludeFolders;
        private bool recurse;
    }
}
