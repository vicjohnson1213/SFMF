using SFMFConstants;
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
        public List<Mod> OnlineMods = new List<Mod>();
        public List<Mod> SavedMods = new List<Mod>();
        public List<Mod> InstalledMods = new List<Mod>();

        public Manager()
        {
            LoadAllMods();
        }

        public void InstallSFMF()
        {
            Injector.InstallFramework(!IsScoreReportindEnabled());
        }

        public void UninstallSFMF()
        {
            Injector.UninstallFramework();
        }

        public bool IsSFMFInstalled()
        {
            return Injector.IsFrameworkInstalled();
        }

        public bool IsScoreReportindEnabled()
        {
            return !InstalledMods.Any(m => m.DisableScoreReporting);
        }

        public void SaveMod(Mod mod)
        {
            var newPath = $"{Constants.SavedModLocation}/{mod.Name}.dll";

            if (!Directory.Exists(Constants.SavedModLocation))
                Directory.CreateDirectory(Constants.SavedModLocation);

            if (String.IsNullOrEmpty(mod.Path))
                using (var client = new WebClient())
                    client.DownloadFileAsync(new Uri(mod.Download), newPath);
            else
                File.Move(mod.Path, newPath);

            mod.Path = newPath;
            
            SavedMods.Add(mod);

            File.WriteAllText(Constants.SavedManifestLocation, Newtonsoft.Json.JsonConvert.SerializeObject(SavedMods));
        }

        public void RemoveMod(Mod mod)
        {
            SavedMods = SavedMods.Where(m => m.Path != mod.Path).ToList();

            File.Delete(mod.Path);
            File.WriteAllText(Constants.SavedManifestLocation, Newtonsoft.Json.JsonConvert.SerializeObject(SavedMods));
        }

        public void InstallMod(Mod mod)
        {
            var newPath = $"{Constants.InstalledModLocation}/{mod.Name}.dll";

            if (!Directory.Exists(Constants.InstalledModLocation))
                Directory.CreateDirectory(Constants.InstalledModLocation);

            File.Move(mod.Path, newPath);
            RemoveMod(mod);
            mod.Path = newPath;

            InstalledMods.Add(mod);

            File.WriteAllText(Constants.InstalledManifestLocation, Newtonsoft.Json.JsonConvert.SerializeObject(InstalledMods));
        }

        public void UninstallMod(Mod mod)
        {
            InstalledMods = InstalledMods.Where(m => m.Path != mod.Path).ToList();

            File.WriteAllText(Constants.InstalledManifestLocation, Newtonsoft.Json.JsonConvert.SerializeObject(InstalledMods));

            SaveMod(mod);
        }

        public void LoadAllMods()
        {
            OnlineMods = LoadOnlineMods();
            SavedMods = LoadSavedMods();
            InstalledMods = LoadInstalledMods();
        }

        public List<Mod> LoadInstalledMods()
        {
            if (!File.Exists(Constants.InstalledManifestLocation))
                return new List<Mod>();

            string manifest = File.ReadAllText(Constants.InstalledManifestLocation);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Mod>>(manifest);
        }

        public List<Mod> LoadSavedMods()
        {
            if (!File.Exists(Constants.SavedManifestLocation))
                return new List<Mod>();

            string manifest = File.ReadAllText(Constants.SavedManifestLocation);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Mod>>(manifest);
        }

        public List<Mod> LoadOnlineMods()
        {
            return LoadSettings().modlist;
        }

        public Settings LoadSettings()
        {
            string json;
            using (var client = new WebClient())
                json = client.DownloadString(new Uri(Constants.SettingsURL));

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(json);
        }
    }
}
