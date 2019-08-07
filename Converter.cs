using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace support_conversation_converter
{
    public class Converter
    {
        public static void BinaryToText(string filepath)
        {
            TextSupportBinary binary = new TextSupportBinary();
            binary.Load(filepath);
            binary.ExportAsText(filepath);
        }

        public static void TextToBinary(string filepath)
        {
            TextSupportBinary binary = new TextSupportBinary();

            foreach (string line in File.ReadLines(filepath))
                binary.addLine(Encoding.GetEncoding("ISO-8859-15").GetString(Encoding.UTF8.GetBytes(line)));

            binary.ExportAsBinary(filepath);
        }
    }
}
