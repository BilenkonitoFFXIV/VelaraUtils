using VelaraUtils.Attributes;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/noop")]
    [Summary("Does absolutely nothing. At all. Literally not a single thing.")]
    [Aliases("/nop", "/null")]
    [HelpMessage(
        "This command does literally nothing at all.",
        "Its only purpose is for use with Macrology, to allow macro-macros to use <wait.(delay)> without doing anything."
    )]
    public static void EmptyCommand(string command, string args, FlagMap flags, ref bool showHelp)
    {
    }
}
