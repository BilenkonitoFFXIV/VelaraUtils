using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/ifjob")]
    [Arguments("condition flags", "jobs to match against", "command to run...?")]
    [Summary("Run a chat command (or directly send a message) only when playing certain classes/jobs")]
    [Aliases("/ifclass", "/whenjob", "/whenclass", "/job", "/class")]
    [HelpMessage(
        "Much like /ifcmd and /ifgp, this command executes a given command when the condition is met.",
        "In this case, the condition is whether or not the current class/job is one of the given set.",
        "Use the three-letter abbreviation, and if you want to check against more than one, separate them with commas but NOT spaces. Can be a variable.",
        "If you pass the -t (TARGET) flag, the match will be done based on the target instead of player.",
        "If you pass the -m (MOUSEOVER) flag, the match will be done based on the mouseover target instead of player.",
        "If you pass the -n (NOT) flag, the match will be inverted."
    )]
    public static void RunIfJobMatches(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        GameObject? target =
            flags["t"] ? VelaraUtils.TargetManager.Target :
            flags["m"] ? VelaraUtils.TargetManager.MouseOverTarget :
            VelaraUtils.Client.LocalPlayer;

        if (target?.ObjectKind is not ObjectKind.Player)
        {
            string? targetName = target?.Name.ToString();
            if (string.IsNullOrWhiteSpace(targetName))
                targetName = "null";

            ChatUtil.ShowPrefixedError(
                ChatColour.WHITE,
                "Invalid target: ",
                ChatColour.RESET,
                targetName == "null" ? ChatColour.RED : ChatColour.CONDITION_FAILED,
                targetName,
                ChatColour.RESET,
                ChatColour.WHITE,
                ".",
                ChatColour.RESET
            );
            return;
        }

        PlayerCharacter pTarget = (PlayerCharacter)target;

        string? currentJobName = pTarget.ClassJob.GameData?.Abbreviation.ToString().ToUpper().Trim();
        if (string.IsNullOrWhiteSpace(currentJobName))
        {
            ChatUtil.ShowPrefixedError("Target has invalid job.");
            return;
        }

        List<string> cmd = CommandArgumentParser.Parse(argLine, out string value);
        if (!string.IsNullOrEmpty(value))
        {
            string[] wantedJobNames = value.ToUpper().Split(',').Select(o => o.Trim()).ToArray();

            bool match = wantedJobNames.Contains(currentJobName) ^ flags["n"];
            if (cmd.Count > 0)
            {
                if (match)
                    ChatUtil.SendChatLineToServer(string.Join(" ", cmd));
                return;
            }

            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "Job: ",
                ChatColour.RESET,
                ChatColour.BLUE,
                currentJobName,
                ChatColour.RESET,
                ChatColour.WHITE,
                ", Value: ",
                ChatColour.RESET,
                match ? ChatColour.TRUE : ChatColour.FALSE,
                value,
                ChatColour.RESET,
                ChatColour.WHITE,
                ".",
                ChatColour.RESET
            );
            return;
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "Job: ",
            ChatColour.RESET,
            ChatColour.BLUE,
            currentJobName,
            ChatColour.RESET,
            ChatColour.WHITE,
            ".",
            ChatColour.RESET
        );
    }
}
