using System.Collections.Generic;
using System.Threading.Tasks;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/chain")]
    [Arguments("delay in seconds?", "commands to run...?")]
    [Summary("Run a series of chat command (or directly send messages) in one line")]
    [Aliases("/dochain")]
    [HelpMessage(
        "Chains a series of chat commands in one line.",
        "Commands will be executed immediately one after the other.",
        "Commands shoulde be separated by vertical bars (|)."
    )]
    public static void RunChain(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        string cmds = string.Join(' ', CommandArgumentParser.Parse(argLine, out string delay)).Trim();
        if (!float.TryParse(delay, out float fDelay))
        {
            ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Invalid delay.", ChatColour.RESET);
            return;
        }

        List<string> cmd = CommandArgumentParser.Parse('|', cmds);
        if (cmd.Count > 0)
            if (fDelay < float.Epsilon)
                ChatUtil.SendChatLinesToServer(cmd);
            else
                Task.Run(async () => await ChatUtil.SendChatLinesToServer(fDelay, cmd));
        else
            ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Too few arguments", ChatColour.RESET);
    }
}
