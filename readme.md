[![Build status](https://ci.appveyor.com/api/projects/status/f9ybv32qo9klq7xd?svg=true)](https://ci.appveyor.com/project/petedishman/whitespace)

# Whitespace
 
A small utility to manage whitespace in a group of files.
It can convert leading whitespace to either tabs or spaces, normalize line endings to either \r\n or \n, and trim any trailing whitespace.

## Options

```
Usage: Whitespace [arguments] [options]

Arguments:
  path  the file path containing files to process

Options:
  -h|--help                   Show help information
  --version                   Show version information
  --indent                    spaces, tabs or leave (default=leave)
  -t|--tabwidth               number of spaces per tab (default=4)
  -r|--recurse                recurse through sub-folders when finding files (default=false)
  -i|--include                file extensions to include, e.g. --include=cpp --include=c,cpp,h,hpp (default=<all>)
  -e|--exclude                file extensions to exclude (default=<none>)
  -x|--exclude-folders        exclude folders (default=<none>)
  -s|--strip-trailing-spaces  strip trailing whitespace from end of lines (default=false)
  -l|--line-endings           convert line endings to crlf|lf (default=leave alone)
  -d|--dry-run                just show files that would be changed but don't do anything
```

#### Indentation Style

Whitespace can change leading whitespace to be tabs or spaces. 
It will cope with switching one to the other or fixing a mixture of both tabs and spaces.

Specify `--indent=tabs`, `--indent=spaces` or `--indent=leave`

When changing leading whitespace you should specify the desired/expected tab-size (default=4).  
When converting spaces to tabs, a tab width of N means N spaces are converted to 1 tab.  
And when converting tabs to spaces, 1 tab is converted to N spaces. 

#### Trailing whitespace

If `--strip-trailing-spaces` is specified, any tabs or spaces at the end of a line will be removed.

#### Line Endings

Whitespace can normalize line endings to be either \r\n or \n, or leave them as they are.

Specify `--line-endings=crlf` or `--line-endings=lf` or exclude the option to leave alone.

#### What files to process

The first argument to Whitespace should be the path(s) to process. 
Whitepsace will then look for all files under those paths, including sub-folders if `--recurse` is specified.

By default, all files will be processed, but you can control the file extensions processed with `--include` and `--exclude`  
Multiple extensions can be specified either comma separated or with multiple arguments, i.e. `--include=cpp,c` or `--include=cpp --include=c`

Folders under the given path can also be excluded using `--exclude-folders`  
i.e. `--exclude-folders=node_modules` will exclude any folders under the given path that match `node_modules`

#### Dry Run

Specifying `--dry-run` will cause whitespace to just list out the files it would attempt to change without actually doing anything.
It's useful for verifying that your set of paths, and include/exclude extensions are correct.