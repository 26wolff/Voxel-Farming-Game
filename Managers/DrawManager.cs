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
        public static Vector3[] CubeNormals = [new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(0, -1, 0), new Vector3(0, 1, 0), new Vector3(0, 0, -1), new Vector3(0, 0, 1)];

        public static void Init(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
        }

        public static void Draw()
        {
            int[][] WorldToRender = World.GetChunksToRender();
            int[][] ToRender = Camera.GetChunksInVeiw(WorldToRender);
            Face[] Faces3dSpace = GetFacesIn3dSpace(ToRender, rep);

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
            _spriteBatch.Draw(tex, new Rectangle((int)(Player.Position.X * Screen.Height), (int)(-Player.Position.Z * Screen.Height), 64, 64), Color.White);
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
        public static Face[]  GetFacesIn3dSpace(int[][] ToRender, bool log)
        {
            List<Face> Faces3dSpace = new List<Face>();

            foreach (int[] c in ToRender)
            {
                Chunk? ChunkData = World.GetChunkData(c[0], c[1], c[2]);
                if (ChunkData == null || ChunkData.empty)
                {
                    continue;
                }
                for (int i = 0; i < 16 * 16 * 16; i++)
                {
                    Vector3 cord = new Vector3(i % 16 + 0.5f, (i / 16) % 16 + 0.5f, i / (16 * 16) + 0.5f);
                    int[] f = new int[3];
                    int val = ChunkData.data[(int)cord.X][(int)cord.Y][(int)cord.Z];
                    if (val == 0) continue;
                    float dx = cord.X + 16 * c[0] - Camera.Position.X;
                    float dy = cord.Y + 16 * c[1] - Camera.Position.Y;
                    float dz = cord.Z + 16 * c[2] - Camera.Position.Z;

                    float horizontal = (float)Math.Atan2(dx, dz);
                    float vertical = (float)Math.Atan2(dy, Math.Sqrt(dx * dx + dz * dz));

                    Vector3 normal = Player.GetNormalFromYawPitch(horizontal, vertical);
                    if (log) Console.WriteLine($"{normal.X}, {normal.Y}, {normal.Z}, :");
                    foreach (Vector3 face in CubeNormals)
                    {

                        Vector3 newCord = cord + face;
                        if (IsVoxelSolid(ChunkData, (int)newCord.X, (int)newCord.Y, (int)newCord.Z))
                        {
                            if (Vector3.Dot(Player.Normal, normal) > 0)
                            {
                                Faces3dSpace.Add(new Face());
                            }
                        }

                    }

                }


            }

            return Faces3dSpace.ToArray();
        }
        private static bool IsVoxelSolid(Chunk chunk, int x, int y, int z)
        {
            if (x < 0 || x >= 16 || y < 0 || y >= 16 || z < 0 || z >= 16) return false; // treat outside chunk as air
            return chunk.data[x][y][z] != 0;
        }
    }
    public class Face
    {
        Vector3[] V = new Vector3[3];
        float D;
        public Face(Vector3[] v,float d)
        {
            V = v;D = d;
        }
    }
}