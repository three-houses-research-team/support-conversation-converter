using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using support_conversation_converter.IO;

namespace support_conversation_converter
{
    public class TextSupportBinary
    {
        List<string> lines = new List<string>();

        public void Load(string filepath)
        {
            using (EndianBinaryReader reader = new EndianBinaryReader(filepath, Endianness.Little))
            {
                /* Header */
                reader.SeekBegin(0x8); // Skip the first two unknowns
                uint offset_table_pos = reader.ReadUInt32();
                uint offset_table_size = reader.ReadUInt32();
                uint offset_count = reader.ReadUInt32(); // This does not account for the very first offset as it is always 0

                reader.SeekBegin(offset_table_pos); // Skip the padding

                /* Offset table */
                List<uint> offset_list = new List<uint>();

                for(int i = 0;i < offset_count;i++) // Take the very first offset into account too
                {
                    offset_list.Add(reader.ReadUInt32());
                }

                reader.SeekBegin(offset_table_pos + offset_table_size); // Skip the padding and jump to the string table

                long string_table_pos = reader.Position;

                foreach(uint offset in offset_list)
                {
                    lines.Add(reader.ReadStringAtOffset(string_table_pos + offset, StringBinaryFormat.NullTerminated)); // Move in front of the string to read it
                }
            }
        }

        public void addLine(string line)
        {
            lines.Add(line);
            Console.WriteLine(line);
        }

        public void ExportAsText(string filepath)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(Path.GetFileNameWithoutExtension(filepath) + ".txt", FileMode.Create), Encoding.GetEncoding("ISO-8859-15")))
            {
                foreach (string line in lines)
                {
                    writer.WriteLine(line.Replace(System.Convert.ToChar(0x0A), '#'));
                }
            }
        }

        public void ExportAsBinary(string filepath)
        {
            using (EndianBinaryWriter writer = new EndianBinaryWriter(new FileStream(Path.GetFileNameWithoutExtension(filepath) + "_edit.bin", FileMode.Create), Encoding.GetEncoding("ISO-8859-15"), Endianness.Little))
            {
                /* Header */
                writer.Write((uint)1);
                writer.Write((uint)1);
                writer.Write((uint)0x20); // The offset table is always located at this position
                writer.Write((uint)((lines.Count * 0x4) + ((lines.Count * 0x4) % 0x10))); // Compute the size of the offset table + padding. (Yes this is terrible, deal with it)
                writer.Write((uint)(lines.Count));
                writer.WriteAlignmentPadding(0x10);

                /* Offset table */
                uint relative_offset = 0;
                List<uint> offsets = new List<uint>();

                foreach (string line in lines)
                {
                    offsets.Add(relative_offset);
                    relative_offset += (uint)line.Length + 1; // Account for the null-terminator
                }

                foreach (uint offset in offsets)
                {
                    writer.Write((uint)offset);

                }
                writer.Write((uint)relative_offset); // The very last offset is the end of the file

                writer.WriteAlignmentPadding(0x10);

                // String table
                foreach (string line in lines)
                {
                    writer.Write(line.Replace('#', System.Convert.ToChar(0x0A)), StringBinaryFormat.FixedLength, line.Length);
                    writer.Write((byte)0); //Add a null-terminator
                }
            }
        }
    }
}
