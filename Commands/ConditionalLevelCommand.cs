using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/iflevel")]
    [Arguments("'-n'?", "command to run...?")]
    [Summary("Run a chat command (or directly send a message) only when greater or equal than certain level")]
    [Aliases("/whenlevel")]
    [HelpMessage(
        "Much like /ifcmd and /ifgp, this command executes a given command when the condition is met.",
        "In this case, the condition is whether or not the player is greater or equal than the given level.",
        "If you pass the -n (NOT) flag, the match will be inverted."
    )]
    public static void RunIfLevel(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        string args = !string.IsNullOrEmpty(argLine) ? argLine : string.Empty;

        string levelStr = args.Split()[0];
        if (!uint.TryParse(levelStr, out uint level) || level > 80)
        {
            ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Invalid level", ChatColour.RESET);
            return;
        }

        string cmd = args[levelStr.Length..].Trim();
        bool match = (VelaraUtils.Client.LocalPlayer.Level >= level) ^ flags["n"];

        if (cmd.Length > 0)
        {
            if (match) ChatUtil.SendChatLineToServer(cmd);
            return;
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "You are currently level ",
            ChatColour.RESET,
            match ? ChatColour.GREEN : ChatColour.RED,
            level.ToString(),
            ChatColour.RESET
        );
    }
}
