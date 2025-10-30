using System;
using System.IO;
using System.Text.Json;
using System.Numerics;

namespace Program
{
    public static class Player
    {
        public static Vector3 Position = Vector3.Zero;
        public static Vector2 ViewAngle = Vector2.Zero;
        public static string World = "w-1";

        // Path in AppData\Local\VoxelFarm\Data\Player\PlayerData.json
        private static string dataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "VoxelFarm",
            "Data",
            "Player",
            "PlayerData.json"
        );

        public static void Init()
        {
            if (!File.Exists(dataPath))
            {
                Console.WriteLine($"Player data file not found: {dataPath}");
                return;
            }

            string jsonString = File.ReadAllText(dataPath);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var playerData = JsonSerializer.Deserialize<PlayerData>(jsonString, options);

            if (playerData != null)
            {
                if (playerData.Position != null)
                {
                    Position = new Vector3(
                        playerData.Position.X,
                        playerData.Position.Y,
                        playerData.Position.Z
                    );
                }
            }
            Console.WriteLine($"Loaded player position: {Position}");
        }

        public static void Update(float dt, InputManager input)
        {
            if (input.Right) Position.X += 0.5f*dt;
            if (input.Left) Position.X -= 0.5f*dt;
            if (input.Up) Position.Y -= 0.5f*dt;
            if (input.Down) Position.Y += 0.5f*dt;
        }

        public static void Save()
        {
            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(dataPath)!);

            var playerData = new PlayerData
            {
                Position = new PositionData
                {
                    X = Position.X,
                    Y = Position.Y,
                    Z = Position.Z
                }
            };

            var options = new JsonSerializerOptions { WriteIndented = true }; // pretty JSON
            string jsonString = JsonSerializer.Serialize(playerData, options);

            File.WriteAllText(dataPath, jsonString);
            Console.WriteLine($"Saved player position to: {dataPath}");
        }

        // Helper classes
        public class PlayerData
        {
            public PositionData Position { get; set; } = new PositionData();
        }

        public class PositionData
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
        }
    }
}
