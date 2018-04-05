using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SFMFManager.Util
{
    public static class Constants
    {
        public static readonly string Version = "v1.0.5";
        public const string SettingsURL = "https://raw.githubusercontent.com/vicjohnson1213/SFMF/manifest/manifest.json";
        public static string SFMFDirectory = $"{AbsoluteInstallDirectory}/SFMF";
        public static string ManifestFile = $"{SFMFDirectory}/manifest.json";
        public static string InstalledModsFile = $"{SFMFDirectory}/installedMods.txt";

        private const string SteamRegistry = @"HKEY_CURRENT_USER\Software\Valve\Steam";
        private const string SteamConfig = "config/config.vdf";
        private const string SuperflightDirectory = "steamapps/common/SuperFlight";
        private const string SFManagedDirectory = "superflight_Data/Managed";
        private const string SFAssemblyFileName = "Assembly-CSharp.dll";

        public static string ManagedLocation => $"{AbsoluteInstallDirectory}/superflight_Data/Managed";
        public static string AssemblyLocation => $"{ManagedLocation}/{SFAssemblyFileName}";
        public static string AssemblyBackupLocation => $"{AssemblyLocation}.backup";
        private static string _absoluteInstallDirectory;

        public static string AbsoluteInstallDirectory
        {
            get
            {
                if (_absoluteInstallDirectory != null)
                    return _absoluteInstallDirectory;

                string steamDirectory = Registry.GetValue($"{SteamRegistry}", "SteamPath", null)?.ToString();
                if (steamDirectory != null)
                {
                    var steamInstallLocations = new List<string> { steamDirectory };

                    var configLines = File.ReadAllLines($"{steamDirectory}/{SteamConfig}").ToList();

                    steamInstallLocations.AddRange(configLines
                        .Where(l => l.Contains("BaseInstallFolder_"))
                        .Select(ExtractValueFromVDF));

                    foreach (var location in steamInstallLocations)
                    {
                        var assemblyFile = new FileInfo($"{location}/{SuperflightDirectory}/{SFManagedDirectory}/{SFAssemblyFileName}");
                        if (assemblyFile.Exists)
                        {
                            _absoluteInstallDirectory = $"{location}/{SuperflightDirectory}";
                            return _absoluteInstallDirectory;
                        }
                    }
                }

                return null;
            }
        }

        private static string ExtractValueFromVDF(string vdfLine)
        {
            var re = new Regex(@"\s*""BaseInstallFolder_\d+""\s+""(?<Value>.+)""\s*");
            return re.Match(vdfLine).Groups["Value"].Value;
        }
    }
}
