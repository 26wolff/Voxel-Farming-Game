using System;
using System.IO;

namespace Program
{
    public static class GameDataSync
    {
        // Source: your bin folder (where the game executable is)
        private static string sourceRoot = Path.Combine(AppContext.BaseDirectory, "Data");

        // Destination: AppData\Local\VoxelFarm\Data
        private static string destRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "VoxelFarm",
            "Data"
        );

        /// <summary>
        /// Call this on startup to sync the base data structure to AppData
        /// </summary>
        public static void Reset(bool kill = false)
        {
            string targetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            targetDirectory = Path.Combine(targetDirectory, "VoxelFarm");
            Console.WriteLine("KILLED ALL FILES");
            if (kill)
            {
                if (Directory.Exists(targetDirectory))
                {
                    Directory.Delete(targetDirectory, true);
                }
                else
                {
                    Console.WriteLine($"Directory not found: {targetDirectory}");
                }
            }
        }
        public static void Sync(bool kill = false)
        {
            if (kill)
            {
                
            }
            if (!Directory.Exists(sourceRoot))
            {
                Console.WriteLine($"Base data folder not found: {sourceRoot}");
                return;
            }

            CopyDirectoryRecursive(sourceRoot, destRoot);
        }

        /// <summary>
        /// Recursively copy files and folders if they don't exist
        /// </summary>
        private static void CopyDirectoryRecursive(string sourceDir, string targetDir)
        {
            // Create target folder if it doesn't exist
            Directory.CreateDirectory(targetDir);

            // Copy all files
            foreach (var filePath in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(filePath);
                string destFile = Path.Combine(targetDir, fileName);

                if (!File.Exists(destFile))
                {
                    File.Copy(filePath, destFile);
                    Console.WriteLine($"Copied file: {destFile}");
                }
            }

            // Recurse into subfolders
            foreach (var dirPath in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dirPath);
                string destSubDir = Path.Combine(targetDir, dirName);

                CopyDirectoryRecursive(dirPath, destSubDir);
            }
        }
    }
}
