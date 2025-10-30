using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Program
{
    public static class World
    {
        public static List<Chunk> ChunkStorage = new List<Chunk>();

        // Use custom comparer for int[] keys
        public static Dictionary<int[], int> Indexies = new Dictionary<int[], int>(new IntArrayComparer());

        private static string heldPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "VoxelFarm",
            "Data",
            "World"
        );

        public static void Init()
        {
            Console.WriteLine(LoadChunk(0, 0, 0));
            Console.WriteLine(GetChunkData(0, 0, 0)[8][0][8]);
            Console.WriteLine(LoadChunk(1, 0, 0));
            Console.WriteLine(GetChunkData(1, 0, 0)[8][0][8]);

            foreach (int[] key in Indexies.Keys)
            {
                Console.WriteLine(string.Join(",", key));
                Console.WriteLine(Indexies[key]);
                Console.WriteLine(ChunkStorage[Indexies[key]]);
            }
        }

        public static int[][][] GetChunkData(int cx, int cy, int cz)
        {
            int[] key = [cx, cy, cz];

            if (Indexies.ContainsKey(key))
            {
                Console.WriteLine("PASS");
                return ChunkStorage[Indexies[key]].data;
            }
            else
            {
                Console.WriteLine("CHUNK FAILED");
                return GetEmptyChunk();
            }
        }

        public static bool LoadChunk(int cx, int cy, int cz, string? WorldOveride = null)
        {
            if (WorldOveride == null) WorldOveride = Player.World;

            int[][][] Data = FormatChunkData(WorldOveride, $"{cx}_{cy}_{cz}");
            Chunk New = new Chunk([cx, cy, cz], Data);

            int[] key = [cx, cy, cz];

            if (Indexies.TryGetValue(key, out int spare))
            {
                // Exists, writing over it
                ChunkStorage[spare] = New;
            }
            else
            {
                // Does not exist, adding to it
                Indexies.Add(key, ChunkStorage.Count);
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

            int[][][] formed = GetEmptyChunk();

            int pos = 0;
            while (pos < bn.Length)
            {
                byte header = bn[pos++];
                int x = (header >> 4) & 0x0F;
                int y = header & 0x0F;

                List<int> expanded = new List<int>(ROW_LEN);
                int lastBlock = 0;
                bool haveLastBlock = false;

                while (pos < bn.Length)
                {
                    byte token = bn[pos++];

                    if (token == 0xFF)
                        break;

                    if (token == 0xFE)
                    {
                        if (pos >= bn.Length) throw new Exception("Unexpected EOF after 0xFE");
                        int extra = bn[pos++];
                        for (int r = 0; r < extra; r++)
                            expanded.Add(haveLastBlock ? lastBlock : 0);
                        continue;
                    }

                    if (pos >= bn.Length) throw new Exception("Unexpected EOF while expecting meta byte");
                    byte meta = bn[pos++];
                    int blockValue = (token << 4) + meta;
                    expanded.Add(blockValue);
                    lastBlock = blockValue;
                    haveLastBlock = true;
                }

                while (expanded.Count < ROW_LEN) expanded.Add(0);
                if (expanded.Count > ROW_LEN) expanded = expanded.Take(ROW_LEN).ToList();

                for (int k = 0; k < ROW_LEN; k++)
                {
                    formed[15 - k][y][x] = expanded[k];
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

        public static int[][][] GetEmptyChunk()
        {
            int size = 16;
            int[][][] ou = new int[size][][];

            for (int x = 0; x < size; x++)
            {
                ou[x] = new int[size][];
                for (int y = 0; y < size; y++)
                {
                    ou[x][y] = new int[size];
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

    // 👇 Added comparer for int[] dictionary keys
    public class IntArrayComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[]? a, int[]? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null || a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        public int GetHashCode(int[] obj)
        {
            unchecked
            {
                int hash = 17;
                foreach (int val in obj)
                    hash = hash * 23 + val.GetHashCode();
                return hash;
            }
        }
    }

    // Dummy Player class for testing — remove if already defined elsewhere
   
}