using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace ImageDrafter
{
    class Program
    {
        static string HelpMessage =
@"Image Drafter Utility for MineDrafter Mod (MineTest): Converts a B/W or Colored Image into Draft format.
General format: imagedrafter filepath mode (output)
Use command line arguments to specify:
    - filepath: path to source file
    - mode: either `outline` or `fill`, affects how the program interprets color
    - output: (optional) output file name, file will be saved in the same folder as source image
        will overwrite any existing file without warning
";

        static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine(HelpMessage);
            else
                Process(args);
        }

        private static void Process(string[] args)
        {
            string filePath = args[0];
            string mode = args[1];
            // Check file exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File doesn't exist: {filePath}");
                return;
            }
            // Process file
            switch (mode)
            {
                // Treat file as greyscale
                case "outline":
                    Bitmap image = new Bitmap(filePath);
                    StringBuilder builder = new StringBuilder("--0 clear\n--1 keep\n");
                    // Loop through the images pixels
                    for(int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            Color color = image.GetPixel(x, y);
                            if ((color.R + color.G + color.B) / 3 >= 255 / 2)
                                builder.Append('0');
                            else
                                builder.Append('1');
                        }
                        builder.AppendLine();
                    }
                    // Get new file name
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string newFilename = $"{fileName}_output.txt";
                    if (args.Length == 3)
                        newFilename = $"{Path.GetFileNameWithoutExtension(args[2])}.txt";
                    string newFilePath = Path.Combine(Path.GetDirectoryName(filePath) + newFilename);
                    // Append additional information
                    builder.AppendLine($"# Width: {image.Width}; Height: {image.Height}");
                    builder.AppendLine($"# Use `/draft -{image.Width/2} {image.Height/2} 1 1 {Path.GetFileNameWithoutExtension(newFilename)}` to put the pattern centered around player.");
                    // Output
                    File.WriteAllText(newFilePath, builder.ToString());
                    break;
                case "fill":
                    Console.WriteLine($"Mode `{mode}` is not implemented yet.");
                    break;
                default:
                    Console.WriteLine($"Mode `{mode}` is not supoorted.");
                    break;
            }
        }
    }
}
