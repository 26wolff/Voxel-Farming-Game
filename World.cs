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
            int[] binData = GetChunkBin("w-1", "0_0", out id);
            Chunk tc = formatChunkData(id,binData);
        }
        public static Chunk formatChunkData(string id, int[] de)
        {
            int[,,] formed = { };

            // Do dec to chunk conversion //

            return new Chunk(id, formed);
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
        public int[,,] data { get; set; }
        public Chunk(string Id, int[,,] Data)
        {
            id = Id;
            data = Data;
        }
    }
}