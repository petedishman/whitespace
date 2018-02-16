using System.Collections.Generic;
namespace Whitespace
{
    public class ConversionOptions
    {
        public static readonly List<string> DefaultPaths = new List<string>();
        public static readonly IndentationStyle DefaultIndentationStyle = IndentationStyle.Leave;
        public static readonly int DefaultTabWidth = 4;
        public static readonly List<string> DefaultIncludeExtensions = new List<string>() {"*"};
        public static readonly List<string> DefaultExcludeExtensions = new List<string>();
        public static readonly List<string> DefaultExcludeFolders = new List<string>();
        public static readonly bool DefaultStripTrailingSpaces = true;
        public static readonly LineEnding DefaultLineEndingStyle = LineEnding.Leave;

        public IndentationStyle Indentation { get; set; } = DefaultIndentationStyle;
        public List<string> Paths { get; set; } = DefaultPaths;
        public int TabWidth { get; set; } = DefaultTabWidth;
        public bool Recurse { get; set; } = false;
        public List<string> IncludeExtensions { get; set; } = DefaultIncludeExtensions;
        public List<string> ExcludeExtensions { get; set; } = DefaultExcludeExtensions;
        public List<string> ExcludeFolders { get; set; } = DefaultExcludeFolders;
        public bool StripTrailingSpaces { get; set; } = DefaultStripTrailingSpaces;
        public LineEnding LineEndingStyle { get; set; } = DefaultLineEndingStyle;
        public bool DryRun { get; set; } = false;
    }
}
