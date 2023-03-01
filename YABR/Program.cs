using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace YABR;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: ./YABR <path/to/directory> [regexFilter]");
            return;
        }

        string path = args[0];
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Directory {path} was not found. Please provide a valid path.");
            return;
        }

        string? regexFilter = null;
        if (args.Length > 1)
            regexFilter = args[1];

        string[] files = Directory.GetFiles(path, string.Empty, SearchOption.TopDirectoryOnly)
                                    .Select(file => file.Replace($"{path}{Path.DirectorySeparatorChar}", string.Empty)
                                                        .Replace($".{Path.DirectorySeparatorChar}", string.Empty))
                                    .ToArray();

        if (regexFilter is not null)
            files = files.Where(file => Regex.Match(file, regexFilter).Success).ToArray();

        files = files.Order().ToArray();

        bool isSorting = true;
        while (isSorting)
        {
            Console.Clear();
            for (int i = 0; i < files.Length; i++)
                Console.WriteLine($"{i}: {files[i]}");

            Console.WriteLine("\n[id] [id] -> Swap entries\n[A] -> Sort alphabetical\n[R] -> Reverse order\n[D] -> Done");

            string input = Console.ReadLine() ?? string.Empty;
            switch (input)
            {
                default:
                    break;
            }
        }
    }
}