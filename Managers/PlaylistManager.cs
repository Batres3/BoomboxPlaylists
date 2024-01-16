using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BoomboxPlaylists.Managers
{
    internal class PlaylistManager
    {
        public string[] playlistSongsPaths;
        public List<AudioClip> clips = new List<AudioClip>();
        private readonly string playlistPath;
        public bool hasNoSongs => playlistSongsPaths.Length == 0;
        public string playlistName;

        internal PlaylistManager(string playlistname)
        {
            playlistName = playlistname;
            playlistPath = Path.Combine(BepInEx.Paths.BepInExRootPath, "Custom Playlists", playlistName);
            playlistSongsPaths = Directory.GetFiles(playlistPath);
            if (hasNoSongs) 
            {
                BoomboxPlaylistsBase.LogError("No songs found for playlist " + playlistName);
                return; 
            }
        }
    }
}
