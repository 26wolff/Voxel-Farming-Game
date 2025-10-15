using System.IO;
using System.Text.Json;
using System.Numerics;

namespace Program
{
    public static class Player
    {
        public static Vector3 Position = Vector3.Zero;
        public static Vector2 VeiwAngle = Vector2.Zero;

        public static void Innit()
        {
            // Build the path relative to the executable
            string dataPath = Path.Combine(AppContext.BaseDirectory, "Data", "Player", "PlayerData.json");
            if (!File.Exists(dataPath))
            {
                Console.WriteLine($"Player data file not found: {dataPath}");
                return;
            }
            string jsonString = File.ReadAllText(dataPath);
            Console.WriteLine(jsonString);
        }
    }
}