using DSharpPlus;
using DSharpPlus.EventArgs;
using MelodyBot.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MelodyBot.Events
{
    public class ClientEvents
    {
        public static Task OnReadyAsync(ReadyEventArgs e)
        {
            e.Client.Logger.LogInformation($"[{e.Client.CurrentUser.Username}] [Compiled Successfully] - Listening to subscribed events");
            return Task.CompletedTask;
        }

        public static Task ClientErroredAsync(ClientErrorEventArgs e)
        {
            e.Client.Logger.LogError($"[{e.Client.CurrentUser.Username}] [{e.Exception.GetType()}] - {e.Exception.Message}");
            return Task.CompletedTask;
        }

        public static Task GuildAvailableAsync(GuildCreateEventArgs e)
        {
            e.Client.Logger.LogInformation($"[{e.Client.CurrentUser.Username}] [Guild Available] - {e.Guild.Name}; {e.Guild.Id}");
            return Task.CompletedTask;
        }

        public static Task GuildDownloadCompletedAsync(GuildDownloadCompletedEventArgs e)
        {
            e.Client.Logger.LogInformation($"[{e.Client.CurrentUser.Username}] [Guild Download] - All guilds have been successfully downloaded! ({e.Guilds.Count})");
            return Task.CompletedTask;
        }

        public static async Task GuildCreatedAsync(GuildCreateEventArgs e)
        {
            Melody.GuildsData.Add(e.Guild, new GuildData());
            await e.Guild.SystemChannel.SendMessageAsync("Hey there, thanks for adding me to your server!\nThere's a whole lot you can do with me, so make sure to check out my `help` command to see what I can do!\n\nThe default prefix is `>>`!");
        }

        /*
         * Todo: Add a property that is set to the date the bot left the guild, and check it every hour to see if 24 hours have passed to delete the data and update the json
         * 
         * Example: public DateTime? LeaveDate { get; private set; } = null;
         */
        public static async Task GuildDeletedAsync(GuildDeleteEventArgs e) => await Task.CompletedTask;

        // Todo: Update json to show that the guild was removed
        public static Task GuildUnavailableAsync(GuildDeleteEventArgs e)
        {
            Melody.GuildsData.Remove(e.Guild);
            return Task.CompletedTask;
        }

        //public static async Task MessageCreatedAsync(MessageCreateEventArgs e)
        //{
        //    if (!Melody.GuildsData.ContainsKey(e.Guild))
        //    {
        //        Melody.GuildsData.Add(e.Guild, new GuildData());
        //        Melody.GuildsData[e.Guild].AddMember(e.Author);
        //    }
        //    else if (!Melody.GuildsData[e.Guild].Members.ContainsKey(e.Author))
        //    {
        //        Melody.GuildsData[e.Guild].AddMember(e.Author);
        //    }
        //    await Task.CompletedTask;
        //}
    }
}
