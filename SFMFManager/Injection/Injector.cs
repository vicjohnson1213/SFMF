using ILRepacking;
using Mono.Cecil;
using Mono.Cecil.Cil;
using SFMFManager.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SFMFManager.Injection
{
    public static class Injector
    {
        public static bool IsFrameworkInstalled()
        {
            if (!File.Exists(Constants.AssemblyLocation))
                return false;

            AssemblyDefinition asmDef = AssemblyDefinition.ReadAssembly(Constants.AssemblyLocation);
            ModuleDefinition modDef = asmDef.MainModule;

            return Util.Util.GetDefinition(modDef.Types, "SFMF.Core", true) != null;
        }

        public static void InstallFramework(bool disableScoreReporting)
        {
            List<string> managed = Directory.GetFiles(Constants.ManagedLocation).Where(x => (x.Contains("UnityEngine") || x.Contains("AmplifyMotion") || x.Contains("Assembly-CSharp.") || x.Contains("Mono.Security")) && x.EndsWith(".dll")).ToList();
            foreach (string file in managed)
                if (!File.Exists(file.Substring(file.LastIndexOf("\\") + 1)))
                    File.Copy(file, file.Substring(file.LastIndexOf("\\") + 1), true);

            Merge(Constants.AssemblyLocation, Constants.AssemblyBackupLocation, "./SFMF.dll");
            Inject(Constants.AssemblyLocation, disableScoreReporting);
        }

        public static void UninstallFramework()
        {
            File.Copy(Constants.AssemblyBackupLocation, Constants.AssemblyLocation, true);
            File.Delete(Constants.AssemblyBackupLocation);
        }

        public static void Merge(string asmLoc, string asmBacLoc, string sfmfLoc)
        {
            AssemblyDefinition asmDef = AssemblyDefinition.ReadAssembly(asmLoc);
            ModuleDefinition modDef = asmDef.MainModule;

            if (Util.Util.GetDefinition(modDef.Types, "SFMF.Core", true) == null)
                File.Copy(asmLoc, asmBacLoc, true);

            var options = new RepackOptions
            {
                OutputFile = asmLoc,
                InputAssemblies = new[]
                {
                    asmBacLoc, sfmfLoc
                },
                SearchDirectories = new List<string>().AsEnumerable(),
                TargetKind = ILRepack.Kind.Dll
            };

            var repack = new ILRepack(options);
            repack.Repack();
        }

        private static void Inject(string asmLoc, bool disableScoreReporting)
        {
            Assembly asm = Assembly.Load(File.ReadAllBytes(asmLoc));

            AssemblyDefinition asmDef = AssemblyDefinition.ReadAssembly(asmLoc);
            ModuleDefinition modDef = asmDef.MainModule;

            InjectionMethod imStart = new InjectionMethod(modDef, "LocalGameManager", "Start");
            MethodReference mrInit = modDef.Import(asm.GetType("SFMF.Core").GetMethod("Init", new Type[] { }));
            imStart.MethodDef.Body.Instructions.Insert(7, imStart.MethodDef.Body.GetILProcessor().Create(OpCodes.Call, mrInit));

            if (disableScoreReporting)
            {
                InjectionMethod imSubmitHighscore = new InjectionMethod(modDef, "SteamIntegration", "SubmitHighscore");
                imSubmitHighscore.MethodDef.Body.Instructions.Insert(0, imSubmitHighscore.MethodDef.Body.GetILProcessor().Create(OpCodes.Ret));

                InjectionMethod imSubmitCombo = new InjectionMethod(modDef, "SteamIntegration", "SubmitCombo");
                imSubmitCombo.MethodDef.Body.Instructions.Insert(0, imSubmitCombo.MethodDef.Body.GetILProcessor().Create(OpCodes.Ret));
            }

            asmDef.Write(asmLoc);
        }
    }
}
