using System.Linq;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Internal;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/ifzone")]
    [Arguments("condition flags", "zone ids to match against", "command to run...?")]
    [Summary("Run a chat command (or directly send a message) only when in any of the given zone IDs")]
    [Aliases("/whenzone")]
    [HelpMessage(
        "Much like /ifcmd and /ifgp, this command executes a given command when the condition is met.",
        "In this case, the condition is whether or not the player is in any of the given zone IDs.",
        "Use the id of the zones separated by commas.",
        "If you pass the -n (NOT) flag, the match will be inverted."
    )]
    public static void RunIfZone(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        if (VelaraUtils.Client.TerritoryType == 0)
        {
            ChatUtil.ShowPrefixedError("Unable to get territory info. Please switch zone to initialize plugin.");
            return;
        }

        string args = !string.IsNullOrEmpty(argLine) ? argLine : string.Empty;
        string[] argsArr = args.Split();
        if (argsArr.Length < 1)
        {
            ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Too few arguments", ChatColour.RESET);
            return;
        }

        ulong[] zoneIdArray = argsArr[0].Split(',')
            .Select<string, ulong?>(id => ulong.TryParse(id, out ulong result) ? result : null)
            .Where(id => id != null)
            .Select(id => id!.Value)
            .ToArray();

        TerritoryManager.TerritoryDetail? territoryDetail = VelaraUtils.TerritoryManager?.GetByTerritoryType(VelaraUtils.Client.TerritoryType);

        string cmd = string.Join(' ', argsArr.Skip(1));
        bool match = (territoryDetail != null && zoneIdArray.Contains(territoryDetail.TerritoryType)) ^ flags["n"];

        if (cmd.Length > 0)
        {
            if (match) ChatUtil.SendChatLineToServer(cmd);
            return;
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "You are currently in zone ",
            ChatColour.RESET,
            match ? ChatColour.GREEN : ChatColour.RED,
            territoryDetail?.TerritoryType.ToString() ?? "Unknown",
            ChatColour.RESET,
            ChatColour.WHITE,
            $" (Name: {territoryDetail?.Name ?? "null"},",
            $" MapId: {territoryDetail?.MapId.ToString() ?? "null"},",
            $" SizeFactor: {territoryDetail?.SizeFactor.ToString() ?? "null"},",
            ChatColour.RESET
        );
    }
}
