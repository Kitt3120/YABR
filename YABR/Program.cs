using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace YABR;

public class Program
{
    public static void Main(string[] args)
    {
        string path;
        if (args.Length == 0)
            path = Directory.GetCurrentDirectory();
        else
            path = args[0];

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
    }
}