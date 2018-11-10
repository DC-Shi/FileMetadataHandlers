# Change file or directory time-metadata
When moving files or dirs across different drives, many metadata would be changed by default.
- Directory modified time: seems unchanged using windows move/copy. Changed using other utilities.
- Directory created time: changed to current time due to copying/moving is actually create a new folder in destination.
- Directory accessed time: if you add/remove files in this folder, it set to current time. By default, after dest folder is created, it copies files into folder, so accessed time changed as well.
- File modified time: seems unchanged.
- File created time: unchanged when using windows move/copy. But changed using link-shell-extension tools.
- File accessed time: changed to current time.

Copying under Linux with `cp -a` seems keeps file modified time.

link-shell-extension "smart copy":  modified time keeps, file created time and file accessed time are changed. Folder time keeps.
normal windows copy: file create time and modified keeps, accessed time changed. Folder create time keeps, other two modified.

This program compares folders in from.txt and folders in to.txt . If there have same children in folders, it tries to change time to the oldest.

# Usage
`LastAccessTimeChange.exe` would print a list of what will be changed.

`LastAccessTimeChange.exe -a` would print the list and apply the timestamp modification.