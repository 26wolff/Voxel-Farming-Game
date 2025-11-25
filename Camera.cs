using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Program
{
    public static class Camera
    {
        // Camera Position is synced with Player
        public static Vector3 Position = Player.Position;

        // ViewAngle.X = yaw, ViewAngle.Y = pitch
        public static Vector2 ViewAngle = Vector2.Zero;

        // FOV in radians
        public static float Fov = 1.75f;
        public static float sqrt2 = (float) Math.Sqrt(2f);

        public static Vector3 Forward
        {
            get
            {
                float yaw = ViewAngle.X;
                float pitch = ViewAngle.Y;
                double cosP = Math.Cos(pitch);

                return Vector3.Normalize(new Vector3(
                    (float)(Math.Sin(yaw) * cosP),
                    (float)(Math.Sin(pitch)),
                    (float)(Math.Cos(yaw) * cosP)
                ));
            }
        }

        public static Vector3 Right => Vector3.Normalize(Vector3.Cross(Forward, Vector3.Up));
        public static Vector3 Up => Vector3.Normalize(Vector3.Cross(Right, Forward));

        public static float renderDistance = 3.5f;

        public static Matrix View =>
            Matrix.CreateLookAt(
                Position,
                Position + Forward,
                Up
            );

        public static Matrix Projection =>
            Matrix.CreatePerspectiveFieldOfView(
                Fov,
                Screen.Aspect,
                0.1f,
                2000f
            );

        public static void Update(float dt)
        {
            Position = Player.Position;
        }

        public static int[][] Cull_Chunks_Not_In_View(int[][] chunks)
        {
                          
            return [];
        }


        public static int[][] Get_Chunks_To_Render(bool log = false)
        {
            Vector3 Pos_Chunk = Position / 16f;

            // FIXED: Z was used twice. Correct order is X,Y,Z
            Vector3Int Curr_Chunk = new Vector3Int(
                (int)Math.Floor(Pos_Chunk.X),
                (int)Math.Floor(Pos_Chunk.Y),
                (int)Math.Floor(Pos_Chunk.Z)
            );

            int Int_Render_Dist = (int)Math.Ceiling(renderDistance);
            float rendeInt_Render_Dist_Sq = renderDistance * renderDistance;

            List<int[]> result = new List<int[]>();
            WorldStorage worldData = World.WorldData;

            // Loop around camera
            for (int ox = -Int_Render_Dist; ox <= Int_Render_Dist; ox++)
            {
                int cx = ox + Curr_Chunk.X;
                if(!Spare.Bound(cx,worldData.XMin,worldData.XMax)) {
                    if(log){
                        Console.WriteLine($"{cx} failed with {worldData.XMin} and {worldData.XMax}");
                    }
                    continue;
                    
                }
                for (int oy = -Int_Render_Dist; oy <= Int_Render_Dist; oy++)
                {
                    int cy = oy + Curr_Chunk.Y;
                    if(!Spare.Bound(cy,worldData.YMin,worldData.YMax)) continue;
                    for (int oz = -Int_Render_Dist; oz <= Int_Render_Dist; oz++)
                    {
                        int cz = oz + Curr_Chunk.Z;
                        if(!Spare.Bound(cz,worldData.ZMin,worldData.ZMax)) continue;

                        float ox2 = (ox+0.5f) * (ox+0.5f);
                        float oy2 = (oy+0.5f) * (oy+0.5f);
                        float oz2 = (oz+0.5f) * (oz+0.5f);

                        // cull out of distance sphere
                        if (ox2 + oy2 + oz2 >= rendeInt_Render_Dist_Sq+2*sqrt2)
                            continue;

                        result.Add(new int[] { cx, cy, cz });
                    }
                }
            }

            if (log)
            {
                Console.WriteLine("-----------------------------------------------");
                foreach (var a in result)
                    Console.WriteLine($"{a[0]}, {a[2]}");

                Console.WriteLine("Total: " + result.Count);
            }

            return result.ToArray();
        }
    }
}
