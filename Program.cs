using System;
using System.IO;

namespace support_conversation_converter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0) {
                Console.WriteLine("Drag and drop a text file or a text-based support binary (for Fire Emblem: Three Houses) on this executable.");
                Console.ReadKey();
                Environment.Exit(-1);
            }

            string filepath = Path.GetFullPath(Convert.ToString(args[0]));

            if (!File.Exists(filepath)) {
                Console.WriteLine("Please provide a filepath to an existing file.");
                Console.ReadKey();
                Environment.Exit(-1);
            }

            switch (Path.GetExtension(filepath))
            {
                case ".txt":
                    Converter.TextToBinary(filepath);
                    break;
                case ".bin":
                    Converter.BinaryToText(filepath);
                    break;
                default:
                    Console.WriteLine("Please provide a valid file format (*.txt, *.bin).");
                    break;
            }
        }
    }
}
