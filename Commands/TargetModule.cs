using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Internal.Command;
using VelaraUtils.Internal.Target;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

[CommandModule("Target", "target")]
public class TargetModule : ICommandModule
{
    [Command("clear")]
    [Summary("Clears the current target")]
    [Arguments("flags")]
    [HelpMessage("Supported flags: (v)erbose, (t)arget, (s)oft target, (m)ouseover target, (f)ocus target, (p)revious target",
                 "Default: (t)arget",
                 "Note: Only one target type flag can be used at the same time. The (v)erbose flag can be combined without restrictions.")]
    public static void ClearTargetCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        TargetType targetType = TargetType.NormalTarget;
        if (flags["s"]) targetType = TargetType.SoftTarget;
        if (flags["m"]) targetType = TargetType.MouseOverTarget;
        if (flags["f"]) targetType = TargetType.FocusTarget;
        if (flags["p"]) targetType = TargetType.PreviousTarget;

        if (VelaraUtils.TargetManager[targetType] is not null)
            VelaraUtils.TargetManager[targetType] = null;
        else if (flags["v"])
            ChatUtil.ShowPrefixedError(
                ChatColour.CONDITION_FAILED,
                $"Cannot clear {targetType.ToString()}: no {targetType.ToString()} selected",
                ChatColour.RESET);

        // GameObject? target = VelaraUtils.TargetState.Target;
        // if (target is not null && target.IsValid())
        // {
        //     if (flags["v"]) ChatUtil.ShowPrefixedError();
        //     return;
        // }
        //
        // VelaraUtils.TargetState.ClearTarget();
    }
}
