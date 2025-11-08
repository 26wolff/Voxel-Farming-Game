using System;
using System.IO;
using System.Text.Json;
using System.Numerics;
using System.ComponentModel;
using System.Collections.Generic;

namespace Program
{
    public static class Camera
    {
        public static Vector3 Position = Player.Position;
        public static Vector2 ViewAngle = Vector2.Zero;
        public static float Fov = 2f;

        public static float PI = (float)Math.PI;
        public static float TAU = 2 * (float)Math.PI;

        public static void Init() { }

        public static void Update()
        {
            Position = Player.Position;
        }

        public static int[][] GetChunksInVeiw(int[][] chunks)
        {
            List<int[]> valid = new List<int[]>();

            // Determine which chunk the player is standing in
            int pChunkX = (int)Math.Floor(Position.X / 16f);
            int pChunkY = (int)Math.Floor(Position.Y / 16f);
            int pChunkZ = (int)Math.Floor(Position.Z / 16f);

            float camH = ViewAngle.X;
            float camV = ViewAngle.Y;

            float buffer = 0.15f;      // Margin buffer to prevent popping
            float halfFov = Fov / 2f;

            foreach (int[] c in chunks)
            {
                // Force include player's current chunk
                if (c[0] == pChunkX && c[1] == pChunkY && c[2] == pChunkZ)
                {
                    valid.Add(c);
                    continue;
                }

                // Convert chunk coordinate to world-space center
                // (chunkIndex * 16 + 8 = middle of chunk)
                float cx = c[0] * 16 + 8f;
                float cy = c[1] * 16 + 8f;
                float cz = c[2] * 16 + 8f;

                float dx = cx - Position.X;
                float dy = cy - Position.Y;
                float dz = cz - Position.Z;

                // Horizontal angle (left/right)
                float horizontal = (float)Math.Atan2(dx, dz);

                // Vertical angle (up/down)
                float distXZ = (float)Math.Sqrt(dx * dx + dz * dz);
                float vertical = (float)Math.Atan2(dy, distXZ);

                bool inHorizontal = Math.Abs(camH - horizontal) <= (halfFov + buffer);
                bool inVertical   = Math.Abs(camV - vertical)   <= (halfFov + buffer);

                if (inHorizontal && inVertical)
                    valid.Add(c);
            }

            return valid.ToArray();
        }
    }
}