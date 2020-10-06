using System;
using System.IO;
using System.Text;

namespace Labb3
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ReadImageData(out byte[] data, out int fileSize);
                PrintImageData(data, fileSize);
            }

            catch (FileNotFoundException)
            {
                Console.WriteLine("Bilden kunde inte hittas.");
            }
        }
        static void FindPNGChunks(byte[] data, int fileSize)
        {
            int chunkCounter = 1;

            for (int i = 8; i < fileSize; i += 12)
            {
                string chunkHexValues = String.Format($"{data[i]:X02}{data[i + 1]:X02}{data[i + 2]:X02}{data[i + 3]:X02}");
                int chunkDecimalValue = Convert.ToInt32(chunkHexValues.ToString(), 16);
                string chunkType = ASCIIEncoding.ASCII.GetString(data, i + 4, 4);

                Console.WriteLine("----------------");
                Console.WriteLine("Chunk #" + chunkCounter + "\nChunk type: " + chunkType + "\nChunk Length: " + chunkDecimalValue);

                chunkCounter++;
                i += chunkDecimalValue;
            }
        }
        static int ReturnBMPWidth(byte[] data)
        {
            string hexValues = String.Format($"{data[21]:X02}{data[20]:X02}{data[19]:X02}{data[18]:X02}");
            return Convert.ToInt32(hexValues.ToString(), 16);
        }
        static int ReturnBMPHeight(byte[] data)
        {
            string hexValues = String.Format($"{data[25]:X02}{data[24]:X02}{data[23]:X02}{data[22]:X02}");
            return Convert.ToInt32(hexValues.ToString(), 16);
        }
        static int ReturnPNGHeight(byte[] data)
        {
            string hexValues = String.Format($"{data[20]:X02}{data[21]:X02}{data[22]:X02}{data[23]:X02}");
            return Convert.ToInt32(hexValues.ToString(), 16);
        }
        static int ReturnPNGWidth(byte[] data)
        {
            string hexValues = String.Format($"{data[16]:X02}{data[17]:X02}{data[18]:X02}{data[19]:X02}");
            return Convert.ToInt32(hexValues.ToString(), 16);
        }
        static void PrintImageData(byte[] data, int fileSize)
        {
            string pngIdentifier = "89504E47";
            string bmpIdentifier = "424D";
            string checkHeader = String.Format($"{data[0]:X02}{data[1]:X02}{data[2]:X02}{data[3]:X02}");

            if (checkHeader == pngIdentifier)
            {
                Console.WriteLine("Format: PNG");
                Console.WriteLine($"Upplösning: {ReturnPNGWidth(data)}x{ReturnPNGHeight(data)}");
                Console.WriteLine($"Storlek: {fileSize} bytes");
                FindPNGChunks(data, fileSize);
            }
            else if (checkHeader.Contains(bmpIdentifier))
            {
                Console.WriteLine("Format: BMP");
                Console.WriteLine($"Upplösning: {ReturnBMPWidth(data)}x{ReturnBMPHeight(data)}");
                Console.WriteLine($"Storlek: {fileSize} bytes");
            }
            else
            {
                Console.WriteLine("Inget giltigt bildformat.");
            }
        }
        static void ReadImageData(out byte []data, out int fileSize)
        {
            Console.Write("Ange sökväg: ");
            var fs = new FileStream(Console.ReadLine(), FileMode.Open);
            fileSize = (int)fs.Length;
            data = new byte[fileSize];
            fs.Read(data, 0, fileSize);
            fs.Close();
        }
    }
}