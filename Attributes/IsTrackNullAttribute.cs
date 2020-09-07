using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;

namespace MelodyBot.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class IsTrackNullAttribute : CheckBaseAttribute
    {
        public string ErrorResponse { get; }

        public string ValidResponse { get; }

        public async override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if (!help)
            {
                bool trackNull = Melody.LavalinkNode.GetGuildConnection(ctx.Guild).CurrentState.CurrentTrack is null;
                if (trackNull)
                {
                    await ctx.RespondAsync(ErrorResponse);
                }
                else if (ValidResponse != null)
                {
                    await ctx.RespondAsync(ValidResponse);
                }
                return !trackNull;
            }
            return true;
        }

        public IsTrackNullAttribute(string ErrorResponse)
        {
            this.ErrorResponse = ErrorResponse;
        }

        public IsTrackNullAttribute(string ErrorResponse, string ValidResponse)
        {
            this.ErrorResponse = ErrorResponse;
            this.ValidResponse = ValidResponse;
        }
    }
}
