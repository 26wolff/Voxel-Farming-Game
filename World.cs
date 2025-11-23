using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json;

namespace Program
{
    public static class World
    {
        // storage of loaded chunk objects
        public static List<Chunk> ChunkStorage = new List<Chunk>();
        public static WorldStorage WorldData = new WorldStorage(0, 0, 0, 0, 0, 0);
        public static Dictionary<int[], int> Indexies = new Dictionary<int[], int>(new IntArrayComparer());
        // safe empty chunk: id = {0,0,0}, data empty
        public static Chunk EmptyChunk = new Chunk(new int[] { 0, 0, 0 }, Array.Empty<int>(), true);

        private static string heldPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "VoxelFarm",
            "Data",
            "World"
        );

        public static void Init()
        {
            WorldData = Get_WorldStorage(Player.WorldID);
            Load_World(Player.WorldID);
        }

        public static WorldStorage Get_WorldStorage(string world)
        {
            string dataPath = Path.Combine(heldPath, world, $"{world}.json");
            if (!File.Exists(dataPath))
            {
                throw new FileNotFoundException($"Error: World not found at '{dataPath}'");
            }

            string jsonString = File.ReadAllText(dataPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var TWorldData = JsonSerializer.Deserialize<WorldStorageGetSet>(jsonString, options);
            if (TWorldData != null)
            {
                WorldData = new WorldStorage(
                    TWorldData.XMin,
                    TWorldData.XMax,
                    TWorldData.YMin,
                    TWorldData.YMax,
                    TWorldData.ZMin,
                    TWorldData.ZMax
                );
            }

            return WorldData;
        }

        public static bool Load_World(string world)
        {
            int[][] toLoad = Get_All_Chunk_ID_In_World(WorldData);
            foreach (int[] t in toLoad)
            {
                // ensure t is length 3
                if (t != null && t.Length == 3)
                    Load_Chunk_Into_Array(t[0], t[1], t[2]);
            }
            return true;
        }

        public static int[][] Get_All_Chunk_ID_In_World(WorldStorage wData)
        {
            if (wData == null)
                return Array.Empty<int[]>();

            // compute lengths directly from min/max (defensive)
            int xLen = wData.XMax - wData.XMin + 1;
            int yLen = wData.YMax - wData.YMin + 1;
            int zLen = wData.ZMax - wData.ZMin + 1;

            if (xLen <= 0 || yLen <= 0 || zLen <= 0)
            {
                Console.WriteLine($"Invalid world bounds: X[{wData.XMin}..{wData.XMax}] Y[{wData.YMin}..{wData.YMax}] Z[{wData.ZMin}..{wData.ZMax}]");
                return Array.Empty<int[]>();
            }

            var list = new List<int[]>(xLen * yLen * zLen);

            for (int x = wData.XMin; x <= wData.XMax; x++)
            {
                for (int y = wData.YMin; y <= wData.YMax; y++)
                {
                    for (int z = wData.ZMin; z <= wData.ZMax; z++)
                    {
                        list.Add(new int[] { x, y, z });
                        //Console.WriteLine($"Chunk: {x}, {y}, {z}");
                    }
                }
            }

            Console.WriteLine($"THIS PART count = {list.Count} (expected {xLen} * {yLen} * {zLen} = {xLen * yLen * zLen})");
            return list.ToArray();
        }


        // ---------------------------------------------------------------

        public static Chunk GetChunkData(int cx, int cy, int cz)
        {
            int[] key = new int[] { cx, cy, cz };

            if (Indexies.ContainsKey(key))
            {
                return ChunkStorage[Indexies[key]];
            }
            return EmptyChunk;
        }

        public static bool Load_Chunk_Into_Array(int cx, int cy, int cz)
        {
            // This should only be called on world load
            //Console.WriteLine($"{cx}, {cz} loading");
            string WorldOveride = Player.WorldID;

            // Validate world data lengths
            if (WorldData.XLen <= 0 || WorldData.YLen <= 0 || WorldData.ZLen <= 0) return false;

            Chunk New = Get_Formated_Chunk_Data(WorldOveride, new int[] { cx, cy, cz });

            int[] key = new int[] { cx, cy, cz };

            if (Indexies.TryGetValue(key, out int spare))
            {
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

        public static Chunk Get_Formated_Chunk_Data(string world, int[] chunk)
        {
            string dataPath = Path.Combine(heldPath, world, "Chunks", $"{chunk[0]}_{chunk[1]}_{chunk[2]}.bin");
            if (!File.Exists(dataPath))
            {
                return EmptyChunk;
            }

            byte[] bn = File.ReadAllBytes(dataPath);
            const int ROW_LEN = 16;

            int[] Formed_Data = new int[4096];

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
                    // original formula kept: use x and y from header (0..15)
                    Formed_Data[15 - k + 16 * y + 256 * x] = expanded[k];
                }
            }

            return new Chunk(chunk, Formed_Data, false);
        }
    }

    public class Chunk
    {
        public int[] id { get; set; }
        public int[] data { get; set; } = Array.Empty<int>();
        public bool empty { get; }

        public Chunk(int[] Id, int[] Data, bool Empty)
        {
            empty = Empty;
            id = Id;
            if (!empty && Data != null)
            {
                data = Data;
            }
            else
            {
                data = Array.Empty<int>();
            }
        }
    }

    public class WorldStorage
    {
        public int XMin;
        public int XMax;
        public int YMin;
        public int YMax;
        public int ZMin;
        public int ZMax;
        public int XLen;
        public int YLen;
        public int ZLen;

        public WorldStorage(int xmin, int xmax, int ymin, int ymax, int zmin, int zmax)
        {
            Console.WriteLine($"int {xmin}, int {xmax}, int {ymin}, int {ymax}, int {zmin}, int {zmax}");
            XMin = xmin;
            XMax = xmax;
            YMin = ymin;
            YMax = ymax; // fixed (was xmax)
            ZMin = zmin;
            ZMax = zmax;
            // lengths are inclusive: (max - min + 1)
            XLen = XMax - XMin + 1;
            YLen = YMax - YMin + 1;
            ZLen = ZMax - ZMin + 1;
            if (XLen < 0) XLen = 0;
            if (YLen < 0) YLen = 0;
            if (ZLen < 0) ZLen = 0;
        }
    }

    public class WorldStorageGetSet
    {
        public int XMin { get; set; }
        public int XMax { get; set; }
        public int YMin { get; set; }
        public int YMax { get; set; }
        public int ZMin { get; set; }
        public int ZMax { get; set; }
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
}
