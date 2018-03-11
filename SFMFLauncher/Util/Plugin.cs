using System;

namespace SFMFLauncher.Util
{
    public class Plugin
    {
        public string Name { get; set; }
        public string[] Authors { get; set; }
        public int[] Version { get; set; }
        public string Download { get; set; }
        public bool Featured { get; set; }

        public Plugin(string name, string[] authors, int[] version, string download, bool featured)
        {
            Name = name;
            Authors = authors;
            Version = version;
            Download = download;
            Featured = featured;
        }

        public override string ToString()
        {
            return $"{Name} v{String.Join(".", Version)} By {String.Join(", ", Authors)}";
        }
    }
}
