using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Program
{
    public static class World
    {
        private static string heldPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "VoxelFarm",
            "Data",
            "World"
        );

        public static void Init()
        {
            Console.WriteLine("T");
            Chunk tc = FormatChunkData("w-1", "0_0_0");
        }

        public static Chunk FormatChunkData(string world, string chunk)
        {
            string dataPath = Path.Combine(heldPath, world, "Chunks", $"{chunk}.bin");
            if (!File.Exists(dataPath))
            {
                Console.WriteLine($"Error: File not found at '{dataPath}'");
                return new Chunk(chunk, GetEmptyChunk());
            }

            byte[] bn = File.ReadAllBytes(dataPath);
            const int ROW_LEN = 16;

            // formed[x][y][z] where:
            // x = left/right (0..15)
            // y = up/down  (0..15)
            // z = forward/back (0..15)
            int[][][] formed = GetEmptyChunk();

            int pos = 0;
            while (pos < bn.Length)
            {
                // Read header byte: high nibble = x, low nibble = y
                byte header = bn[pos++];
                int x = (header >> 4) & 0x0F;
                int y = header & 0x0F;

                // Expand the row into exactly ROW_LEN values (top->bottom)
                List<int> expanded = new List<int>(ROW_LEN);
                int lastBlock = 0;
                bool haveLastBlock = false;

                while (pos < bn.Length)
                {
                    byte token = bn[pos++];

                    if (token == 0xFF)
                    {
                        // End of this row
                        break;
                    }

                    if (token == 0xFE)
                    {
                        if (pos >= bn.Length) throw new Exception("Unexpected EOF after 0xFE");
                        int extra = bn[pos++]; // number of extra duplicates
                        if (!haveLastBlock)
                        {
                            // No previous block: treat repeats as zeros
                            for (int r = 0; r < extra; r++) expanded.Add(0);
                        }
                        else
                        {
                            for (int r = 0; r < extra; r++) expanded.Add(lastBlock);
                        }
                        continue;
                    }

                    // token is type byte; next byte is meta
                    if (pos >= bn.Length) throw new Exception("Unexpected EOF while expecting meta byte");
                    byte meta = bn[pos++];
                    int blockValue = (token << 4) + meta; // packing consistent with writer
                    expanded.Add(blockValue);
                    lastBlock = blockValue;
                    haveLastBlock = true;
                }

                // Normalize length
                while (expanded.Count < ROW_LEN) expanded.Add(0);
                if (expanded.Count > ROW_LEN) expanded = expanded.Take(ROW_LEN).ToList();

                // Map expanded to z axis: expanded[0] -> z=15 (top), expanded[15] -> z=0 (bottom)
                for (int k = 0; k < ROW_LEN; k++)
                {
                    formed[15-k][y][x] = expanded[k];
                }
            }

            // Debug print (optional)
            
            for (int yy = 0; yy < 16; yy++)
            {
                for (int xx = 0; xx < 16; xx++)
                {
                    for (int zz = 0; zz < 16; zz++)
                    {
                        Console.Write($"{formed[xx][yy][zz],4}");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("------------------------------------------------");
            }
            

            return new Chunk(chunk, formed);
        }

        // Correct GetEmptyChunk: returns int[16][16][16] initialized to zeros.
        public static int[][][] GetEmptyChunk()
        {
            int size = 16;
            int[][][] ou = new int[size][][];

            for (int x = 0; x < size; x++)
            {
                ou[x] = new int[size][];
                for (int y = 0; y < size; y++)
                {
                    ou[x][y] = new int[size]; // automatically zeros
                }
            }

            return ou;
        }
    }

    public class Chunk
    {
        public string id { get; set; }
        public int[][][] data { get; set; }
        public Chunk(string Id, int[][][] Data)
        {
            id = Id;
            data = Data;
        }
    }
}