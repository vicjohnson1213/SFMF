using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;

namespace SFMFConstants
{
    public static class Constants
    {
        public static readonly string Version = "v1.0.1";
        public const string SettingsURL = "https://gist.github.com/vicjohnson1213/a2d401c991f31ea54377fc42dafb548b";
        public const string SteamRegistry = @"HKEY_CURRENT_USER\Software\Valve\Steam";
        public const string SteamConfig = "config/config.vdf";
        public const string SuperflightDirectory = "steamapps/common/SuperFlight";
        public const string ManagedDirectory = "superflight_Data/Managed";
        public const string AssemblyLib = "Assembly-CSharp.dll";
        public const string InstalledModDirectory = "installedMods";
        public const string SavedModDirectory = "savedMods";
        public const string ModExt = "dll";

        public static string SavedManifestLocation => $"{SavedModLocation}/manifest.json";
        public static string InstalledManifestLocation => $"{InstalledModLocation}/manifest.json";
        public static string ManagedLocation => $"{AbsoluteInstallDirectory}/{ManagedDirectory}";
        public static string AssemblyLocation => $"{ManagedLocation}/{AssemblyLib}";
        public static string AssemblyBackupLocation => $"{AssemblyLocation}.backup";
        public static string InstalledModLocation => $"{AbsoluteInstallDirectory}/{InstalledModDirectory}";
        public static string SavedModLocation => $"{AbsoluteInstallDirectory}/{SavedModDirectory}";
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
