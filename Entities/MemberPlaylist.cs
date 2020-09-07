using System.Collections.Generic;

namespace MelodyBot.Entities
{
    public struct MemberPlaylist
    {
        public string PlaylistName { get; }

        public List<Audio> Audios { get; }

        public MemberPlaylist(string playlistName, bool connectedToSpotify = false)
        {
            PlaylistName = playlistName;
            Audios = new List<Audio>(150);
        }

        //public void AddAudio(string audioName) => Audios.Add(new Audio(audioName));

        public void RemoveAudio(Audio audio) => Audios.Remove(audio);
    }
}