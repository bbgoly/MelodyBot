using DSharpPlus.Lavalink;
using System;
using System.Collections.Generic;
using System.Text;

namespace MelodyBot.Entities
{
    public struct Audio
    {
        public LavalinkTrack LavalinkTrack { get; }

        public Audio(LavalinkTrack lavalinkTrack)
        {
            LavalinkTrack = lavalinkTrack;
        }
    }
}
