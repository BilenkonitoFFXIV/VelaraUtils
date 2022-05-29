using System.Linq;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Internal.IPC;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/echoerr")]
    [Summary("Like /echo, but to the error channel")]
    [Aliases("/echoerror", "/error")]
    [HelpMessage(
        "Functionally identical to the built-in /echo command, except that the output text is sent to the \"error\" chat channel instead.",
        "Mostly useful with the conditional chat commands to allow, for instance, an emote macro to warn you when you use it wrong.",
        "If you use the -p flag, the error message will be prefixed as coming from this plugin, instead of being a bare message."
    )]
    public static void EchoToErrorChannel(string command, string args, FlagMap flags, ref bool showHelp)
    {
        object[] message =
        {
            ChatColour.ERROR,
            args.Trim(),
            ChatColour.RESET
        };

        if (flags["p"])
        {
            ChatUtil.ShowPrefixedError(message);
            return;
        }

        message[1] = "Error: " + message[1];
        ChatUtil.ShowError(message);
    }

    [Command("/echodbg")]
    [Summary("Like /echo, but to the debug channel")]
    [Aliases("/echodebug", "/edebug")]
    [HelpMessage(
        "Functionally identical to the built-in /echo command, except that the output text is sent to the \"debug\" chat channel instead.",
        "Mostly useful to troubleshoot.",
        "If you use the -p flag, the error message will be prefixed as coming from this plugin, instead of being a bare message."
    )]
    public static void EchoToDebugChannel(string command, string args, FlagMap flags, ref bool showHelp)
    {
        object[] message =
        {
            ChatColour.DEBUG,
            args.Trim(),
            // " | ",
            // string.Join(';', VelaraUtils.QolBar.ConditionSets.Select(cSet => cSet.Name)),
            ChatColour.RESET
        };

        if (flags["p"])
            ChatUtil.ShowPrefixedMessage(message);
        else
            ChatUtil.ShowMessage(message);
    }
}
