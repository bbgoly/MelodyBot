using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MelodyBot.Commands
{
    public sealed class HelpFormatter : BaseHelpFormatter
    {
        private DiscordEmbedBuilder EmbedBuilder { get; }

        private Dictionary<Type, string> FriendlyTypeNames { get; }

        private Command CurrentCommand { get; set; }

        public HelpFormatter(CommandContext ctx) : base(ctx)
        {
            EmbedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Help")
                .WithColor(0x007FFF)
                .WithThumbnail(ctx.Member.AvatarUrl);

            FriendlyTypeNames = new Dictionary<Type, string>()
            {
                [typeof(string)] = "text",
                [typeof(bool)] = "boolean",
                [typeof(sbyte)] = "8-bit integer",
                [typeof(byte)] = "positive 8-bit integer",
                [typeof(short)] = "16-bit integer",
                [typeof(ushort)] = "positive 16-bit integer",
                [typeof(int)] = "integer",
                [typeof(uint)] = "positive integer",
                [typeof(long)] = "64-bit integer",
                [typeof(ulong)] = "positive 64-bit integer",
                [typeof(float)] = "decimal",
                [typeof(double)] = "decimal",
                [typeof(decimal)] = "decimal",
                [typeof(DateTime)] = "date & time",
                [typeof(DateTimeOffset)] = "date & time",
                [typeof(TimeSpan)] = "time span",
                [typeof(Uri)] = "url",
                [typeof(DiscordUser)] = "user",
                [typeof(DiscordMember)] = "member",
                [typeof(DiscordRole)] = "role",
                [typeof(DiscordChannel)] = "channel",
                [typeof(DiscordGuild)] = "guild",
                [typeof(DiscordMessage)] = "message",
                [typeof(DiscordEmoji)] = "emoji",
                [typeof(DiscordColor)] = "color"
            };
        }

        public override CommandHelpMessage Build()
        {
            if (CurrentCommand is null)
            {
                EmbedBuilder.WithDescription("The following is a list of all the things you can do with me!\n\nYou can specify a group to narrow down the list of commands, or you can specify a command for more information on it!");
            }
            return new CommandHelpMessage(embed: EmbedBuilder.Build());
        }

        public override BaseHelpFormatter WithCommand(Command command)
        {
            CurrentCommand = command;
            EmbedBuilder.WithTitle($"Help - {command.Name}").WithDescription(command.Description ?? "No description provided");

            if (command is CommandGroup cGroup && cGroup.IsExecutableWithoutSubcommands)
            {
                EmbedBuilder.WithDescription($"{EmbedBuilder.Description}\n\nThis group can be executed as a standalone command to execute the {command.Name} command.");
            }

            if (command.Aliases?.Any() is true)
            {
                EmbedBuilder.AddField("Command Aliases", Formatter.InlineCode(string.Join(", ", command.Aliases)), false);
            }

            if (command.Overloads?.Any() is true)
            {
                bool hasArgs = true;
                StringBuilder stringBuilder = new StringBuilder();
                foreach (CommandOverload overload in command.Overloads.OrderByDescending(x => x.Priority))
                {
                    hasArgs = overload.Arguments.Count > 0;
                    stringBuilder.Append('`').Append(command.QualifiedName);
                    if (hasArgs)
                    {
                        foreach (CommandArgument arg in overload.Arguments)
                        {
                            stringBuilder.Append(arg.IsOptional || arg.IsCatchAll ? ": [" : ": <")
                                .Append(arg.Name)
                                .Append(arg.IsCatchAll ? "..." : "")
                                .Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');
                        }
                    }
                    stringBuilder.Append("`\n");
                }

                EmbedBuilder.AddField("Command Arguments", stringBuilder.ToString().Trim());
                stringBuilder.Clear();

                if (hasArgs)
                {
                    foreach (CommandOverload overload in command.Overloads.OrderByDescending(x => x.Priority))
                    {
                        foreach (CommandArgument arg in overload.Arguments)
                        {
                            stringBuilder.Append($"`{arg.Name} [{ConvertTypeName(arg.Type)}]: {arg.Description ?? "No description provided"}`\n");
                        }
                    }
                    EmbedBuilder.AddField("\nArgument Descriptions", stringBuilder.ToString().Trim());
                }
            }
            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            List<Command> commands = new List<Command>(subcommands);
            foreach (Command command in subcommands)
            {
                if (command is CommandGroup cGroup)
                {
                    EmbedBuilder.AddField(command.QualifiedName, string.Join("\n", cGroup.Children.Select(x => Formatter.InlineCode(x.Name))));
                    commands.Remove(command);
                }
            }
            EmbedBuilder.AddField(CurrentCommand is null ? "Commands" : "Subcommands", string.Join("\n", commands.Select(x => Formatter.InlineCode(x.Name))));
            return this;
        }

        private string ConvertTypeName(Type type)
        {
            if (FriendlyTypeNames.ContainsKey(type))
            {
                return $"{type.Name.ToLower()} ({FriendlyTypeNames[type]})";
            }

            TypeInfo typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type genericType = typeInfo.GenericTypeArguments[0];
                return FriendlyTypeNames.ContainsKey(genericType) ? $"{genericType.Name.ToLower()} ({FriendlyTypeNames[genericType]})" : genericType.Name.ToLower();
            }
            return type.Name.ToLower();
        }

        //private string GetOverloadedCommandArguments(Command command)
        //{
        //    var stringBuilder = new StringBuilder();
        //    foreach (CommandOverload overload in command.Overloads.OrderByDescending(x => x.Priority))
        //    {
        //        stringBuilder.Append("\n").Append($"{Melody.BotConfig.DefaultPrefix}{command.QualifiedName}").AppendLine(FormatArguments(overload.Arguments));
        //    }
        //    return stringBuilder.ToString();
        //}

        //private string FormatArguments(IReadOnlyList<CommandArgument> arguments)
        //{
        //    var stringBuilder = new StringBuilder();
        //    foreach (CommandArgument arg in arguments)
        //    {
        //        stringBuilder.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');
        //    }
        //    return stringBuilder.ToString();
        //}
    }
}
