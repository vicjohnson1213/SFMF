using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFMFManager.Dto
{
    public class Mod
    {
        public string Name { get; set; }
        public string Homepage { get; set; }
        public string[] Authors { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Download { get; set; }
        public string Path { get; set; }
        public string SettingsPath { get; set; }
        public bool DisableScoreReporting { get; set; } = true;
        public bool Local { get; set; }

        public ModSettings Settings { get; set; }

        [JsonIgnore]
        public bool Installed { get; set; }
    }

    public class ModSettings
    {
        public List<ModSetting> Settings { get; set; }
        public List<ModControl> Controls { get; set; }

        public override string ToString()
        {
            var settings = "";

            if (Settings != null)
                settings += string.Join("\n", Settings);

            if (Settings != null && Controls != null)
                settings += "\n";

            if (Controls != null)
                settings += string.Join("\n", Controls);

            return settings;
        }

        public static ModSettings FromString(string settingsString)
        {
            var settings = new List<ModSetting>();
            var controls = new List<ModControl>();

            foreach (var line in settingsString.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("Setting"))
                    settings.Add(ModSetting.FromString(line));
                else if (line.StartsWith("Control"))
                    controls.Add(ModControl.FromString(line));
            }

            return new ModSettings
            {
                Settings = settings,
                Controls = controls
            };
        }
    }

    public class ModSetting
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"Setting,{Name},{Value}";
        }

        public static ModSetting FromString(string setting)
        {
            var parts = setting.Split(',');

            return new ModSetting
            {
                Name = parts[1],
                Value = parts[2]
            };
        }
    }

    public class ModControl
    {
        public string Action { get; set; }
        public string Keyboard { get; set; }
        public string Controller { get; set; }

        public override string ToString()
        {
            return $"Control,{Action},{Keyboard},{Controller}";
        }

        public static ModControl FromString(string setting)
        {
            var parts = setting.Split(',');

            return new ModControl
            {
                Action = parts[1],
                Keyboard = parts[2],
                Controller = parts[3]
            };
        }
    }
}
