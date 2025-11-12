using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Program
{
    public static class Camera
    {
        // Camera Position is synced with Player
        public static Vector3 Position = Player.Position;

        // ViewAngle.X = horizontal yaw (left/right)
        // ViewAngle.Y = vertical pitch (up/down)
        public static Vector2 ViewAngle = Vector2.Zero;

        // FOV in degrees
        public static float Fov = 90f;

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
        public static int renderDistance = 3;

        public static Matrix View =>
            Matrix.CreateLookAt(
                Position,
                Position + Forward,
                Up
            );

        public static Matrix Projection =>
            Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(Fov),
                Screen.Aspect,
                0.1f,
                2000f
            );

        public static void Update(float dt)
        {
            Position = Player.Position;
            // Sync camera to player every frame
            // If mouse look code exists, modify ViewAngle here
        }

        public static int[][] GetChunksInView(int[][] chunks)
        {
            List<int[]> visible = new List<int[]>();

            Vector3 camPos = Position;
            float yaw = ViewAngle.X;
            float pitch = ViewAngle.Y;
            float halfHFov = MathHelper.ToRadians(Fov * 0.5f);
            float halfVFov = MathHelper.ToRadians(Fov * 0.5f); // or use a different vertical FOV if desired

            foreach (var c in chunks)
            {
                Vector3 basePos = new Vector3(c[0] * 16f, c[1] * 16f, c[2] * 16f);
                bool inView = false;

                for (int xi = 0; xi <= 1 && !inView; xi++)
                {
                    for (int yi = 0; yi <= 1 && !inView; yi++)
                    {
                        for (int zi = 0; zi <= 1 && !inView; zi++)
                        {
                            Vector3 corner = basePos + new Vector3(xi * 16f, yi * 16f, zi * 16f);
                            Vector3 rel = corner - camPos;

                            // Get yaw and pitch to this vertex from camera
                            float dirYaw = (float)Math.Atan2(rel.X, rel.Z);
                            float dirPitch = (float)Math.Atan2(rel.Y, Math.Sqrt(rel.X * rel.X + rel.Z * rel.Z));

                            // Angle differences
                            float dYaw = MathHelper.WrapAngle(dirYaw - yaw);
                            float dPitch = MathHelper.WrapAngle(dirPitch - pitch);

                            // If within FOV horizontally and vertically -> visible
                            if (Math.Abs(dYaw) <= halfHFov && Math.Abs(dPitch) <= halfVFov)
                            {
                                inView = true;
                                break;
                            }
                        }
                    }
                }

                if (inView)
                    visible.Add(c);
            }

            return visible.ToArray();
        }




    }
}
