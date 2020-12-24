using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Assembly = Roslyn.Shared.Assembly;

namespace Roslyn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Assembly.Mirror(typeof(Console), typeof(ArrayList), typeof(Thread));
            Assembly.Manifest(GetFiles(args));
            Assembly.Run();
        }

        private static IEnumerable<string> GetFiles(IEnumerable<string> args)
        {
            var files = new List<string>();
            foreach (var s in args)
                GetFiles(new DirectoryInfo(s), files);
            return files;
        }

        private static void GetFiles(DirectoryInfo root, List<string> files)
        {
            files.AddRange(from file in root.GetFiles() where file.Extension == ".cs" select file.FullName);
            foreach (var sub in root.GetDirectories())
                GetFiles(sub, files);
        }
    }
}
