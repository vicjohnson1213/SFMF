using SFMFManager.Dto;
using SFMFManager.Injection;
using SFMFManager.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace SFMFManager
{
    public class Manager
    {
        private Settings Settings { get; set; }
        private List<Mod> Manifest { get; set; }

        public List<Mod> OnlineMods => Settings.modlist;
        public List<Mod> InstalledMods => Manifest.Where(m => m.Installed).ToList();
        public List<Mod> SavedMods => Manifest.Where(m => !m.Installed).ToList();

        public bool IsSFMFInstalled => Injector.IsFrameworkInstalled();
        public bool IsScoreReportingEnabled => !InstalledMods.Any(m => m.DisableScoreReporting);
        public bool IsUpdateAvailable => Settings != null && (Settings.version != Constants.Version);

        public string Homepage => Settings.homepage;
        public string SuperflightGameURI => Settings.superflightGameURI;

        public Manager()
        {
            Manifest = new List<Mod>();

            InitSFMFDirectory();

            InitSettings();
            InitSavedMods();
        }

        /// <summary>
        /// Injects the SFMF framework into the Superflight game.
        /// </summary>
        public void InstallSFMF()
        {
            if (!IsSFMFInstalled)
                Injector.InstallFramework(!IsScoreReportingEnabled);
        }

        /// <summary>
        /// Removes the SFMF framework from the Superflight game.
        /// </summary>
        public void UninstallSFMF()
        {
            if (IsSFMFInstalled)
                Injector.UninstallFramework();
        }

        /// <summary>
        /// Downloads an online mod and installs it. If the mod has any settings or controls,
        /// a settings file is created (if it doesn't exist already) with the defaults populated.
        /// </summary>
        /// <param name="mod">The mod to download.</param>
        public void DownloadMod(Mod mod)
        {
            var newPath = $"{Constants.SFMFDirectory}/{mod.Name}-{mod.Version}.dll";

            mod.SettingsPath = $"{Constants.ModSettingsDirectory}/{mod.Name}.csv";

            // Don't everwrite any custom settings that may already exist for this mod, load them instead.
            if (!File.Exists(mod.SettingsPath))
                SaveModSettings(mod);
            else
                mod.Settings = GetModSettings(mod.SettingsPath);

            using (var client = new WebClient())
                client.DownloadFileAsync(new Uri(mod.Download), newPath);

            mod.Path = newPath;
            
            Manifest.Add(mod);
            SaveManifest();

            InstallMod(mod);
        }

        /// <summary>
        /// Loads a mod from the user's local machine and installs it.
        /// </summary>
        /// <param name="path">The absolute path of the mod to load.</param>
        public void LoadLocalMod(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var settingsPath = $"{Constants.ModSettingsDirectory}/{name}.csv";

            var mod = new Mod
            {
                Name = name,
                Version = "local",
                Path = path,
                SettingsPath = settingsPath,
                Settings = GetModSettings(settingsPath),
                DisableScoreReporting = true,
                Local = true
            };

            Manifest.Add(mod);
            SaveManifest();

            InstallMod(mod);
        }

        /// <summary>
        /// Removes a mod from the user's machine.
        /// </summary>
        /// <param name="mod">The mod to remove</param>
        public void RemoveMod(Mod mod)
        {
            if (!mod.Local)
                File.Delete(mod.Path);

            Manifest.Remove(mod);
            SaveManifest();
        }

        /// <summary>
        /// Adds a mod to the list of mods to load into Superflight.
        /// </summary>
        /// <param name="mod"></param>
        public void InstallMod(Mod mod)
        {
            var isScoreReportingEnabled = IsScoreReportingEnabled;

            mod.Installed = true;
            var installedMods = File.ReadAllLines(Constants.InstalledModsFile).ToList();
            installedMods.Add(mod.Path);

            File.WriteAllLines(Constants.InstalledModsFile, installedMods);

            if (IsSFMFInstalled && isScoreReportingEnabled != IsScoreReportingEnabled)
            {
                UninstallSFMF();
                InstallSFMF();
            }
        }

        /// <summary>
        /// Removes a mod from the list of mods to load into Superflight.
        /// </summary>
        /// <param name="mod"></param>
        public void UninstallMod(Mod mod)
        {
            var isScoreReportingEnabled = IsScoreReportingEnabled;

            mod.Installed = false;
            var installedMods = File.ReadAllLines(Constants.InstalledModsFile).ToList();
            installedMods.Remove(mod.Path);

            File.WriteAllLines(Constants.InstalledModsFile, installedMods);

            if (IsSFMFInstalled && isScoreReportingEnabled != IsScoreReportingEnabled)
            {
                UninstallSFMF();
                InstallSFMF();
            }
        }

        /// <summary>
        /// Re-downloads the settings file and updates the in-memory settings.
        /// </summary>
        public void ReloadOnlineMods()
        {
            InitSettings();
        }

        public void SaveModSettings(Mod mod)
        {
            if (mod.Settings != null && mod.SettingsPath != null)
                File.WriteAllText(mod.SettingsPath, mod.Settings.ToString());

            SaveManifest();
        }

        public void SaveManifest()
        {
            File.WriteAllText(Constants.ManifestFile, Newtonsoft.Json.JsonConvert.SerializeObject(Manifest));
        }

        /// <summary>
        /// Creates any of the SFMF directories and files that don't already exist.
        /// </summary>
        private void InitSFMFDirectory()
        {
            if (!Directory.Exists(Constants.SFMFDirectory))
                Directory.CreateDirectory(Constants.SFMFDirectory);

            if (!Directory.Exists(Constants.ModSettingsDirectory))
                Directory.CreateDirectory(Constants.ModSettingsDirectory);

            if (!File.Exists(Constants.ManifestFile))
                using (var w = File.AppendText(Constants.ManifestFile))
                    w.WriteLine("[]");

            if (!File.Exists(Constants.InstalledModsFile))
                using (var w = File.AppendText(Constants.InstalledModsFile)) { };
        }

        /// <summary>
        /// Populates the in-memory mod list from the manifest file.
        /// </summary>
        private void InitSavedMods()
        {
            var json = File.ReadAllText(Constants.ManifestFile);
            Manifest = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Mod>>(json);

            var installedMods = File.ReadAllLines(Constants.InstalledModsFile);
            foreach (var m in Manifest)
                m.Installed = installedMods.Contains(m.Path);
        }

        /// <summary>
        /// Downloads the settings file and updates the in-memory settings.
        /// </summary>
        private void InitSettings()
        {
            string json;
            using (var client = new WebClient())
                json = client.DownloadString(new Uri(Constants.SettingsURL));

            Settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(json);
        }

        private ModSettings GetModSettings(string settingsPath)
        {
            if (!File.Exists(settingsPath))
                return null;

            var settings = File.ReadAllText(settingsPath);
            return ModSettings.FromString(settings);
        }
    }
}
