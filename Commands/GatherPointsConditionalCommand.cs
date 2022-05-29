using Dalamud.Game.ClientState.Objects.SubKinds;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/ifgp")]
    [Aliases("/gp", "/whengp")]
    [Arguments("condition flag", "GP to compare?", "command to run...?")]
    [Summary("Run a chat command (or directly send a message) only if GP meets condition")]
    [HelpMessage(
        "Similar to /ifcmd, but specifically checks numeric inequality conditions against your GP to allow running commands based on how much you have.",
        "There are three possible tests: at least (-g), less than (-l), and a simple at capacity (-c).",
        "If using -g or -l, the first argument should be a number to compare against. If using -c, ALL arguments are the command to run when your GP passes the check."
    )]
    public static void RunChatIfPlayerGp(string command, string args, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        PlayerCharacter? player = VelaraUtils.Client.LocalPlayer;
        uint gp = player.CurrentGp;
        if (player.MaxGp > 0 && flags["c"])
        {
            if (player.CurrentGp >= player.MaxGp)
            {
                if (args.Length > 0)
                {
                    ChatUtil.SendChatLineToServer(args);
                }
                else
                {
                    ChatUtil.ShowPrefixedMessage(
                        ChatColour.CONDITION_PASSED,
                        "GP is at capacity (",
                        ChatGlow.CONDITION_PASSED,
                        gp,
                        ChatGlow.RESET,
                        ")",
                        ChatColour.RESET
                    );
                }
            }
            else if (args.Length < 1)
            {
                ChatUtil.ShowPrefixedMessage(
                    ChatColour.CONDITION_FAILED,
                    "GP is below capacity (",
                    ChatGlow.CONDITION_FAILED,
                    gp,
                    ChatGlow.RESET,
                    ")",
                    ChatColour.RESET
                );
            }
        }
        else if (flags["g"] || flags["l"])
        {
            if (args.Length < 1)
            {
                ChatUtil.ShowPrefixedError("-g and -l both require a number to compare your current GP against");
            }
            else
            {
                string num = args.Split()[0];
                string cmd = args[num.Length..].Trim();
                if (int.TryParse(num, out int compareTo))
                {
                    if (flags["g"])
                    {
                        if (gp >= compareTo)
                        {
                            if (cmd.Length > 0)
                            {
                                ChatUtil.SendChatLineToServer(cmd);
                            }
                            else
                            {
                                ChatUtil.ShowPrefixedMessage(
                                    ChatColour.CONDITION_PASSED,
                                    $"GP is at least {compareTo} (",
                                    ChatGlow.CONDITION_PASSED,
                                    gp,
                                    ChatGlow.RESET,
                                    ")",
                                    ChatColour.RESET
                                );
                            }
                        }
                        else if (cmd.Length < 1)
                        {
                            ChatUtil.ShowPrefixedMessage(
                                ChatColour.CONDITION_FAILED,
                                $"GP is below {compareTo} (",
                                ChatGlow.CONDITION_FAILED,
                                gp,
                                ChatGlow.RESET,
                                ")",
                                ChatColour.RESET
                            );
                        }
                    }
                    else if (flags["l"])
                    {
                        if (gp < compareTo)
                        {
                            if (cmd.Length > 0)
                            {
                                ChatUtil.SendChatLineToServer(cmd);
                            }
                            else
                            {
                                ChatUtil.ShowPrefixedMessage(
                                    ChatColour.CONDITION_PASSED,
                                    $"GP is below {compareTo} (",
                                    ChatGlow.CONDITION_PASSED,
                                    gp,
                                    ChatGlow.RESET,
                                    ")",
                                    ChatColour.RESET
                                );
                            }
                        }
                        else if (cmd.Length < 1)
                        {
                            ChatUtil.ShowPrefixedMessage(
                                ChatColour.CONDITION_FAILED,
                                $"GP is above {compareTo} (",
                                ChatGlow.CONDITION_FAILED,
                                gp,
                                ChatGlow.RESET,
                                ")",
                                ChatColour.RESET
                            );
                        }
                    }
                }
                else
                {
                    ChatUtil.ShowPrefixedError($"Couldn't parse \"{num}\" as an integer");
                    showHelp = true;
                }
            }
        }
        else
        {
            ChatUtil.ShowPrefixedError("Expected one of -c, -g, or -l, but found none");
            showHelp = true;
        }
    }
}
