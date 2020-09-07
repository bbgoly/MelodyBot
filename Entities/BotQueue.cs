using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using MelodyBot.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MelodyBot.Entities
{
    public sealed class BotQueue
    {
        public LavalinkGuildConnection Connection { get; }

        public Queue<LavalinkTrack> TracksQueue { get; }

        public PlaybackMode Mode { get; set; }

        public BotQueue(LavalinkGuildConnection connection)
        {
            Connection = Connection;
            Mode = PlaybackMode.None;
            TracksQueue = new Queue<LavalinkTrack>(100);
        }

        public bool TryAddTrack(LavalinkTrack track)
        {
            //Audio audio = new Audio(track);
            //Audios.Add(audio);
            return true;
        }

        public bool TryAddTrack(IEnumerable<LavalinkTrack> track)
        {
            
            return true;
        }
    }
}
