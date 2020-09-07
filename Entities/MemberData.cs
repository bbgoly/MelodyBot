using DSharpPlus.Entities;
using System.Collections.Generic;

namespace MelodyBot.Entities
{
    public class MemberData
    {
        public List<MemberPlaylist> Playlists { get; }

        public MemberData()
        {
            Playlists = new List<MemberPlaylist>(20);
        }

        public void AddPlaylist(string playlistName, bool connectedToSpotify = false)
        {
            Playlists.Add(new MemberPlaylist(playlistName, connectedToSpotify));
        }
    }
}