using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace MelodyBot
{
    public class CommandEvents
    {
        public static async Task HandleCommandsAsync(MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot && !e.Channel.IsPrivate)
            {
                if (!Melody.GuildsData.ContainsKey(e.Guild))
                {
                    Melody.GuildsData[e.Guild].RetrieveOrCreate();
                }

                // Implement PrefixResolver here
                int prefixLength = e.Message.Content.StartsWith("<@") ? e.Message.GetMentionPrefixLength(e.Client.CurrentUser) : e.Message.GetStringPrefixLength(Melody.BotConfig.DefaultPrefix);
                if (prefixLength > -1)
                {
                    CommandsNextExtension commands = e.Client.GetCommandsNext();
                    Command command = GetCommand(commands, e.Message.Content.Substring(prefixLength), out string args);
                    if (command != null)
                    {
                        CommandContext ctx = commands.CreateContext(e.Message, e.Message.Content.Substring(0, prefixLength), command, args);
                        _ = Task.Run(async () => await commands.ExecuteCommandAsync(ctx));
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":thumbsup:"));
                    }
                    else
                    {
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":thumbsdown:"));
                    }
                }
            }
            await Task.CompletedTask; // For now
        }

        /* \n\nYou need: {Formatter.InlineCode(string.Join(", ", "e"))}
         * public static async Task CommandErroredAsync(CommandErrorEventArgs e) => 
         *   await e.Context.RespondAsync(e.Exception is CommandFailedException ? $"You lack the permissions required to execute this command!" 
         *       : $"An error occured while executing the command!\n\n{Formatter.InlineCode(e.Command.QualifiedName)} threw an exception: `{e.Exception.GetType()} - {e.Exception.Message}`");
         */

        private static Command GetCommand(CommandsNextExtension commandsNext, string msg, out string args)
        {
            Command command = commandsNext.FindCommand(msg, out args);
            if (command is null)
            {
                foreach (Command rCommand in commandsNext.RegisteredCommands.Values)
                {
                    if (rCommand is CommandGroup cGroup && commandsNext.FindCommand($"{cGroup.Name} {msg}", out args) is Command gCommand && IsMatchingCommand(msg.ToLowerInvariant(), gCommand))
                    {
                        command = gCommand;
                        break;
                    }
                }
            }
            return command;
        }

        private static bool IsMatchingCommand(string msg, Command gCommand)
        {
            int gLength = gCommand.Name.Length - 1, i = 0;
            bool isGCommand = msg.Length - 1 >= gLength && msg[gLength] == gCommand.Name[gLength];
            while (!isGCommand && i < gCommand.Aliases.Count)
            {
                string alias = gCommand.Aliases[i++];
                int length = alias.Length - 1;
                isGCommand = msg.Length >= alias.Length && msg[length] == alias[length] && msg.Substring(0, alias.Length) == alias;
            }
            return isGCommand;
        }
    }
}