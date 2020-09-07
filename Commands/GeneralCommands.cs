using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace MelodyBot.Commands
{
    [Group("General")]
    public class GeneralCommands : BaseCommandModule
    {
        [Command("Ping"), Description("Returns the bot's ping")]
        public async Task PingAsync(CommandContext ctx) => await ctx.Channel.SendMessageAsync($"{DiscordEmoji.FromName(ctx.Client, ":hourglass_flowing_sand:")}{ctx.Client.Ping}ms");

        [Command("Prefix"), Description("Displays the bot's prefix used in the server")]
        public async Task PrefixAsync(CommandContext ctx) => await ctx.Channel.SendMessageAsync($"The prefix is `>>`!\nUse it at the beginning of your message followed by a command to use me!\n\nDon't know what I can do? Use the `>>help` command to get started!");

        [Command("Prefix"), Description("Changes the bot's prefix in the server")]
        public async Task PrefixAsync(CommandContext ctx, string newPrefix) => await ctx.Channel.SendMessageAsync($"Sorry, You currently can't change my prefix!\n\nThis will be added in an incoming update.");

        [Command("Settings")]
        public async Task SettingsAsync(CommandContext ctx)
        {

        }

        //[Command("Settings")]
        //public async Task SettingsAsync(CommandContext ctx, GuildSetting setting)
        //{

        //}
    }
}
