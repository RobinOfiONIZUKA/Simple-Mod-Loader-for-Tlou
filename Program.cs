using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Memory;
using Injector;
using DiscordRPC;

namespace Scripthook
{
    class Program
    {
        private static DiscordRpcClient discordRpcClient;
        private static int numScriptsLoaded = 0;
        static void Main(string[] args)
        {
            bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            if (!isAdmin)
            {
                Console.WriteLine("Please run this program as an administrator.");
                Console.ReadKey();
                return;

            }



            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string assemblyDir = Path.GetDirectoryName(assemblyPath);
            Environment.CurrentDirectory = assemblyDir;

            Console.Title = "Basic Mods (dlls) Loader Made By Robin";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Welcome to Mods Loader!");
            Console.ResetColor();
            Console.WriteLine("Scripthook Started Successfully");

            string gamePath;


            string configPath = Path.Combine(Environment.CurrentDirectory, "Config.ini");
            if (File.Exists(configPath))
            {
                string[] configLines = File.ReadAllLines(configPath);
                gamePath = configLines[0].Trim();
            }
            else
            {
                Console.WriteLine("Could not find Config.ini file. Make sure it's in the same directory as this Mod Loader.");
                Console.ReadKey();
                return;
            }



            string scriptsPath = Path.Combine(Environment.CurrentDirectory, "Scripts");
            if (!Directory.Exists(scriptsPath))
            {
                Console.WriteLine("Could not find Scripts directory. Make sure it's in the same directory as this Mod Loader.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Loading Scripts");

            Dictionary<string, bool> modList = new Dictionary<string, bool>();
            string iniPath = Path.Combine(Environment.CurrentDirectory, "scripthook.ini");
            if (File.Exists(iniPath))
            {
                string[] iniLines = File.ReadAllLines(iniPath);
                foreach (string iniLine in iniLines)
                {
                    string[] parts = iniLine.Split('=');
                    string modName = parts[0].Trim();
                    bool isEnabled = bool.Parse(parts[1].Trim());
                    modList[modName] = isEnabled;
                }
            }

            // Actualiza la lista de mods.
            foreach (string dllPath in Directory.GetFiles(scriptsPath, "*.dll"))
            {
                string modName = Path.GetFileNameWithoutExtension(dllPath);
                if (!modList.ContainsKey(modName))
                {
                    modList[modName] = true;
                }
            }

            // Escribe la lista de mods en el archivo INI.
            List<string> iniLinesList = new List<string>();
            foreach (string modName in modList.Keys)
            {
                iniLinesList.Add($"{modName}={modList[modName]}");
            }
            File.WriteAllLines(iniPath, iniLinesList);


            Console.WriteLine("Scripts Loaded");
            Console.WriteLine("Opening TLOU");
            Process.Start("steam://rungameid/1888930");
            Console.WriteLine(":)...");

            string processName = "tlou-i";
            Process[] processes = Process.GetProcessesByName(processName);
            bool processOpened = false;

            while (!processOpened)
            {
                Console.WriteLine("Waiting for game process to open...");
                processes = Process.GetProcessesByName(processName);

                if (processes.Length > 0)
                {
                    processOpened = true;
                }

                Thread.Sleep(1000);
            }

            int numModsActivated = 0;

            foreach (string dllPath in Directory.GetFiles(scriptsPath, "*.dll"))
            {
                string modName = Path.GetFileNameWithoutExtension(dllPath);
                bool isEnabled = modList[modName];

                if (isEnabled)
                {
                    try
                    {
                        // Get the game process.
                        Process gameProcess = processes[0];

                        // Inject the mod's DLL into the game process.
                        ProcessLoader.InjectDll(gameProcess.Id, dllPath);

                        numModsActivated++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error injecting {modName}: {ex.Message}");
                    }
                }
            }

            if (numModsActivated > 0)
            {
                Console.WriteLine("All scripts injected.");
            }
            else
            {
                foreach (string dllPath in Directory.GetFiles(scriptsPath, "*.dll"))
                {
                    string modName = Path.GetFileNameWithoutExtension(dllPath);
                    bool isEnabled = modList[modName];

                    if (isEnabled)
                    {
                        Console.WriteLine($"Injected {modName} successfully.");
                        numScriptsLoaded++;
                    }
                }
            }

            // Inicializar el cliente de Discord RPC
            discordRpcClient = new DiscordRpcClient("YOUR_DISCORD_APP_ID");
            discordRpcClient.Initialize();

            discordRpcClient.SetPresence(new RichPresence
            {
                Details = "Scripts Loaded",
                State = $"Mod Loader| {numScriptsLoaded} Scripts loaded in TLOU",
                Assets = new Assets
                {
                    LargeImageKey = "large_image_key",
                    LargeImageText = "Mods Loader"
                }
            });

            Console.WriteLine("Press ENTER to close the game and the program.");
            Console.ReadKey();

            foreach (var process in Process.GetProcessesByName("tlou-i"))
            {
                process.Kill();
            }
            Environment.Exit(0);
        }
    }
}