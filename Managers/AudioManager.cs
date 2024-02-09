using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using BepInEx.Configuration;

namespace BoomboxPlaylists.Managers
{
    internal static class AudioManager
    {
        public static List<PlaylistManager> playlistManagers = new List<PlaylistManager>();

        public static int currentPlaylistID;

        public static bool finishedLoading = false;

        public static int currentSongID = 0;

        private static readonly string directory = Path.Combine(BepInEx.Paths.BepInExRootPath, "Custom Playlists");

        public static event Action OnAllSongsLoaded;

        private static readonly string[] defaultTooltip = new string[3]{ "Toggle Music : [LMB]", "Playlist : ", "Change Volume : [Ctrl]+[Shift]+[K]/[J]",};

        private static KeyCode[] conditionKeys = new KeyCode[2] { KeyCode.LeftShift, KeyCode.LeftControl };
        public static KeyboardShortcut lowerVolume = new KeyboardShortcut(KeyCode.J, conditionKeys);
        public static KeyboardShortcut increaseVolume = new KeyboardShortcut(KeyCode.K, conditionKeys);
        public static float volume = 1f;

        public static void Load()
        {
            if (playlistManagers.Count != 0)
            {
                return;
            }
            string[] playlistNames = Directory.GetDirectories(directory).Select(Path.GetFileName).ToArray();
            if (playlistNames.Length == 0)
            {
                BoomboxPlaylistsBase.LogError("No playlists detected");
                return;
            }
            foreach (string playlistName in playlistNames)
            {
                playlistManagers.Add(new PlaylistManager(playlistName));
            }
            int i = 0;
            foreach (PlaylistManager playlistManager in playlistManagers)
            {
                string[] songs = playlistManager.playlistSongsPaths;
                foreach (string song in songs)
                {
                    LoadAudioClip(song, i);
                }
                BoomboxPlaylistsBase.LogInfo("Loaded playlist " + playlistManager.playlistName);
                i++;
            }
        }

        private static void LoadAudioClip(string filePath, int playlistID)
        {
            if (GetAudioType(filePath) == AudioType.UNKNOWN)
            {
                BoomboxPlaylistsBase.LogError("Failed to load AudioClip from " + filePath + "!");
                return;
            }
            UnityWebRequest loader = UnityWebRequestMultimedia.GetAudioClip(filePath, GetAudioType(filePath));
            loader.SendWebRequest();
            while (!loader.isDone)
            {
            }
            if (loader.error != null)
            {
                BoomboxPlaylistsBase.LogError("Error loading clip from path: " + filePath);
                BoomboxPlaylistsBase.LogError(loader.error);
                return;
            }
            AudioClip content = DownloadHandlerAudioClip.GetContent(loader);
            if ((bool)content && content.loadState == AudioDataLoadState.Loaded)
            {
                content.name = Path.GetFileName(filePath);
                BoomboxPlaylistsBase.LogInfo("Loaded song " + content.name);
                playlistManagers[playlistID].clips.Add(content);
            }
            else
            {
                BoomboxPlaylistsBase.LogError("Failed to load clip at: " + filePath + "\nThis might be due to an mismatch between the audio codec and the file extension!");
            }
        }

        private static IEnumerator WaitForAllClips(List<Coroutine> coroutines, int playlistID)
        {
            foreach (Coroutine coroutine in coroutines)
            {
                yield return coroutine;
            }
            playlistManagers[playlistID].clips.Sort((AudioClip first, AudioClip second) =>  first.name.CompareTo(second.name));
            finishedLoading = true;
            OnAllSongsLoaded?.Invoke();
            OnAllSongsLoaded = null;
        }

        private static AudioType GetAudioType(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            switch (extension)
            {
                case ".wav":
                    return AudioType.WAV;
                case ".ogg":
                    return AudioType.OGGVORBIS;
                case ".mp3":
                    return AudioType.MPEG;
                default:
                    BoomboxPlaylistsBase.LogError("Unsupported extesion type: " + extension);
                    return AudioType.UNKNOWN;
            }
        }

        public static void ApplyClips(ref BoomboxItem __istance)
        {
            currentPlaylistID++;
            if (currentPlaylistID >= playlistManagers.Count)
            {
                currentPlaylistID = 0;
            }
            currentSongID = 0;
            __istance.musicAudios = playlistManagers[currentPlaylistID].clips.ToArray();
        }

        public static void SetTooltip(ref BoomboxItem __instance)
        {
            string[] tooltip = new string[3];
            Array.Copy(defaultTooltip, tooltip, defaultTooltip.Length);
            tooltip[1] += playlistManagers[currentPlaylistID].playlistName;
            __instance.itemProperties.toolTips = tooltip;
        }

        public static void ModifyVolume(bool increase, ref BoomboxItem __instance)
        {
            if (increase)
            {
                volume = Mathf.Clamp(volume + 0.01f, 0f, 1f);
                SetVolume(ref __instance);
            }
            else
            {
                volume = Mathf.Clamp(volume - 0.01f, 0f, 1f);
                SetVolume(ref __instance);
            }
        }

        public static void SetVolume(ref BoomboxItem __instance)
        {
            __instance.boomboxAudio.volume = volume;
        }

        public static void NextSong(ref BoomboxItem __instance, ref bool startMusic)
        {
            if (startMusic)
            {
                currentSongID++;
                if (currentSongID >= playlistManagers[currentPlaylistID].clips.Count)
                {
                    currentSongID = 0;
                }
                __instance.boomboxAudio.clip = __instance.musicAudios[currentSongID];
                __instance.boomboxAudio.pitch = 1f;
                __instance.boomboxAudio.Play();
                startMusic = false;
            }

        }
    }
}
