using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Program
{
    public static class DrawManager
    {
        private static SpriteBatch? _spriteBatch;
        private static GraphicsDevice? _graphicsDevice;
        private static Texture2D? _lineTex;
        private static bool rep = true;

        public static readonly Vector3[] CubeNormals =
        {
            new Vector3(-1, 0, 0), // -X
            new Vector3( 1, 0, 0), // +X
            new Vector3( 0,-1, 0), // -Y
            new Vector3( 0, 1, 0), // +Y
            new Vector3( 0, 0,-1), // -Z
            new Vector3( 0, 0, 1)  // +Z
        };

        private static readonly Vector3[][] FaceTriangles =
        {
            // -X
            new [] { new Vector3(-.5f,-.5f, .5f), new Vector3(-.5f, .5f, .5f), new Vector3(-.5f, .5f,-.5f) },
            new [] { new Vector3(-.5f,-.5f, .5f), new Vector3(-.5f, .5f,-.5f), new Vector3(-.5f,-.5f,-.5f) },

            // +X
            new [] { new Vector3(.5f,-.5f, .5f), new Vector3(.5f, .5f, .5f), new Vector3(.5f, .5f,-.5f) },
            new [] { new Vector3(.5f,-.5f, .5f), new Vector3(.5f, .5f,-.5f), new Vector3(.5f,-.5f,-.5f) },

            // -Y
            new [] { new Vector3(-.5f,-.5f,-.5f), new Vector3(-.5f,-.5f, .5f), new Vector3(.5f,-.5f, .5f) },
            new [] { new Vector3(-.5f,-.5f,-.5f), new Vector3(.5f,-.5f, .5f), new Vector3(.5f,-.5f,-.5f) },

            // +Y
            new [] { new Vector3(-.5f, .5f,-.5f), new Vector3(-.5f, .5f, .5f), new Vector3(.5f, .5f, .5f) },
            new [] { new Vector3(-.5f, .5f,-.5f), new Vector3(.5f, .5f, .5f), new Vector3(.5f, .5f,-.5f) },

            // -Z
            new [] { new Vector3(-.5f,-.5f,-.5f), new Vector3(-.5f, .5f,-.5f), new Vector3(.5f, .5f,-.5f) },
            new [] { new Vector3(-.5f,-.5f,-.5f), new Vector3(.5f, .5f,-.5f), new Vector3(.5f,-.5f,-.5f) },

            // +Z
            new [] { new Vector3(-.5f,-.5f, .5f), new Vector3(-.5f, .5f, .5f), new Vector3(.5f, .5f, .5f) },
            new [] { new Vector3(-.5f,-.5f, .5f), new Vector3(.5f, .5f, .5f), new Vector3(.5f,-.5f, .5f) },
        };

        public static void Init(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _lineTex = new Texture2D(graphicsDevice, 1, 1);
            _lineTex.SetData(new[] { Color.White });
        }

        public static void Draw(bool log = false)
        {
            if (_spriteBatch == null) return;
            
            _graphicsDevice?.Clear(Color.CornflowerBlue);

            int[][] chunks = Camera.Get_Chunks_To_Render(rep);
            Face[] faces = GetFacesIn3dSpace(chunks, rep);
            
            if (rep)
            {
                Console.WriteLine($"Generated {faces.Length} visible faces.");
                rep = false;
            }
            
            _spriteBatch.Begin();

            // Draw each triangle projected to screen
            foreach (var face in faces)
            {
                Vector2 p0 = WorldToScreen(face.V[0]);
                Vector2 p1 = WorldToScreen(face.V[1]);
                Vector2 p2 = WorldToScreen(face.V[2]);

                DrawLine(p0, p1, face.C);
                DrawLine(p1, p2, face.C);
                DrawLine(p2, p0, face.C);
            }

            _spriteBatch.End();
        }

        private static void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            if (_lineTex == null) return;

            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            float length = Vector2.Distance(start, end);
            _spriteBatch!.Draw(_lineTex, start, null, color, angle, Vector2.Zero, new Vector2(length, 1f), SpriteEffects.None, 0);
        }

        private static Vector2 WorldToScreen(Vector3 worldPos)
        {
            // Convert XNA Vector3 to Vector4 for transformation
            Vector4 pos = new Vector4(worldPos, 1f);

            // Transform to view space
            Vector4 viewSpace = Vector4.Transform(pos, Camera.View);

            // Backface culling fix: skip triangles behind the camera
            if (viewSpace.Z > 0) return new Vector2(-1000, -1000);

            // Transform to clip space
            Vector4 clipSpace = Vector4.Transform(viewSpace, Camera.Projection);

            if (clipSpace.W == 0) clipSpace.W = 0.0001f;

            Vector3 ndc = new Vector3(clipSpace.X, clipSpace.Y, clipSpace.Z) / clipSpace.W;

            float sx = (ndc.X + 1f) / 2f * Screen.Width;
            float sy = (1f - ndc.Y) / 2f * Screen.Height;

            return new Vector2(sx, sy);
        }


        public static Face[] GetFacesIn3dSpace(int[][] chunks, bool log)
        {
            List<Face> faces = new List<Face>();

            foreach (var c in chunks)
            {
                Chunk? chunk = World.GetChunkData(c[0], c[1], c[2]);
                if (chunk == null || chunk.empty) continue;

                int baseX = c[0] * 16;
                int baseY = c[1] * 16;
                int baseZ = c[2] * 16;

                for (int x = 0; x < 16; x++)
                    for (int y = 0; y < 16; y++)
                        for (int z = 0; z < 16; z++)
                        {
                            int val = chunk.data[x * 256 + y * 16 + z];
                            if (val == 0) continue;

                            Vector3 center = new Vector3(baseX + x + .5f, baseY + y + .5f, baseZ + z + .5f);

                            for (int i = 0; i < 6; i++)
                            {
                                Vector3 n = CubeNormals[i];
                                int nx = x + (int)n.X;
                                int ny = y + (int)n.Y;
                                int nz = z + (int)n.Z;

                                if (nx >= 0 && nx < 16 && ny >= 0 && ny < 16 && nz >= 0 && nz < 16 &&
                                    chunk.data[nx * 256 + ny * 16 + nz] != 0) continue;

                                if (Vector3.Dot(n, Camera.Position - center) <= 0) continue;

                                float dist = Vector3.Distance(center, Camera.Position);
                                int triIndex = i * 2;
                                Color color;
                                switch (val)
                                {
                                    case 16:
                                        color = Color.Gray;
                                        break;
                                    case 17:
                                        color = Color.Brown;
                                        break;
                                    case 18:
                                        color = Color.DarkGreen;
                                        break;
                                    default:
                                        color = Color.Magenta;
                                        break;
                                }

                                faces.Add(new Face(new[]
                                {
                            FaceTriangles[triIndex][0] + center,
                            FaceTriangles[triIndex][1] + center,
                            FaceTriangles[triIndex][2] + center
                        }, dist, color));

                                faces.Add(new Face(new[]
                                {
                            FaceTriangles[triIndex + 1][0] + center,
                            FaceTriangles[triIndex + 1][1] + center,
                            FaceTriangles[triIndex + 1][2] + center
                        }, dist, color));
                            }
                        }
            }

            if (log) Console.WriteLine($"Faces: {faces.Count}");
            return faces.ToArray();
        }
    }

    public class Face
    {
        public Vector3[] V;
        public float D;
        public Color C;

        public Face(Vector3[] v, float d, Color c)
        {
            V = v;
            D = d;
            C = c;
        }

        public string log() => $"{V[0]} | {V[1]} | {V[2]}";
    }
}
