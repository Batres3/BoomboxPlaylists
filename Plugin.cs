using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace BoomboxPlaylists
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class BoomboxPlaylistsBase : BaseUnityPlugin
    {
        private const string modGUID = "Batres3.BoomboxPlaylists";
        private const string modName = "Boombox Playlists";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static BoomboxPlaylistsBase Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            if (!Directory.Exists(Path.Combine(BepInEx.Paths.BepInExRootPath, "Custom Playlists")))
            {
                Directory.CreateDirectory(Path.Combine(BepInEx.Paths.BepInExRootPath, "Custom Playlists"));
                LogError("No custom playlists detected, they must be in the 'Custom Playlists' folder");
                return;
            }
            harmony.PatchAll();
            Managers.AudioManager.Load();
            LogInfo("Boombox Playlists Loaded!");
        }

        internal static void LogInfo(string message) 
        {
            Instance.Log(message, (LogLevel)16);
        }

        internal static void LogError(string message)
        {
            Instance.Log(message, (LogLevel)2);
        }

        private void Log(string message, LogLevel logLevel)
        {
            this.Logger.Log(logLevel, message);
        }
    }
}
