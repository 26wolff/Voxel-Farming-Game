using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Program
{
    public static class DrawManager
    {
        private static SpriteBatch? _spriteBatch;
        private static GraphicsDevice? _graphicsDevice;
        private static bool rep = true;
        public static int[][] CubeNormals = [[-1, 0, 0], [1, 0, 0], [0, -1, 0], [0, 1, 0], [0, 0, -1], [0, 0, 1]];

        public static void Init(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
        }

        public static void Draw()
        {
            int[][] WorldToRender = World.GetChunksToRender();
            int[][] ToRender = Camera.GetChunksInVeiw(WorldToRender);
            (Vector3[] Faces3dSpace, Color[] FaceColors) = GetFacesIn3dSpace(ToRender, rep);

            //Get a list of not empty chunks that i am in veiw of
            if (rep)
            {



                Console.WriteLine("DREW");
                foreach (var c in WorldToRender)
                {
                    Console.WriteLine($"{c[0]},{c[2]}");
                }
                Console.WriteLine("DREW2");
                foreach (var c in ToRender)
                {
                    Console.WriteLine($"{c[0]},{c[2]}");
                }
                Console.WriteLine();





                rep = false;
            }

            // Example: draw a red square
            _spriteBatch!.Begin();
            var tex = CreateTexture(Color.Red);
            _spriteBatch.Draw(tex, new Rectangle((int)(Player.Position.X * Screen.Width), (int)(-Player.Position.Z * Screen.Height), 64, 64), Color.White);
            _spriteBatch.End();
        }

        public static Texture2D CreateTexture(Color color)
        {
            // if graphics device is still null, try to grab it from the spritebatch
            if (_graphicsDevice == null && _spriteBatch != null)
                _graphicsDevice = _spriteBatch.GraphicsDevice;

            if (_graphicsDevice == null)
                throw new Exception("DrawManager.Init() must be called before creating textures.");

            Texture2D texture = new Texture2D(_graphicsDevice, 1, 1);
            texture.SetData(new[] { color });
            return texture;
        }
        public static (Vector3[] Faces3dSpace, Color[] FaceColors) GetFacesIn3dSpace(int[][] ToRender, bool log)
        {
            List<Vector3> Faces3dSpace = new List<Vector3>();
            List<Color> FaceColors = new List<Color>();

            foreach (int[] c in ToRender)
            {
                Chunk? ChunkData = World.GetChunkData(c[0], c[1], c[2]);
                if (ChunkData == null || ChunkData.empty)
                {
                    continue;
                }
                for (int i = 0; i < 16 * 16 * 16; i++)
                {
                    int[] cord = [i % 16, (i / 16) % 16, i / (16 * 16)];
                    int[] f = new int[3];
                    int val = ChunkData.data[cord[0]][cord[1]][cord[2]];
                    if (val == 0) continue;
                    float dx = cord[0] + 16 * c[0] - Camera.Position.X;
                    float dy = cord[1] + 16 * c[1] - Camera.Position.Y;
                    float dz = cord[2] + 16 * c[2] - Camera.Position.Z;

                    float horizontal = (float)Math.Atan2(dx, dz);
                    float distXZ = (float)Math.Sqrt(dx * dx + dz * dz);
                    float vertical = (float)Math.Atan2(dy, distXZ);

                    Vector3 n = Player.GetNormalFromYawPitch(horizontal, vertical);
                    if (log) Console.WriteLine($"{n.X}, {n.Y}, {n.Z}, :");
                    foreach (int[] face in CubeNormals)
                    {
                        int nx = cord + face;
                        int ny = cord[1] + face[1];
                        int nz = cord[2] + face[2];

                    }

                }


            }

            return (Faces3dSpace.ToArray(), FaceColors.ToArray());
        }
        public static int[] AddIntList(int[] l1, int[] l2)
        {
            int[] rL = new int[l1.Length];
            foreach (int i = 0; i < l1.L)
            {
                rL
            }
            return rL;   
        }
    }
}