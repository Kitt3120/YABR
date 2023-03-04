using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace YABR;

public class Program
{
    public static void Main(string[] args)
    {
        //Get path from args or use current working directory by default
        string path;
        if (args.Length == 0)
            path = Directory.GetCurrentDirectory();
        else
            path = args[0];

        //Get regex filter if defined as second arg
        string? regexFilter = null;
        if (args.Length > 1)
            regexFilter = args[1];


        //Make sure path exists
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Directory {path} was not found. Please provide a valid path.");
            return;
        }

        //Get possible relative path as full path and get rid of trailing separator chars
        path = Path.GetFullPath(path);
        while (path.EndsWith(Path.DirectorySeparatorChar))
            path = path[..^1];
        string[] files = Directory.GetFiles(path, string.Empty, SearchOption.TopDirectoryOnly)
                                    .Select(file => Path.GetFileName(file))
                                    .ToArray();

        //Apply regex filter on file selection if defined
        if (regexFilter is not null)
            files = files.Where(file => Regex.Match(file, regexFilter).Success).ToArray();

        //Order files alphabetically
        files = files.Order().ToArray();

        //Interactive menu for sorting files
        bool isSorting = true;
        while (isSorting)
        {
            Console.Clear();
            for (int i = 0; i < files.Length; i++)
                Console.WriteLine($"{i + 1}: {files[i]}");

            Console.WriteLine("\n[A] -> Sort alphabetically\n[D] -> Done\n[M] [id] [id] -> Move entry\n[R] -> Reverse order\n[S] [id] [id] -> Swap entries");

            string[] input = Console.ReadLine()?.Split(" ") ?? Array.Empty<string>();
            switch (input[0].ToUpper())
            {
                case "A":
                    files = files.Order().ToArray();
                    break;
                case "D":
                    isSorting = false;
                    break;
                case "M":
                    if (input.Length < 3)
                    {
                        Console.WriteLine("Please specifiy two indexes for moving.");
                        Console.ReadKey();
                        break;
                    }

                    if (!int.TryParse(input[1], out int entryIndex))
                    {
                        Console.WriteLine($"Invalid position: {input[1]}");
                        Console.ReadKey();
                        break;
                    }

                    if (!int.TryParse(input[2], out int targetIndex))
                    {
                        Console.WriteLine($"Invalid position: {input[1]}");
                        Console.ReadKey();
                        break;
                    }

                    entryIndex--;
                    targetIndex--;

                    if (entryIndex < 0 || entryIndex >= files.Length)
                    {
                        Console.WriteLine($"Position is out of bounds: {entryIndex + 1}. Valid range: 0 - {files.Length}");
                        Console.ReadKey();
                        break;
                    }

                    if (targetIndex < 0 || targetIndex >= files.Length)
                    {
                        Console.WriteLine($"Position is out of bounds: {entryIndex + 1}. Valid range: 0 - {files.Length}");
                        Console.ReadKey();
                        break;
                    }

                    if (entryIndex == targetIndex)
                    {
                        Console.WriteLine("You're funny.");
                        Console.ReadKey();
                        break;
                    }

                    string entryToMove = files[entryIndex];

                    for (int index = entryIndex; index < targetIndex; index++)
                        files[index] = files[index + 1];

                    for (int index = entryIndex; index > targetIndex; index--)
                        files[index] = files[index - 1];

                    files[targetIndex] = entryToMove;
                    break;
                case "R":
                    files = files.Reverse().ToArray();
                    break;
                case "S":
                    if (input.Length < 3)
                    {
                        Console.WriteLine("Please specifiy two indexes to swap.");
                        Console.ReadKey();
                        break;
                    }

                    if (!int.TryParse(input[1], out int swapFromIndex))
                    {
                        Console.WriteLine($"Invalid position: {input[1]}");
                        Console.ReadKey();
                        break;
                    }

                    if (!int.TryParse(input[2], out int swapToIndex))
                    {
                        Console.WriteLine($"Invalid position: {input[1]}");
                        Console.ReadKey();
                        break;
                    }

                    swapFromIndex--;
                    swapToIndex--;

                    if (swapFromIndex < 0 || swapFromIndex >= files.Length)
                    {
                        Console.WriteLine($"Position is out of bounds: {swapFromIndex + 1}. Valid range: 0 - {files.Length}");
                        Console.ReadKey();
                        break;
                    }

                    if (swapToIndex < 0 || swapToIndex >= files.Length)
                    {
                        Console.WriteLine($"Position is out of bounds: {swapFromIndex + 1}. Valid range: 0 - {files.Length}");
                        Console.ReadKey();
                        break;
                    }

                    if (swapFromIndex == swapToIndex)
                    {
                        Console.WriteLine("You're funny.");
                        Console.ReadKey();
                        break;
                    }

                    (files[swapFromIndex], files[swapToIndex]) = (files[swapToIndex], files[swapFromIndex]);
                    break;
                default:
                    break;
            }
        }

        //Ask for the naming scheme until a valid scheme has been provided
        string namingScheme = string.Empty;
        while (!namingScheme.Contains("{i}"))
        {
            Console.Write("New file naming scheme (use {i} for index variable): ");
            namingScheme = Console.ReadLine() ?? string.Empty;
        }

        //Preview renaming
        Console.WriteLine("\nPreview renaming:");
        int longestFileName = files.Max(file => file.Length);
        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            int nameLengthDifference = longestFileName - file.Length;
            string remainingSpaces = new string(' ', (int)Math.Round(nameLengthDifference / 5.0) * 5);
            Console.WriteLine($"{file}{remainingSpaces}\t->\t{namingScheme.Replace("{i}", (i + 1).ToString("00"))}");
        }

        //Confirm renaming and abort if user wants to cancel
        Console.Write("Is this correct? [y/N]: ");
        string confirmation = Console.ReadLine() ?? string.Empty;
        if (confirmation.ToLower() != "y")
        {
            Console.WriteLine("Aborted.");
            return;
        }

        //Rename files
        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            File.Move(path + Path.DirectorySeparatorChar + file, path + Path.DirectorySeparatorChar + namingScheme.Replace("{i}", (i + 1).ToString("00")));
        }
    }
}