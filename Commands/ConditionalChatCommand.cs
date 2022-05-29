using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/ifcmd")]
    [Arguments("condition flags", "command to run...?")]
    [Summary("Run a chat command (or directly send a message) only if a condition is met")]
    [Aliases("/ifthen")]
    [HelpMessage(
        "If the condition indicated by the flags is met, then all of the arguments will be executed as if entered into the chatbox manually. If no command/message is given, the test will print the result to your chatlog.",
        "Lowercase flags require that their condition be met, uppercase flags require that their condition NOT be met. Available flags are:",
        "-t has target, -f has focus, -o has mouseover, -c in combat, -p target is player, -n target is NPC, -m target is minion"
    )]
    public static void RunChatIfCond(string command, string args, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        ChatColour msgCol = ChatColour.CONDITION_FAILED;
        string msg = "Test passed but no command given";
        if (flags["t"] && VelaraUtils.TargetManager.Target is null)
            msg = "No target";
        else if (flags["T"] && VelaraUtils.TargetManager.Target is not null)
            msg = "Target present";
        else if (flags["p"] && VelaraUtils.TargetManager.Target?.ObjectKind is not ObjectKind.Player)
            msg = "Target is not player";
        else if (flags["P"] && VelaraUtils.TargetManager.Target?.ObjectKind is ObjectKind.Player)
            msg = "Target is player";
        else if (flags["n"] && VelaraUtils.TargetManager.Target?.ObjectKind is not ObjectKind.BattleNpc or ObjectKind.EventNpc or ObjectKind.Retainer)
            msg = "Target is not NPC";
        else if (flags["N"] && VelaraUtils.TargetManager.Target?.ObjectKind is ObjectKind.BattleNpc or ObjectKind.EventNpc or ObjectKind.Retainer)
            msg = "Target is NPC";
        else if (flags["m"] && VelaraUtils.TargetManager.Target?.ObjectKind is not ObjectKind.Companion)
            msg = "Target is not minion";
        else if (flags["M"] && VelaraUtils.TargetManager.Target?.ObjectKind is ObjectKind.Companion)
            msg = "Target is minion";
        else if (flags["f"] && VelaraUtils.TargetManager.FocusTarget is null)
            msg = "No focus target";
        else if (flags["F"] && VelaraUtils.TargetManager.FocusTarget is not null)
            msg = "Focus target present";
        else if (flags["o"] && VelaraUtils.TargetManager.MouseOverTarget is null)
            msg = "No mouseover target";
        else if (flags["O"] && VelaraUtils.TargetManager.MouseOverTarget is not null)
            msg = "Mouseover target present";
        else if (flags["c"] && !(VelaraUtils.Conditions != null && VelaraUtils.Conditions[ConditionFlag.InCombat]))
            msg = "Not in combat";
        else if (flags["C"] && (VelaraUtils.Conditions != null && VelaraUtils.Conditions[ConditionFlag.InCombat]))
            msg = "In combat";
        else if (flags["a"] && !VelaraUtils.Client.LocalPlayer.IsCasting)
            msg = "Not casting";
        else if (flags["A"] && VelaraUtils.Client.LocalPlayer.IsCasting)
            msg = "Casting";
        else
            msgCol = ChatColour.CONDITION_PASSED;
        if (args.Length > 0)
        {
            if (msgCol == ChatColour.CONDITION_PASSED)
            {
                ChatUtil.SendChatLineToServer(args);
            }
        }
        else
        {
            ChatUtil.ShowPrefixedMessage(msgCol, msg, ChatColour.RESET);
        }
    }
}
