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
        private const string modDir = @".\installedMods";
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
            try
            {
                string[] files = Directory.GetFiles(modDir, $"*.{modExt}", SearchOption.TopDirectoryOnly);
                if (files.Length == 0)
                    Console.WriteLine("No mods found");
                else
                {
                    List<Type> plugins = new List<Type>();
                    Console.WriteLine($"Found {files.Length} mods");
                    foreach (string file in files)
                    {
                        Console.WriteLine($"Loading mod: {file}");
                        try
                        {
                            Assembly asm = Assembly.Load(File.ReadAllBytes(file));
                            foreach (var type in asm.GetExportedTypes())
                                if (typeof(IPlugin).IsAssignableFrom(type))
                                    plugins.Add(type);
                            Console.WriteLine($"Loaded mod: {file}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Failed to load mod: '{file}': {e.Message}");
                        }
                    }
                    Console.WriteLine($"Loaded {plugins.Count} mods");
                    foreach (Type plugin in plugins)
                        gameObject.AddComponent(plugin);
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to find mod directory '{modDir}'");
            }
        }
    }
}