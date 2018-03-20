using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SFMF
{
    public class Core : MonoBehaviour
    {
        private const string modsFile = @".\SFMF\installedMods.txt";
        private const string modExt = "dll";

        [DllImport("kernel32")]
        private static extern bool AllocConsole();

        public static void Init()
        {
            LocalGameManager.Singleton.gameObject.AddComponent<Core>();
        }

        public void Start()
        {
            AllocConsole();
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            });

            Console.WriteLine("SFMF Initialized");

            LoadPlugins();
        }

        private void LoadPlugins()
        {
            Console.WriteLine("Loading mods...");

            if (!File.Exists(modsFile))
            {
                Console.WriteLine($"'{modsFile}' does not exist.");
                return;
            }

            string[] installedMods = File.ReadAllLines(modsFile);
            if (installedMods.Length == 0)
                Console.WriteLine("No mods found");
            else
            {
                List<Type> mods = new List<Type>();
                Console.WriteLine($"Found {installedMods.Length} mods");
                foreach (string modPath in installedMods)
                {
                    try
                    {
                        Assembly asm = Assembly.Load(File.ReadAllBytes(modPath));
                        foreach (var type in asm.GetExportedTypes())
                            if (typeof(IMod).IsAssignableFrom(type))
                                mods.Add(type);

                        Console.WriteLine($"Loaded mod: {modPath}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to load mod: '{modPath}': {e.Message}");
                    }
                }

                Console.WriteLine($"Loaded {mods.Count} mods");

                foreach (Type mod in mods)
                    gameObject.AddComponent(mod);
            }
        }
    }
}