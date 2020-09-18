using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using MelodyBot.Commands;
using MelodyBot.Entities;
using MelodyBot.Enums;
using MelodyBot.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MelodyBot
{
    public class Melody
    {
        private static DiscordClient Client { get; }

        public static BotConfiguration BotConfig { get; }

        private static CommandsNextExtension Commands { get; }

        public static Dictionary<DiscordGuild, GuildData> GuildsData { get; }

        public static LavalinkNodeConnection LavalinkNode { get; private set; }

        static Melody()
        {
            BotConfig = BotConfiguration.Setup();
            Client = new DiscordClient(new DiscordConfiguration
            {
                AutoReconnect = true,
                Token = BotConfig.Token,
                TokenType = TokenType.Bot
            });

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                EnableDms = false,
                StringPrefixes = new string[] { BotConfig.DefaultPrefix },
                CaseSensitive = false,
                UseDefaultCommandHandler = false
            });

            Client.UseInteractivity(new InteractivityConfiguration());
            
            GuildsData = new Dictionary<DiscordGuild, GuildData>();
        }

        public async Task RunAsync()
        {
            Client.Ready += ClientEvents.OnReadyAsync;
            Client.ClientErrored += ClientEvents.ClientErroredAsync;
            Client.GuildAvailable += ClientEvents.GuildAvailableAsync;
            Client.GuildDownloadCompleted += ClientEvents.GuildDownloadCompletedAsync;

            Client.GuildCreated += ClientEvents.GuildCreatedAsync;
            Client.GuildDeleted += ClientEvents.GuildDeletedAsync;
            Client.GuildUnavailable += ClientEvents.GuildUnavailableAsync;

            Client.MessageCreated += CommandEvents.HandleCommandsAsync;

            Commands.RegisterCommands(GetType().Assembly);
            Commands.SetHelpFormatter<HelpFormatter>();

            Console.CancelKeyPress += ProgramExitAsync;
            AppDomain.CurrentDomain.ProcessExit += ProgramExitAsync;

            Commands.RegisterConverter<PlaybackMode>(new EnchancedPlaybackModeConverter());
            Commands.RegisterUserFriendlyTypeName<PlaybackMode>("Playback Mode");

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1",
                Port = 2333
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "}QSD_y=jbbmkzwvKWFkRe",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };
            
            var lavalink = Client.UseLavalink();

            await Client.ConnectAsync();
            LavalinkNode = await lavalink.ConnectAsync(lavalinkConfig);
            await Task.Delay(-1);
        }

        private async void ProgramExitAsync(object sender, EventArgs e)
        {
            foreach (DiscordGuild guild in Client.Guilds.Values)
            {
                LavalinkGuildConnection connection = LavalinkNode.GetGuildConnection(guild);
                if (connection != null)
                {
                    await connection.DisconnectAsync();
                }
            }
        }
    }
}