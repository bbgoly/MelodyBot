using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MelodyBot.Entities
{
    public class GuildData
    {
        public Dictionary<DiscordUser, MemberData> Members { get; }

        public BotQueue Queue { get; private set; }

        public GuildSettings GuildSettings { get; private set; }

        public GuildData()
        {
            Members = new Dictionary<DiscordUser, MemberData>();
            GuildSettings = new GuildSettings();
        }

        public void AddMember(DiscordUser member)
        {
            Members.Add(member, new MemberData());
        }

        public void RetrieveOrCreate()
        {
            
        }
    }

    public struct GuildSettings
    {
        public List<string> GuildPrefixes { get; }

        public Audio? CurrentlyPlaying { get; set; }

        public bool AllowDuplicates { get; set; }

        public GuildSettings(Audio? audio = null)
        {
            GuildPrefixes = new List<string>() { Melody.BotConfig.DefaultPrefix };
            CurrentlyPlaying = audio;
            AllowDuplicates = true;
        }
    }
}
