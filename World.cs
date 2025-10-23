using System;
using System.IO;
using System.Runtime.Intrinsics.X86;
using System.Collections.Generic;
using System.Data.Common;

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
            string id;
            int[] binData = GetChunkBin("w-1", "0_0_0", out id);
            //replace with just a returing chunk function
            Chunk tc = formatChunkData(id,binData);
        }
        public static Chunk formatChunkData(string id, int[] de)
        {
            int[][][] formed = getEmptyChunk();

            // Do dec to chunk conversion //
            


            return new Chunk(id, formed);
        }
        public static int[][][] getEmptyChunk()
        {
            int[][][] ou = new int[16][][];
            for (int x = 0; x < 16; x++)
            {
                ou[x] = new int[16][];
                for (int y = 0; y < 16; y++) ou[x][y] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            }
            return ou;
        }
        public static int[] GetChunkBin(string world, string chunk, out string id)
        {
            id = chunk;
            string dataPath = Path.Combine(heldPath, world, "Chunks", $"{chunk}.bin");
            if (!File.Exists(dataPath))
            {
                Console.WriteLine($"Player data file not found: {dataPath}");
                return [];
            }

            byte[] bn = File.ReadAllBytes(dataPath);
            int[] de = new int[bn.Length];
            for (int i = 0; i < bn.Length; i++)
            {
                de[i] = Convert.ToInt32(bn[i]);
            }
            return de;
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