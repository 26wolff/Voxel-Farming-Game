using System;
using System.IO;
using System.Runtime.Intrinsics.X86;
using System.Collections.Generic;
using System.Data.Common;
using System.Numerics;

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

            for(int i = 0; i < de.Length; i++)
            {
                Console.Write($"{de[i]} ");
            }

            // Do dec to chunk conversion //
            Vector3 cord = new Vector3(0f, 0f, 0f);
            int typeLast = 0;
            string type = "cord";

            for (int i = 0; i < de.Length; i++)
            {
                int code = de[i];
                if (code == 254 && type != "cord")
                {
                    type = "type";
                    i++;
                    if (cord.Z > 0) cord.Z--;
                    for (int t = 0; t < de[i]; t++)
                    {
                        formed[(int)cord.X][(int)cord.Y][(int)cord.Z] = typeLast;
                        cord.Z++;
                    }
                    continue;

                }
                if (code == 255)
                {
                    type = "cord";
                    typeLast = 0;
                    cord.Z = 0;
                    continue;
                }
                else if (type == "cord")
                {
                    cord.X = (int)Math.Floor((double)(code / 16));
                    cord.Y = code % 16;
                    type = "type";
                    continue;
                }
                if (type == "type")
                {
                    int t_type = 0;
                    if (code != 0)
                    {
                        t_type += code * 16;
                        i++;
                        t_type += de[i];
                    }
                    typeLast = t_type;
                    formed[(int)cord.X][(int)cord.Y][(int)cord.Z] = t_type;
                    cord.Z++;
                }
            }

            // for (int y = 0; y < 16; y++)
            // {
            //     for (int x = 0; x < 16; x++)
            //     {
            //         for (int z = 0; z < 16; z++)
            //         {
            //             int t = formed[z][y][x];
            //             string efuck = "";
            //             if (t > 0) 
            //             {
            //                 efuck = $"{t - 15}";
            //             }
            //             else if(t == 0) {
            //                 efuck = " "; 
            //             }
                        
            //             Console.Write(efuck);
            //             Console.Write("  ");
            //         }
            //         Console.WriteLine();
            //     }
            //     Console.WriteLine("-------------------------------------------------");
            // }

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