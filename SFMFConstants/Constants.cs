using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;

namespace SFMFConstants
{
    public static class Constants
    {
        public static readonly string Version = "v1.0.2";
        public const string SettingsURL = "https://raw.githubusercontent.com/vicjohnson1213/SFMF/manifest/manifest.json";
        public static string SFMFDirectory = $"{AbsoluteInstallDirectory}/SFMF";
        public static string ManifestFile = $"{SFMFDirectory}/manifest.json";
        public static string InstalledModsFile = $"{SFMFDirectory}/installedMods.txt";

        public const string SteamRegistry = @"HKEY_CURRENT_USER\Software\Valve\Steam";
        public const string SteamConfig = "config/config.vdf";
        public const string SuperflightDirectory = "steamapps/common/SuperFlight";
        public const string ManagedDirectory = "superflight_Data/Managed";
        public const string AssemblyLib = "Assembly-CSharp.dll";
        public const string ModExt = "dll";

        public static string ManagedLocation => $"{AbsoluteInstallDirectory}/{ManagedDirectory}";
        public static string AssemblyLocation => $"{ManagedLocation}/{AssemblyLib}";
        public static string AssemblyBackupLocation => $"{AssemblyLocation}.backup";
        private static string _absoluteInstallDirectory;

        public static string AbsoluteInstallDirectory {
            get {
                if (_absoluteInstallDirectory != null)
                    return _absoluteInstallDirectory;

                string steam = Registry.GetValue($"{SteamRegistry}", "SteamPath", null)?.ToString();
                if (steam != null)
                {
                    List<string> paths = new List<string>();
                    paths.Add(steam);
                    string[] lines = File.ReadAllLines($"{steam}/{SteamConfig}");
                    VDFPopulate(paths, lines, 1);
                    foreach (var path in paths)
                        if (Directory.Exists($"{path}/{SuperflightDirectory}/{ManagedDirectory}") && File.Exists($"{path}/{SuperflightDirectory}/{ManagedDirectory}/{AssemblyLib}"))
                        {
                            _absoluteInstallDirectory = $"{path}/{SuperflightDirectory}";
                            return _absoluteInstallDirectory;
                        }
                            
                }

                return null;
            }
        }

        private static void VDFPopulate(List<string> paths, string[] lines, int index)
        {
            foreach (string line in lines)
                if (line.Trim().StartsWith($"\"BaseInstallFolder_{index}\""))
                {
                    string path = line.Trim().Split('\t')[2];
                    paths.Add(path.Substring(1, path.Length - 2).Replace("\\\\", "/"));
                    VDFPopulate(paths, lines, index + 1);
                }
        }
    }
}
