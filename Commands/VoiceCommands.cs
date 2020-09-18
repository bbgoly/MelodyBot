using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Lavalink;
using MelodyBot.Attributes;
using MelodyBot.Enums;
using MelodyBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MelodyBot.Commands
{
    [Group("Music"), Description("The bot's commands for music playback")]
    public sealed class VoiceCommands : BaseCommandModule
    {
        private LavalinkGuildConnection LavalinkConnection { get; set; }

        /// <summary>
        /// Overrides the default BeforeExecutionAsync method to assert whether the client is connected to a voice channel or if the user is connected to the same voice channel as the client.
        /// </summary>
        public async override Task BeforeExecutionAsync(CommandContext ctx)
        {
            DiscordChannel channel = ctx.Member.VoiceState?.Channel;
            if (channel is null)
            {
                await ctx.RespondAsync($"{DiscordEmoji.FromName(ctx.Client, ":no_entry:")} You must join a voice channel to use me!");
                throw new CommandFailedException();
            }

            if (ctx.Command.Name != "connect" && (ctx.Guild.CurrentMember?.VoiceState?.Channel != channel || LavalinkConnection is null))
            {
                await ctx.RespondAsync($"{DiscordEmoji.FromName(ctx.Client, ":no_entry:")} I can't find a voice channel to execute that command in!\n\nIf you're sure that I'm in a voice channel, make sure that you're connected to the same voice channel then execute the command again!");
                throw new CommandFailedException();
            }

            await base.BeforeExecutionAsync(ctx);
        }

        /// <summary>
        /// Retrieves the voice channel the client is currently connected to and connects to the voice channel if it already hasn't.
        /// </summary>
        [Command("Connect"), Aliases("Join", "Summon", "C", "J", "S"), Description("Connects the bot to a specified voice channel."), RequirePermissions(Permissions.UseVoice)]
        public async Task ConnectAsync(CommandContext ctx)
        {
            DiscordChannel channel = ctx.Member.VoiceState.Channel;
            if (LavalinkConnection?.Channel != channel)
            {
                LavalinkConnection = await Melody.LavalinkNode.ConnectAsync(channel);
                await ctx.RespondAsync($"Ready to play audio in {channel.Name}!");
            }
            else if (LavalinkConnection?.Channel == channel)
            {
                await ctx.RespondAsync("I'm already connected to that voice channel!");
            }
        }

        /// <summary>
        /// Retrieves the voice channel the client is currently connected to, if the client is in a voice channel, the client disconnects from it.
        /// </summary>
        [Command("Disconnect"), Aliases("Leave", "DC"), Description("Disconnects the bot from the current voice channel.")]
        public async Task DisconnectAsync(CommandContext ctx)
        {
            if (LavalinkConnection?.Channel == ctx.Member.VoiceState.Channel)
            {
                await LavalinkConnection.DisconnectAsync();
                await ctx.RespondAsync($"Disconnected from {LavalinkConnection.Channel.Name}!");
                LavalinkConnection = null;
            }
        }

        [Command("Play"), Aliases("P")]
        public async Task PlayAsync(CommandContext ctx, [RemainingText] string search)
        {
            bool isYURL = Regex.IsMatch(search, @"^(?:https?\:\/\/)?(?:www\.)?(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v\=|playlist\?list\=))(?:.*)"); // (?:[\w-]{10,12})(?:$|\&|\?\#).*
            Console.WriteLine(isYURL);
            bool isSURL = !isYURL && Regex.IsMatch(search, @"^(?:https?\:\/\/)?(?:www\.)?(?:soundcloud\.com|snd\.sc)\/(?:.*)$");
            LavalinkLoadResult loadResult = await Melody.LavalinkNode.Rest.GetTracksAsync(search, isSURL ? LavalinkSearchType.SoundCloud : LavalinkSearchType.Youtube);
            switch (loadResult.LoadResultType)
            {
                case LavalinkLoadResultType.TrackLoaded:
                    await ctx.RespondAsync("Huh. The TrackLoaded enum actually does something.. Could you please screenshot all of the context of the command you issued and send it to <@291305496550440961>? Thanks!");
                    break;
                case LavalinkLoadResultType.PlaylistLoaded:
                    await PlaylistLoadedAsync(ctx, loadResult.PlaylistInfo, loadResult.Tracks);
                    break;
                case LavalinkLoadResultType.SearchResult:
                    if (isYURL || isSURL) { await PlayTrackAsync(ctx, loadResult.Tracks.First()); }
                    else { await ResultsFoundAsync(ctx, loadResult.Tracks); }
                    break;
                case LavalinkLoadResultType.NoMatches:
                    await ctx.RespondAsync($"No results were found for **{search}**!");
                    break;
                case LavalinkLoadResultType.LoadFailed:
                    await ctx.RespondAsync("Track failed to load, please try again! If this problem persists, then please try disconnecting me and try to play the track again!");
                    break;
            }
        }

        private async Task PlayTrackAsync(CommandContext ctx, LavalinkTrack lavalinkTrack)
        {
            await LavalinkConnection.PlayAsync(lavalinkTrack);
            await ctx.RespondAsync($"Now playing track - **{lavalinkTrack.Title}** by **{lavalinkTrack.Author}!** :notes:");
        }

        private async Task ResultsFoundAsync(CommandContext ctx, IEnumerable<LavalinkTrack> lavalinkTracks)
        {
            InteractivityExtension interactivity = ctx.Client.GetInteractivity();
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Title = "Search Results",
                Description = $"This is what I found based on the search term you gave me!\n\nWhich one of these audios should I play?",
                Color = ctx.Guild.CurrentMember.Color,
                Timestamp = DateTime.UtcNow
            }.WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl).WithThumbnail(ctx.User.AvatarUrl);

            foreach (LavalinkTrack track in lavalinkTracks.Take(5))
            {
                embedBuilder.AddField($"{embedBuilder.Fields.Count + 1} - {track.Title}", track.Author);
            }

            await ctx.RespondAsync("Here is what I found!", embed: embedBuilder.Build());

            InteractivityResult<DiscordMessage> msg = await interactivity.WaitForMessageAsync(response => response.Author.Id == ctx.User.Id && response.Content.Contains('1', '2', '3', '4', '5', 'c'));
            if (!msg.TimedOut && msg.Result.Content != "c")
            {
                int index = int.Parse(msg.Result.Content);
                LavalinkTrack lavalinkTrack = lavalinkTracks.ElementAt(index - 1);
                await LavalinkConnection.PlayAsync(lavalinkTrack);
                await ctx.RespondAsync($"Now playing track #{index} - **{lavalinkTrack.Title}** by **{lavalinkTrack.Author}!** :notes:");
            }
            else
            {
                await ctx.RespondAsync($"No tracks have been added to the queue! ({(msg.TimedOut ? "Took too long" : "Canceled")})");
            }
        }

        private async Task PlaylistLoadedAsync(CommandContext ctx, LavalinkPlaylistInfo playlistInfo, IEnumerable<LavalinkTrack> lavalinkTracks)
        {
            if (playlistInfo.SelectedTrack > 0)
            {
                Console.WriteLine(lavalinkTracks.ElementAt(playlistInfo.SelectedTrack).Title);
            }
            else
            {
                Console.WriteLine(string.Join('\n', (object[])lavalinkTracks)); // Cringe object array casting so dumb compiler understands which overload im using
            }
        }

        [Command("Pause"), IsTrackNull("There are currently no tracks to pause!", "Paused the player!")]
        public async Task PauseAsync(CommandContext ctx)
        {
            await LavalinkConnection.PauseAsync();
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":pause_button:"));
        }

        [Command("Stop"), Aliases("St"), IsTrackNull("There are currently no tracks to stop!", "Stopped the player!")]
        public async Task StopAsync(CommandContext ctx)
        {
            await LavalinkConnection.StopAsync(); // Have it remove everything from queue etc.
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":stop_button:"));
        }

        [Command("Resume"), Aliases("Res", "Continue"), IsTrackNull("There are currently no tracks to resume!", "Resumed playback!")] // Add descriptions, params, etc.
        public async Task PlayAsync(CommandContext ctx)
        {
            await LavalinkConnection.ResumeAsync();
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":play_pause:"));
        }

        [Command("FastForward"), Aliases("FF", "+"), IsTrackNull("There are no tracks to fastforward!")]
        public async Task FastForwardAsync(CommandContext ctx, double offset = 5.0)
        {
            await LavalinkConnection.SeekAsync(LavalinkConnection.CurrentState.PlaybackPosition + TimeSpan.FromSeconds(Math.Abs(offset)));
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":fast_forward:"));
        }

        [Command("Rewind"), Aliases("R", "-"), IsTrackNull("There are no tracks to rewind!")]
        public async Task RewindAsync(CommandContext ctx, double offset = 5.0)
        {
            await LavalinkConnection.SeekAsync(LavalinkConnection.CurrentState.PlaybackPosition - TimeSpan.FromSeconds(Math.Abs(offset)));
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":rewind:"));
        }

        [Command("Restart"), Aliases("Re", "Again"), IsTrackNull("There are no tracks to restart!")]
        public async Task RestartAsync(CommandContext ctx)
        {
            await LavalinkConnection.SeekAsync(TimeSpan.Zero);
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":arrows_counterclockwise:"));
        }

        [Command("Playback"), Aliases("Mode", "M", "L")]
        public async Task SetPlaybackModeAsync(CommandContext ctx, PlaybackMode playbackMode = PlaybackMode.Single) => await ctx.RespondAsync($"Set playback to **{playbackMode.ToFriendly()}** mode");
    }
}

//[Command("Play")] // Add descriptions, params, etc.
//public async Task PlayAsync(CommandContext ctx, Uri url)
//{
//    LavalinkGuildConnection connection = Melody.LavalinkNode.GetConnection(ctx.Guild);
//    if (connection?.Channel == ctx.Member.VoiceState?.Channel)
//    {

//    }
//}

//[Command("Play")] // Add descriptions, params, etc.
//public async Task PlayAsync(CommandContext ctx, FileInfo file)
//{
//    LavalinkGuildConnection connection = Melody.LavalinkNode.GetConnection(ctx.Guild);
//    if (connection?.Channel == ctx.Member.VoiceState?.Channel)
//    {
//        LavalinkLoadResult loadResult = await Melody.LavalinkNode.Rest.GetTracksAsync(file);
//        LavalinkTrack track = loadResult.Tracks.First();
//        Console.WriteLine(track.Title);
//        await connection.PlayAsync(track);
//        await ctx.RespondAsync($"Now playing \"{track.Title}\" by {track.Author}! :notes:");
//    }
//}