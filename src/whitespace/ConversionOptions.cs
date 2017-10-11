using System.Collections.Generic;

namespace whitespace
{
    // An instance of this class will be passed in when we actually want to do something
    public class ConversionOptions
    {
        public static readonly List<string> DefaultPaths = new List<string>() {"."};
        public static readonly int DefaultTabWidth = 4;
        public static readonly bool DefaultRecurse = false;
        public static readonly List<string> DefaultIncludeExtensions = new List<string>() {"*"};
        public static readonly List<string> DefaultExcludeExtensions = new List<string>();
        public static readonly List<string> DefaultExcludeFolders = new List<string>();
        public static readonly bool DefaultStripTrailingSpaces = true;

        public ConversionType Type {get; set;}
        public List<string> Paths {get; set;}
        public int TabWidth {get; set;}
        public bool Recurse {get; set;}
        public List<string> IncludeExtensions {get; set;}
        public List<string> ExcludeExtensions {get; set;}
        public List<string> ExcludeFolders {get; set;}
        public bool StripTrailingSpaces {get; set;}
    }
}
