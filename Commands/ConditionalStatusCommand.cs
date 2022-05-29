using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/ifhp")]
    [Arguments("condition flags", "value", "command to run...?")]
    [Summary("Run a chat command (or directly send a message) only when HP matches the given amount")]
    [Aliases("/whenhp")]
    [HelpMessage(
        "Much like /ifcmd and /ifgp, this command executes a given command when the condition is met.",
        "In this case, the condition is whether or not the HP of the player or target matches the given amount.",
        "Use the HP percent in decimal form [0.0 - 1.0]. Can be a variable.",
        "If you pass the -t (TARGET) flag, the match will be done based on the target instead of player.",
        "If you pass the -m (MOUSEOVER) flag, the match will be done based on the mouseover target instead of player.",
        "If you pass the -n (NOT) flag, the match will be inverted.",
        "If you pass the -e (EQUAL) flag, the HP must be equal to the given amount.",
        "If you pass the -l (LESS) flag, the HP must be less than the given amount.",
        "If you pass the -g (GREATER) flag, the HP must be greater than the given amount.",
        "If you pass neither the -e (EQUAL), -l (LESS) nor the -g (GREATER) flags, the -e (EQUAL) flag will be used by default.",
        "The -l (LESS) and -g (GREATER) flags can be combined with the -e (EQUAL) flag."
    )]
    public static void RunIfHp(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        GameObject? target =
            flags["t"] ?
                VelaraUtils.TargetManager.Target :
                flags["m"] ?
                    VelaraUtils.TargetManager.MouseOverTarget :
                    VelaraUtils.Client.LocalPlayer;

        float targetHp =
            target != null && target.ObjectKind is ObjectKind.Player or ObjectKind.BattleNpc ?
                (float)((BattleChara)target).CurrentHp / ((BattleChara)target).MaxHp :
                0f;

        List<string> cmd = CommandArgumentParser.Parse(argLine, out string value);
        if (!string.IsNullOrEmpty(value))
        {
            if (!float.TryParse(value, out float fValue))
            {
                ChatUtil.ShowPrefixedError($"Invalid HP: {value}.");
                return;
            }

            ComparisonOperation operation =
                flags["g"] ?
                    flags["e"] ?
                        ComparisonOperation.GREATER_OR_EQUAL :
                        ComparisonOperation.GREATER :
                    flags["l"] ?
                        flags["e"] ?
                            ComparisonOperation.LESS_OR_EQUAL :
                            ComparisonOperation.LESS :
                        ComparisonOperation.EQUAL;

            bool match = operation switch
            {
                ComparisonOperation.GREATER_OR_EQUAL => targetHp >= fValue,
                ComparisonOperation.GREATER => targetHp > fValue,
                ComparisonOperation.LESS_OR_EQUAL => targetHp <= fValue,
                ComparisonOperation.LESS => targetHp < fValue,
                _ => Math.Abs(targetHp - fValue) < float.Epsilon
            } ^ flags["n"];

            if (cmd.Count > 0)
            {
                if (match)
                    ChatUtil.SendChatLineToServer(string.Join(" ", cmd));
                return;
            }

            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "HP: ",
                ChatColour.RESET,
                targetHp < 0.25f ?
                    ChatColour.RED :
                    targetHp < 0.5f ?
                        ChatColour.ORANGE :
                        ChatColour.GREEN,
                targetHp,
                ChatColour.RESET,
                ChatColour.WHITE,
                ", ",
                ChatColour.RESET,
                match ?
                    ChatColour.TRUE :
                    ChatColour.FALSE,
                operation.ToString(),
                ChatColour.RESET,
                ChatColour.WHITE,
                ", Value: ",
                ChatColour.BLUE,
                fValue,
                ChatColour.RESET,
                ChatColour.WHITE,
                ".",
                ChatColour.RESET
            );
            return;
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "HP: ",
            ChatColour.RESET,
            targetHp < 0.25f ?
                ChatColour.RED :
                targetHp < 0.5f ?
                    ChatColour.ORANGE :
                    ChatColour.GREEN,
            targetHp,
            ChatColour.RESET,
            ChatColour.WHITE,
            ".",
            ChatColour.RESET
        );
    }

    [Command("/ifstatus")]
    [Arguments("condition flags", "status ids to match against", "command to run...?")]
    [Summary("Run a chat command (or directly send a message) only when afflicted by all given statuses")]
    [Aliases("/whenstatus")]
    [HelpMessage(
        "Much like /ifcmd and /ifgp, this command executes a given command when the condition is met.",
        "In this case, the condition is whether or not the player or target is afflicted by all of the given status effects.",
        "Use the id of the statuses separated by commas.",
        "If you pass the -t (TARGET) flag, the match will be done based on the target instead of player.",
        "If you pass the -m (MOUSEOVER) flag, the match will be done based on the mouseover target instead of player.",
        "If you pass the -n (NOT) flag, the match will be inverted.",
        "If you pass the -a (AND) flag, all statuses must match.",
        "If you pass the -o (OR) flag, at least one status must match.",
        "If you pass neither the -a (AND) nor -o (OR) flags, the -o (OR) flag will be used by default."
    )]
    public static void RunIfStatus(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        GameObject? target =
            flags["t"] ?
                VelaraUtils.TargetManager.Target :
                flags["m"] ?
                    VelaraUtils.TargetManager.MouseOverTarget :
                    VelaraUtils.Client.LocalPlayer;

        uint[] targetStatusIdArray =
            target != null && target.ObjectKind is ObjectKind.Player or ObjectKind.BattleNpc ?
                ((BattleChara)target).StatusList.Select(s => s.StatusId).ToArray() :
                Array.Empty<uint>();

        string args = !string.IsNullOrEmpty(argLine) ? argLine : string.Empty;

        string statusIds = args.Split()[0];
        uint[] statusIdArray = statusIds.Split(',')
            .Select<string, uint?>(id => uint.TryParse(id, out uint result) ? result : null)
            .Where(id => id != null)
            .Select(id => id!.Value)
            .ToArray();

        string cmd = args[statusIds.Length..].Trim();
        bool match = flags["a"] ?
            statusIdArray.All(id => targetStatusIdArray.Contains(id)) :
            statusIdArray.Any(id => targetStatusIdArray.Contains(id));

        if (match ^ flags["n"] && cmd.Length > 0) {
            ChatUtil.SendChatLineToServer(cmd);
        }
    }

    // private static readonly Regex CmdExperimentalPattern = new(@"^[^\(\)]*(((?'Open'\()[^\(\)]*)+((?'Close-Open'\))[^\(\)]*)+)*(?(Open)(?!))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    // private static readonly Regex CmdTruePattern = new(@"(?<!\\)\@([^\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    // private static readonly Regex CmdFalsePattern = new(@"(?<!\\)\!", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    // private static readonly Regex CmdEndPattern = new(@"(?<!\\)\¤|\Z", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // private static readonly Regex XmlCmdBase64Pattern = new(@"^(?:\s|[\\][\*])*(?()$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    [Command("/ifautorun")]
    [Summary("")]
    [Arguments("flags", "result variable name")]
    [HelpMessage("Supported flags: (n)ot")]
    public static void ClearTargetCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        bool match = InputManager.IsAutoRunning();

        argLine = argLine.Trim();
        if (!string.IsNullOrEmpty(argLine))
        {
            string[] argArr = argLine.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (argArr.Length > 0)
            {
                string varName = argArr[0].ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);
                if (!string.IsNullOrEmpty(varName))
                {
                    VelaraUtils.VariablesConfiguration.Variables[varName] =
                        match ^ flags["n"] ?
                            "true" :
                            "false";
                    return;
                }
            }
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "Autorun is ",
            ChatColour.RESET,
            match ?
                ChatColour.CONDITION_PASSED :
                ChatColour.CONDITION_FAILED,
            match ?
                "enabled" :
                "disabled",
            ChatColour.RESET,
            ChatColour.WHITE,
            ".",
            ChatColour.RESET);

        // ChatUtil.ShowPrefixedMessage(ChatColour.WHITE, "Input:  ", ChatColour.OUTGOING_TEXT, argLine, ChatColour.RESET);
        // Match m = CmdExperimentalPattern.Match(argLine);
        // if (m.Success)
        // {
        //     ChatUtil.ShowMessage(ChatColour.WHITE, "Match: ", ChatColour.RESET, ChatColour.CONDITION_PASSED, m.ToString(), ChatColour.RESET);
        //     int grpCtr = 0;
        //     foreach (Group grp in m.Groups)
        //     {
        //         ChatUtil.ShowMessage(ChatColour.WHITE, "   Group ", ChatColour.RESET, ChatColour.LIGHTBLUE, grpCtr.ToString(), ChatColour.RESET, ": ", ChatColour.OUTGOING_TEXT, grp.Value, ChatColour.RESET);
        //         grpCtr++;
        //         int capCtr = 0;
        //         foreach (Capture cap in grp.Captures)
        //         {
        //             ChatUtil.ShowMessage(ChatColour.WHITE, "      Capture ", ChatColour.RESET, ChatColour.INDIGO, capCtr.ToString(), ChatColour.RESET, ": ", ChatColour.OUTGOING_TEXT, cap.Value, ChatColour.RESET);
        //             capCtr++;
        //         }
        //     }
        // }
        // else
        // {
        //     ChatUtil.ShowError(ChatColour.WHITE, "Match: ", ChatColour.RESET, ChatColour.CONDITION_FAILED, "fail", ChatColour.RESET);
        // }
    }
}
