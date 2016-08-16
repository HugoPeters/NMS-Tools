using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NMS_Extract
{
    static class Program
    {
        public static string Credits = "Insomniac (PSARC.exe), Hugo_Peters (the rest)";
        public static string Version = "0.1";

        static void PrintCredits()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("NMS-Extract by {0}, version {1}", Credits, Version);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            PrintCredits();

            var defaultcolor = Console.ForegroundColor;

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: <PCBANKS dir> <outdir> simple huh");
                return 1;
            }

            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("Directory doesn't exist!");
                return 1;
            }

            var dir = new DirectoryInfo(args[0]);

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var psarc = System.IO.Path.GetDirectoryName(assembly.Location);
            psarc += Path.DirectorySeparatorChar + "PSARC.exe";

            Console.WriteLine("PSARC path: " + psarc);

            var fileCount = dir.GetFiles().Length;
            int i = 0;

            foreach (var file in dir.GetFiles())
            {
                i++;

                if (file.Extension != ".pak")
                    continue;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Extracting {0} ({1}/{2})...", file.Name, i, fileCount);
                Console.ForegroundColor = defaultcolor;

                var dirOut = new DirectoryInfo(args[1]);

                Process p = new Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = psarc;
                p.StartInfo.Arguments = "\"" + file.FullName + "\"";
                p.StartInfo.WorkingDirectory = dirOut.FullName;
                p.Start();

                p.WaitForExit();

                var fnameNoExt = Path.GetFileNameWithoutExtension(file.FullName);

                if (p.ExitCode == 0)
                {
                    foreach (var subdir in dirOut.GetDirectories())
                    {
                        if (subdir.Name.Contains(fnameNoExt))
                        {
                            var rootFname = subdir.Name.Replace(fnameNoExt, "");

                            var dirOutFin = dirOut.FullName + Path.DirectorySeparatorChar + rootFname;
                            Directory.CreateDirectory(dirOutFin);

                            // move file contents to root....
                            MoveDirContentsTo(subdir, new DirectoryInfo(dirOutFin));
                        }
                    }
                }
            }

            return 0;
        }

        public static void MoveDirContentsTo(DirectoryInfo dirIn, DirectoryInfo dirOut)
        {
            MoveDirContentsTo(dirIn.FullName, dirOut.FullName);
        }
        public static void MoveDirContentsTo(string source, string target)
        {
            var sourcePath = source.TrimEnd('\\', ' ');
            var targetPath = target.TrimEnd('\\', ' ');

            var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                                 .GroupBy(s => Path.GetDirectoryName(s));

            foreach (var folder in files)
            {
                var targetFolder = folder.Key.Replace(sourcePath, targetPath);
                Directory.CreateDirectory(targetFolder);
                foreach (var file in folder)
                {
                    var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                    if (File.Exists(targetFile)) File.Delete(targetFile);
                    File.Move(file, targetFile);
                }
            }

            Directory.Delete(source, true);
        }
    }
}
