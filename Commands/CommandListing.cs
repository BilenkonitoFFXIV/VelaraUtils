using System;
using System.Linq;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Internal.Command;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/vucmds")]
    [Summary("List all plugin commands, along with their help messages")]
    [HelpMessage(
        "Lists all plugin commands.",
        "Use \"-a\" to include command aliases, \"-v\" to include help messages, or both (\"-av\" or \"-va\" or separately) for both."
    )]
    public static void ListPluginCommands(string command, string args, FlagMap flags, ref bool showHelp)
    {
        foreach (var cmd in VelaraUtils.CommandManager?.CommandModules.SelectMany(module => module.Commands) ?? Array.Empty<PluginCommand>())
        {
            ChatUtil.ShowPrefixedMessage(
                ChatColour.USAGE_TEXT,
                cmd.Usage,
                ChatColour.RESET
            );

            // if (flags["a"] && cmd.Aliases.Length > 0) {
            ChatUtil.ShowPrefixedMessage(
                ChatColour.QUIET,
                string.Join(", ", cmd.Aliases),
                ChatColour.RESET
            );
            // }

            if (!flags["v"]) continue;

            foreach (var line in cmd.HelpLines)
                ChatUtil.ShowPrefixedMessage(
                    ChatColour.HELP_TEXT,
                    line,
                    ChatColour.RESET
                );
        }
    }
}
