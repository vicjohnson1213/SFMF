using Newtonsoft.Json;

namespace SFMFManager.Util
{
    public class Mod
    {
        public string Name { get; set; }
        public string[] Authors { get; set; }
        public string Version { get; set; }
        public string Download { get; set; }
        public string Path { get; set; }
        public bool DisableScoreReporting { get; set; }
        public bool Local { get; set; }

        [JsonIgnore]
        public bool Installed { get; set; }
    }
}
