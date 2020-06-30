# CleanUpTool

Simple tool to delete files or folders older then [specified days]
```
Usage: CleanUpTool.exe [OPTIONS]+ path
delete files or folders older then x days

Options:
  -m, --mode=MODE            operation mode (MODE= file or folder)
  -f, --filter=FILTER        Wildcard FILTER (e.g. '*.pdf' for all pdf files
                               or '*' for all folders)
  -d, --days=VALUE           days
      --debug                debug mode
  -h, --help                 show this help
```

Examples:

Deleting all pdf files older then 4 days in C:\Temp

```CleanUpTool.exe --mode=file -d=4 -f=*.pdf C:\Temp```

Deleting all folders (including content) older then 7 days in C:\Trash

```CleanUpTool.exe --mode=folder -d=7 C:\Trash```
