[![Build status](https://ci.appveyor.com/api/projects/status/f9ybv32qo9klq7xd?svg=true)](https://ci.appveyor.com/project/petedishman/whitespace)

# Whitespace
 
A small utility to manage whitespace in a group of files.
It can convert leading whitespace to either tabs or spaces, normalize line endings to either \r\n or \n, and trim any trailing whitespace.

## Options

```
Usage: Whitespace [options]

Options:
  --help                   Show help information
  --version                Show version information
  --path                   file paths to search for files in
  --file                   a specific file to process
  --list                   a text file containing the filenames to process
  --indent                 spaces, tabs or leave (default=leave)
  --tabwidth               number of spaces per tab (default=4)
  --recurse                recurse through sub-folders when finding files (default=false)
  --include                file extensions to include, e.g. --include=cpp --include=c,cpp,h,hpp (default=<all>)
  --exclude                file extensions to exclude (default=<none>)
  --exclude-folders        exclude folders (default=<none>)
  --strip-trailing-spaces  strip trailing whitespace from end of lines (default=false)
  --line-endings           convert line endings to crlf|lf (default=leave alone)
  --dry-run                just show files that would be changed but don't do anything
  --verbose                Show all files inspected and generally more information```
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

You can specify files to process in 3 different ways.

Path(s) to search through for particular file extensions. Specific folders and extensions can also be excluded.

By default, all files will be processed, but you can control the file extensions processed with `--include` and `--exclude`  
Multiple extensions can be specified either comma separated or with multiple arguments, i.e. `--include=cpp,c` or `--include=cpp --include=c`

Folders under the given path can also be excluded using `--exclude-folders`  
i.e. `--exclude-folders=node_modules` will exclude any folders under the given path that match `node_modules`

e.g.

```
--path=project\src --path=project\tests --include=cs,cshtml --exclude-folders=bin --recurse
```

Specific files to process
i.e.

```
--file=file1.cpp --file=file2.cpp --file=main.cpp
```

By passing in the filename of a file that contains a list of filenames to process, one per line.
i.e.

```
--list=filenames.txt
```

where filenames.txt contains something like:

```
c:\build\myproject\file1.cpp
c:\build\myproject\file1.h
c:\build\myproject2\main.cpp
```

#### Dry Run

Specifying `--dry-run` will cause whitespace to just list out the files it would actually change without actually doing anything.

#### Verbose

Specifying `--verbose` causes whitespace to log out extra information, including all of the files examined.
It will then state which files were changed and which are left untouched.
