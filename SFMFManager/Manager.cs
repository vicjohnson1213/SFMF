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

        public Manager()
        {
            if (!Directory.Exists(Constants.SFMFDirectory))
                Directory.CreateDirectory(Constants.SFMFDirectory);

            if (!File.Exists(Constants.ManifestFile))
                using (var w = File.AppendText(Constants.ManifestFile))
                    w.WriteLine("[]");

            if (!File.Exists(Constants.InstalledModsFile))
                using (var w = File.AppendText(Constants.InstalledModsFile)) { };
            
            Manifest = new List<Mod>();

            Settings = InitSettings();
            InitSavedMods();
        }

        public void InstallSFMF()
        {
            if (!IsSFMFInstalled)
                Injector.InstallFramework(!IsScoreReportingEnabled);
        }

        public void UninstallSFMF()
        {
            if (IsSFMFInstalled)
                Injector.UninstallFramework();
        }

        public void DownloadMod(Mod mod)
        {
            var newPath = $"{Constants.SFMFDirectory}/{mod.Name}.dll";

            using (var client = new WebClient())
                client.DownloadFileAsync(new Uri(mod.Download), newPath);

            mod.Path = newPath;
            
            Manifest.Add(mod);
            File.WriteAllText(Constants.ManifestFile, Newtonsoft.Json.JsonConvert.SerializeObject(Manifest));

            InstallMod(mod);
        }

        public void LoadLocalMod(string path)
        {
            var fileInfo = new FileInfo(path);

            var mod = new Mod
            {
                Name = fileInfo.Name,
                Version = "local",
                Path = path,
                DisableScoreReporting = true,
                Local = true
            };

            Manifest.Add(mod);
            File.WriteAllText(Constants.ManifestFile, Newtonsoft.Json.JsonConvert.SerializeObject(Manifest));

            InstallMod(mod);
        }

        public void RemoveMod(Mod mod)
        {
            if (!mod.Local)
                File.Delete(mod.Path);

            Manifest.Remove(mod);
            File.WriteAllText(Constants.ManifestFile, Newtonsoft.Json.JsonConvert.SerializeObject(Manifest));
        }

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

        public void ReloadOnlineMods()
        {
            Settings = InitSettings();
        }

        private void InitSavedMods()
        {
            var json = File.ReadAllText(Constants.ManifestFile);
            Manifest = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Mod>>(json);

            var installedMods = File.ReadAllLines(Constants.InstalledModsFile);
            foreach (var m in Manifest)
                m.Installed = installedMods.Contains(m.Path);
        }

        private Settings InitSettings()
        {
            string json;
            using (var client = new WebClient())
                json = client.DownloadString(new Uri(Constants.SettingsURL));

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(json);
        }
    }
}
