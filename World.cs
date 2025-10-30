using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Program
{
    public static class World
    {
        public static List<Chunk> ChunkStorage = new List<Chunk>();
        public static Dictionary<int[], int> Indexies = new Dictionary<int[], int>();

        private static string heldPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "VoxelFarm",
            "Data",
            "World"
        );

        public static void Init()
        {

            Console.WriteLine(LoadChunk(0, 0, 0));
            foreach (int[] key in Indexies.Keys)
            {
                Console.WriteLine(String.Join(",", key));
                Console.WriteLine(Indexies[key]);
                Console.WriteLine(ChunkStorage[Indexies[key]]);
            }
            GetChunkData(0, 0, 0);
        }
        public static int[][][] GetChunkData(int cx,int cy,int cz)
        {
            if(Indexies.TryGetValue([cx,cy,cz], out int spare)) // FAIL HERE _________________
            {
                // Exists, writing over it
                Console.WriteLine("PASS");
                return ChunkStorage[spare].data;
                
            }
            else
            {
                // Does not exist, adding to it
                Console.WriteLine("CHUNK FAILED");
                return GetEmptyChunk();
            }

        }
        public static bool LoadChunk(int cx,int cy,int cz, string? WorldOveride = null)
        {
            if (WorldOveride == null) WorldOveride = Player.World;
            int[][][] Data = FormatChunkData(WorldOveride, $"{cx}_{cy}_{cz}");
            Chunk New = new Chunk([cx, cy, cz], Data);

            if(Indexies.TryGetValue([cx,cy,cz], out int spare))
            {
                // Exists, writing over it
                ChunkStorage[spare] = New;
            }
            else
            {
                // Does not exist, adding to it
                Indexies.Add([cx, cy, cz], ChunkStorage.Count);
                ChunkStorage.Add(New);
            }
            
            return true;
        }
        public static int[][][] FormatChunkData(string world, string chunk)
        {
            string dataPath = Path.Combine(heldPath, world, "Chunks", $"{chunk}.bin");
            if (!File.Exists(dataPath))
            {
                Console.WriteLine($"Error: File not found at '{dataPath}'");
                return GetEmptyChunk();
            }

            byte[] bn = File.ReadAllBytes(dataPath);
            const int ROW_LEN = 16;

            // formed[x][y][z] where:
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
            

            return formed;
        }

        public static void LogChunk(int[][][] data)
        {
            for (int yy = 0; yy < 16; yy++)
            {
                for (int xx = 0; xx < 16; xx++)
                {
                    for (int zz = 0; zz < 16; zz++)
                    {
                        Console.Write($"{data[xx][yy][zz],4}");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("------------------------------------------------");
            }
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
        public int[] id { get; set; }
        public int[][][] data { get; set; }
        public Chunk(int[] Id, int[][][] Data)
        {
            id = Id;
            data = Data;
        }
    }
}