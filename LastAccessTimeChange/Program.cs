using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastAccessTimeChange
{
    class Program
    {
        static void Main(string[] args)
        {
            // Using from.txt and to.txt to save folder locations for compare.
            var fromPaths = File.ReadAllLines("from.txt");
            var toPaths = File.ReadAllLines("to.txt");

            Trace.Assert(fromPaths.Length == toPaths.Length, "ERROR: from.txt and to.txt must have exact number of folder paths.");

            for (int i = 0; i < fromPaths.Length; i++)
            {
                Compare(fromPaths[i], toPaths[i]);
            }
        }

        // From https://stackoverflow.com/questions/2106877/is-there-a-faster-way-than-this-to-find-all-the-files-in-a-directory-and-all-sub/2107294#2107294
        public static void Compare(string fromDir, string toDir)
        {
            // First compare sub-dirs, this will give the full path
            foreach (var curSubDir in Directory.GetDirectories(fromDir))
            {
                var name = Path.GetFileName(curSubDir);
                var toSubDir = Path.Combine(toDir, name);
                bool changed = false; // one line in a block.
                // We only do iterative check if folders exist in toDir
                if (Directory.Exists(toSubDir))
                {
                    Compare(curSubDir, toSubDir);
                }

                // now curSubDir and toSubDir are checked, so we need to modify themselves.
                // Creation time
                if (Directory.GetCreationTime(toSubDir) > Directory.GetCreationTime(curSubDir))
                {
                    Console.WriteLine(curSubDir);
                    Console.WriteLine("\tCreation\t{0}", Directory.GetCreationTime(curSubDir));
                    Console.WriteLine(toSubDir);
                    Console.WriteLine("\tCreation\t{0}", Directory.GetCreationTime(toSubDir));

                    try
                    {
                        Directory.SetCreationTime(toSubDir, Directory.GetCreationTime(curSubDir)); // Set the creation time to older one(from).
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        Console.WriteLine("Directory access denied, change failed.");
                    }
                    catch (IOException ioe)
                    {
                        Console.WriteLine("Directory is occupied, change failed.");
                    }
                    changed = true;
                }
                // Last write(modified) time
                if (Directory.GetLastWriteTime(toSubDir) > Directory.GetLastWriteTime(curSubDir))
                {
                    Console.WriteLine(curSubDir);
                    Console.WriteLine("\tModified\t{0}", Directory.GetLastWriteTime(curSubDir));
                    Console.WriteLine(toSubDir);
                    Console.WriteLine("\tModified\t{0}", Directory.GetLastWriteTime(toSubDir));

                    try
                    {
                        Directory.SetLastWriteTime(toSubDir, Directory.GetLastWriteTime(curSubDir)); // Set the modified time to older one(from).
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        Console.WriteLine("Directory access denied, change failed.");
                    }
                    catch (IOException ioe)
                    {
                        Console.WriteLine("Directory is occupied, change failed.");
                    }
                    changed = true;
                }
                // Last access time
                if (Directory.GetLastAccessTime(toSubDir) > Directory.GetLastAccessTime(curSubDir))
                {
                    Console.WriteLine(curSubDir);
                    Console.WriteLine("\tAccessed\t{0}", Directory.GetLastAccessTime(curSubDir));
                    Console.WriteLine(toSubDir);
                    Console.WriteLine("\tAccessed\t{0}", Directory.GetLastAccessTime(toSubDir));

                    try
                    {
                        Directory.SetLastAccessTime(toSubDir, Directory.GetLastAccessTime(curSubDir)); // Set the last access time to older one(from).
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        Console.WriteLine("Directory access denied, change failed.");
                    }
                    catch (IOException ioe)
                    {
                        Console.WriteLine("Directory is occupied, change failed.");
                    }
                    changed = true;
                }

                if (changed)
                    Console.WriteLine("-----------------------------------------");
            }

            // Compare each files now.
            foreach (var curSubFile in Directory.GetFiles(fromDir))
            {
                var name = Path.GetFileName(curSubFile);
                var toSubFile = Path.Combine(toDir, name);
                bool changed = false; // one line in a block.

                // Creation time
                if (File.GetCreationTime(toSubFile) > File.GetCreationTime(curSubFile))
                {
                    Console.WriteLine(curSubFile);
                    Console.WriteLine("\tCreation\t{0}", File.GetCreationTime(curSubFile));
                    Console.WriteLine(toSubFile);
                    Console.WriteLine("\tCreation\t{0}", File.GetCreationTime(toSubFile));

                    try
                    {
                        File.SetCreationTime(toSubFile, File.GetCreationTime(curSubFile)); // Set the creation time to older one(from).
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        Console.WriteLine("File access denied, change failed.");
                    }
                    catch (IOException ioe)
                    {
                        Console.WriteLine("File is occupied, change failed.");
                    }
                    changed = true;
                }
                // Last write(modified) time
                if (File.GetLastWriteTime(toSubFile) > File.GetLastWriteTime(curSubFile))
                {
                    Console.WriteLine(curSubFile);
                    Console.WriteLine("\tmodified\t{0}", File.GetLastWriteTime(curSubFile));
                    Console.WriteLine(toSubFile);
                    Console.WriteLine("\tmodified\t{0}", File.GetLastWriteTime(toSubFile));

                    throw new FileLoadException("The files modify time are different! Does they actually one file?");
                }
                // Last access time
                if (File.GetLastAccessTime(toSubFile) > File.GetLastAccessTime(curSubFile))
                {
                    Console.WriteLine(curSubFile);
                    Console.WriteLine("\tAccessed\t{0}", File.GetLastAccessTime(curSubFile));
                    Console.WriteLine(toSubFile);
                    Console.WriteLine("\tAccessed\t{0}", File.GetLastAccessTime(toSubFile));

                    try
                    {
                        File.SetLastAccessTime(toSubFile, File.GetLastAccessTime(curSubFile)); // Set the last access time to older one(from).
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        Console.WriteLine("File access denied, change failed.");
                    }
                    catch (IOException ioe)
                    {
                        Console.WriteLine("File is occupied, change failed.");
                    }
                    changed = true;
                }

                if (changed)
                    Console.WriteLine("-----------------------------------------");
            }
        }
    }
}
