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

            bool dryRun = true;
            if (args.Length == 1 && args[0] == "-a") // Apply the modification, do not dry-run
                dryRun = false;

            if (dryRun)
                Console.WriteLine("-------------------DRY-RUN-PLANS-------------------");
            for (int i = 0; i < fromPaths.Length; i++)
            {
                Compare(fromPaths[i], toPaths[i], dryRun);
            }
        }

        /// <summary>
        /// Modify the target file datetime using functions.
        /// </summary>
        /// <param name="getMethod">Function that can get datetime of one file.</param>
        /// <param name="setMethod">Function that can set datatime with given file path.</param>
        /// <param name="fromPath">From file, orginal file, source file location.</param>
        /// <param name="toPath">To file, duplicated file, target file location.</param>
        /// <param name="dryRun">Whether we are dryrun.</param>
        /// <returns>True if we can/already change the datetime of target file.</returns>
        static bool ModifyTime(Func<string, DateTime> getMethod, Action<string, DateTime> setMethod, string fromPath, string toPath, bool dryRun=true)
        {
            bool modified = false;
            
            // We compare 'to' with 'from', because if 'to' doesn't exist, it will return 1601-01-01, so condition failed as expected.
            if (getMethod(toPath) > getMethod(fromPath))
            {
                Console.WriteLine(fromPath);
                Console.WriteLine("\t{0}\t{1}", getMethod.Method.Name, getMethod(fromPath));
                Console.WriteLine(toPath);
                Console.WriteLine("\t{0}\t{1}", getMethod.Method.Name, getMethod(toPath));

                try
                {
                    if (!dryRun) // dryrun option will not change date.
                        setMethod(toPath, getMethod(fromPath)); // Set the modified time to original one(from).
                    modified = true;
                }
                catch (UnauthorizedAccessException uae)
                {
                    Console.WriteLine("Directory/File access denied, failed.");
                }
                catch (IOException ioe)
                {
                    Console.WriteLine("Directory/File is occupied, failed.");
                }
            }

            return modified;
        }

        // From https://stackoverflow.com/questions/2106877/is-there-a-faster-way-than-this-to-find-all-the-files-in-a-directory-and-all-sub/2107294#2107294
        public static void Compare(string fromDir, string toDir, bool dryRun = true)
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
                changed |= ModifyTime(Directory.GetCreationTime, Directory.SetCreationTime, curSubDir, toSubDir, dryRun);
                // Last write(modified) time
                changed |= ModifyTime(Directory.GetLastWriteTime, Directory.SetLastWriteTime, curSubDir, toSubDir, dryRun);
                // Last access time
                changed |= ModifyTime(Directory.GetLastAccessTime, Directory.SetLastAccessTime, curSubDir, toSubDir, dryRun);

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
                changed |= ModifyTime(File.GetCreationTime, File.SetCreationTime, curSubFile, toSubFile, dryRun);
                
                // Last write(modified) time
                if (File.GetLastWriteTime(toSubFile) > File.GetLastWriteTime(curSubFile))
                {
                    Console.WriteLine(curSubFile);
                    Console.WriteLine("\tmodified\t{0}", File.GetLastWriteTime(curSubFile));
                    Console.WriteLine(toSubFile);
                    Console.WriteLine("\tmodified\t{0}", File.GetLastWriteTime(toSubFile));

                    throw new FileLoadException("The files modify time are different! Does they actually one file?");
                }

                // Creation time
                changed |= ModifyTime(File.GetLastAccessTime, File.SetLastAccessTime, curSubFile, toSubFile, dryRun);

                if (changed)
                    Console.WriteLine("-----------------------------------------");
            }
        }
    }
}
