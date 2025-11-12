using System;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Program
{
    public static class Player
    {
        public static Vector3 Position = new Vector3(0f, 20f, 0f);
        public static Vector2 ViewAngle = Vector2.Zero;
        public static string World = "w-1";
        public static float Speed = 5;
        public static Vector3 Normal;

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
            Position.Y = 4f;
            Normal = GetNormalFromYawPitch(ViewAngle.X, ViewAngle.Y);

        }

        public static void OnKeyPress(Keys key)
        {
            if (key == Keys.A)
            {
                Console.WriteLine($"{Position.X}, {Position.Z}");
            }
        }

        public static void Update(float dt)
        {
            Vector3 forward = Camera.Forward;
            Vector3 right = Camera.Right;

            if (InputManager.Key[(int)Keys.W]) Position += forward * Speed * dt;
            if (InputManager.Key[(int)Keys.S]) Position -= forward * Speed * dt;
            if (InputManager.Key[(int)Keys.D]) Position += right * Speed * dt;
            if (InputManager.Key[(int)Keys.A]) Position -= right * Speed * dt;
            if (InputManager.Key[(int)Keys.F]) Position.Y += Speed * dt;
            if (InputManager.Key[(int)Keys.R]) Position.Y -= Speed * dt;
            Normal = GetNormalFromYawPitch(ViewAngle.X, ViewAngle.Y);
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
        public static Vector3 GetNormalFromYawPitch(float yawRad, float pitchRad)
        {// AAAAAAAA Make this in init and update for player, and acces in dferaw for face culling
            double x = Math.Cos(yawRad) * Math.Cos(pitchRad);
            double y = Math.Sin(pitchRad);
            double z = Math.Sin(yawRad) * Math.Cos(pitchRad);

            return Vector3.Normalize(new Vector3((float)x, (float)y, (float)z));
        }
    }
}
