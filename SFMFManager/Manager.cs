using SFMFConstants;
using SFMFManager.Injection;
using SFMFManager.Util;
using Supremes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace SFMFManager
{
    public static class Manager
    {
        public static void InstallSFMF(bool disableScoreReporting)
        {
            Injector.InstallFramework(disableScoreReporting);
        }

        public static void UninstallSFMF()
        {
            Injector.UninstallFramework();
        }

        public static bool IsSFMFInstalled()
        {
            return Injector.IsFrameworkInstalled();
        }

        public static List<Mod> SaveMod(Mod mod)
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

            var mods = GetSavedMods();
            mods.Add(mod);

            File.WriteAllText(Constants.SavedManifestLocation, Newtonsoft.Json.JsonConvert.SerializeObject(mods));

            return mods;
        }

        public static List<Mod> RemoveMod(Mod mod)
        {
            var mods = GetSavedMods();
            mods = mods.Where(m => m.Path != mod.Path).ToList();

            File.Delete(mod.Path);
            File.WriteAllText(Constants.SavedManifestLocation, Newtonsoft.Json.JsonConvert.SerializeObject(mods));

            return mods;
        }

        public static List<Mod> InstallMod(Mod mod)
        {
            var newPath = $"{Constants.InstalledModLocation}/{mod.Name}.dll";

            if (!Directory.Exists(Constants.InstalledModLocation))
                Directory.CreateDirectory(Constants.InstalledModLocation);

            File.Move(mod.Path, newPath);
            RemoveMod(mod);
            mod.Path = newPath;

            var installedMods = GetInstalledMods();
            installedMods.Add(mod);

            File.WriteAllText(Constants.InstalledManifestLocation, Newtonsoft.Json.JsonConvert.SerializeObject(installedMods));

            return installedMods;
        }

        public static List<Mod> UninstallMod(Mod mod)
        {
            var mods = GetInstalledMods();
            mods = mods.Where(m => m.Path != mod.Path).ToList();

            File.WriteAllText(Constants.InstalledManifestLocation, Newtonsoft.Json.JsonConvert.SerializeObject(mods));

            SaveMod(mod);

            return mods;
        }

        public static List<Mod> GetInstalledMods()
        {
            if (!File.Exists(Constants.InstalledManifestLocation))
                return new List<Mod>();

            string manifest = File.ReadAllText(Constants.InstalledManifestLocation);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Mod>>(manifest);
        }

        public static List<Mod> GetSavedMods()
        {
            if (!File.Exists(Constants.SavedManifestLocation))
                return new List<Mod>();

            string manifest = File.ReadAllText(Constants.SavedManifestLocation);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Mod>>(manifest);
        }

        public static List<Mod> GetOnlineMods()
        {
            return GetSettings().modlist;
        }

        public static Settings GetSettings()
        {
            string settingsString;
            using (var client = new WebClient())
            {
                settingsString = client.DownloadString(new Uri(Constants.SettingsURL));
            }

            Supremes.Nodes.Document document = Dcsoup.Parse(settingsString);
            string json = document.GetElementsByClass("blob-wrapper").Text;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(json);
        }
    }
}
