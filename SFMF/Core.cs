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
        private const string pluginDir = @".\plugins";
        private const string pluginExt = "dll";

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
            Console.WriteLine("Loading plugins...");
            try
            {
                string[] files = Directory.GetFiles(pluginDir, $"*.{pluginExt}", SearchOption.TopDirectoryOnly);
                if (files.Length == 0)
                    Console.WriteLine("No plugins found");
                else
                {
                    List<Type> plugins = new List<Type>();
                    Console.WriteLine($"Found {files.Length} plugins");
                    foreach (string file in files)
                    {
                        Console.WriteLine($"Loading plugin {file}");
                        try
                        {
                            Assembly asm = Assembly.Load(File.ReadAllBytes(file));
                            foreach (var type in asm.GetExportedTypes())
                                if (typeof(IPlugin).IsAssignableFrom(type))
                                    plugins.Add(type);
                            Console.WriteLine($"Loaded plugin {file}");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Failed to load plugin '{file}': {e.Message}");
                        }
                    }
                    Console.WriteLine($"Loaded {plugins.Count} plugins");
                    foreach (Type plugin in plugins)
                        gameObject.AddComponent(plugin);
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to find plugin directory '{pluginDir}'");
            }
        }
    }
}