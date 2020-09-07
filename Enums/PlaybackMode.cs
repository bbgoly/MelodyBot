using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace MelodyBot.Enums
{
    /// <summary>
    /// Represents the bot's playback behaviour
    /// </summary>
    public enum PlaybackMode
    {
        /// <summary>
        /// Normal playback behaviour; nothing will be looped
        /// </summary>
        None,

        /// <summary>
        /// The currently playing track will be looped
        /// </summary>
        Single,

        /// <summary>
        /// The entire queue will be looped
        /// </summary>
        All,

        /// <summary>
        /// The bot will randomly select a new song to play and will loop the queue once it has played through each song within the queue
        /// </summary>
        Shuffle
    }

    public sealed class EnchancedPlaybackModeConverter : IArgumentConverter<PlaybackMode>
    {
        public async Task<Optional<PlaybackMode>> ConvertAsync(string value, CommandContext ctx) => (value.ToLowerInvariant()) switch
        {
            "-loop" => Optional.FromValue<PlaybackMode>(PlaybackMode.Single),
            "-l" => Optional.FromValue<PlaybackMode>(PlaybackMode.Single),
            "-queue" => Optional.FromValue<PlaybackMode>(PlaybackMode.All),
            "-q" => Optional.FromValue<PlaybackMode>(PlaybackMode.All),
            "-shuffle" => Optional.FromValue<PlaybackMode>(PlaybackMode.Shuffle),
            "-s" => Optional.FromValue<PlaybackMode>(PlaybackMode.Shuffle),
            _ => Optional.FromValue<PlaybackMode>(PlaybackMode.None)
        };
    }

    public static class PlaybackModeExtension
    {
        public static string ToFriendly(this PlaybackMode playbackMode) => playbackMode switch
        {
            PlaybackMode.All => "Queue Loop",
            PlaybackMode.Single => "Track Loop",
            PlaybackMode.Shuffle => "Shuffle & Queue Loop",
            _ => "Normal"
        };
    }
}
