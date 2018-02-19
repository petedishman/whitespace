using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Whitespace
{
    public class Files
    {
        public Files(ConversionOptions options)
        {
            this.paths = options.Paths;
            this.includeExtensions = options.IncludeExtensions;
            this.excludeExtensions = options.ExcludeExtensions;
            this.excludeFolders = options.ExcludeFolders;
            this.recurse = options.Recurse;

            this.files = options.Files;
            this.listFile = options.ListFile;
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
            var filesToProcess = new List<string>();

            // first check for any matching files under path(s)
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

            // add any explicitly mentioned files
            filesToProcess.AddRange(this.files);

            // read in any files from list file
            filesToProcess.AddRange(GetFilesFromListFile());

            // now sort the list so that the order isn't mental
            filesToProcess.Sort();

            return filesToProcess;
        }

        protected IList<string> GetFilesFromListFile()
        {
            var listedFiles = new List<string>();

            if (listFile.Length > 0)
            {
                // no validation at this stage
                var file = File.ReadAllLines(listFile);
                // remove empty lines
                return file.Where(line => line.Trim().Length > 0).ToList();                
            }

            return listedFiles;
        }

        private IList<string> paths;
        private IList<string> includeExtensions;
        private IList<string> excludeExtensions;
        private readonly IList<string> excludeFolders;
        private bool recurse;

        private IList<string> files;
        private string listFile;
    }
}
