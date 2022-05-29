using System;
using System.Globalization;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/ifcd")]
    [Arguments("condition flags", "action type", "action id", "command to run...?")]
    [Summary("Run a chat command (or directly send a message) only when certain action is on cooldown.")]
    [Aliases("/whencd")]
    [HelpMessage(
        "Much like /ifcmd and /ifgp, this command executes a given command when the condition is met.",
        "In this case, the condition is whether or not the given action is on cooldown.",
        "If you pass the -n (NOT) flag, the match will be inverted.",
        "If you pass the -r (RECAST) flag, the action must be on cooldown.",
        "If you pass the -c (CHARGE) flag, the action must not have charges remaining.",
        "If you pass neither the -r (RECAST) nor -c (CHARGE) flags, the -r (RECAST) flag will be used by default."
    )]
    public static unsafe void RunIfCooldown(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        string args = !string.IsNullOrEmpty(argLine) ? argLine : string.Empty;
        string[] argsArr = args.Split();

        if (argsArr.Length < 2)
        {
            ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Too few arguments", ChatColour.RESET);
            return;
        }

        string actionTypeStr = argsArr[0];
        if (!Enum.TryParse(actionTypeStr, true, out ActionType actionType))
        {
            ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Invalid action type", ChatColour.RESET);
            return;
        }

        string actionIdStr = argsArr[1];
        if (!uint.TryParse(actionIdStr, out uint actionId))
        {
            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "Invalid actionId: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                actionIdStr,
                ChatColour.RESET);
            return;
        }

        ActionManager* actionManager = ActionManager.Instance();
        if ((actionId = actionManager->GetAdjustedActionId(actionId)) == 0)
        {
            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "Invalid action: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                actionId.ToString(),
                ChatColour.RESET);
            return;
        }

        float recastTime = MathF.Min(0, actionManager->GetRecastTime(actionType, actionId));
        bool recastTimeMatch = MathF.Min(0, actionManager->GetRecastTime(actionType, actionId)) > 0;
        bool statusMatch = actionManager->IsRecastTimerActive(actionType, actionId);
        // bool statusMatch = actionManager->GetActionStatus(actionType, actionId) == 582U;

        string cmd = string.Join(' ', argsArr.Skip(2));
        bool match = (flags["c"] && statusMatch || flags["r"] && recastTimeMatch || !flags["c"] && !flags["r"] && recastTimeMatch) ^ flags["n"];

        if (cmd.Length > 0)
        {
            if (match) ChatUtil.SendChatLineToServer(cmd);
            return;
        }

        if (flags["r"] || !flags["c"] && !flags["r"])
        {
            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "Cooldown of action ",
                ChatColour.RESET,
                ChatColour.PURPLE,
                actionId.ToString(),
                ChatColour.RESET,
                ChatColour.WHITE,
                " is ",
                recastTimeMatch ? ChatColour.GREEN : ChatColour.RED,
                recastTime.ToString(CultureInfo.InvariantCulture),
                ChatColour.RESET,
                ChatColour.WHITE,
                ".",
                ChatColour.RESET
            );
        }

        if (flags["c"])
        {
            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "Action ",
                ChatColour.RESET,
                ChatColour.PURPLE,
                actionId.ToString(),
                ChatColour.RESET,
                !statusMatch ? ChatColour.GREEN : ChatColour.RED,
                statusMatch ? " has no" : " has",
                ChatColour.RESET,
                ChatColour.WHITE,
                " charges left.",
                ChatColour.RESET
            );
        }
    }
}
