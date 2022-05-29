using System;
using System.Collections.Generic;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/ifpartysize")]
    [Arguments("condition flags", "value", "command to run...?")]
    [Summary("Run a chat command (or directly send a message) only when party size the given amount")]
    [Aliases("/whenpartysize", "/ifpartycount", "/whenpartycount", "/ifpsize", "/whenpsize", "/ifpcount", "/whenpcount")]
    [HelpMessage(
        "Much like /ifcmd and /ifgp, this command executes a given command when the condition is met.",
        "In this case, the condition is whether or not the size of the player's party the given amount.",
        "Use the party size in integer form [1 - 20]. Can be a variable.",
        "If you pass the -n (NOT) flag, the match will be inverted.",
        "If you pass the -e (EQUAL) flag, the party size must be equal to the given amount.",
        "If you pass the -l (LESS) flag, the party size must be less than the given amount.",
        "If you pass the -g (GREATER) flag, the party size must be greater than the given amount.",
        "If you pass neither the -e (EQUAL), -l (LESS) nor the -g (GREATER) flags, the -e (EQUAL) flag will be used by default.",
        "The -l (LESS) and -g (GREATER) flags can be combined with the -e (EQUAL) flag."
    )]
    public static void RunIfPartySize(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        int partySize = Math.Max(VelaraUtils.PartyList?.Length ?? 0, 1);

        List<string> cmd = CommandArgumentParser.Parse(argLine, out string value);
        if (!string.IsNullOrEmpty(value))
        {
            if (!int.TryParse(value, out int iValue) || iValue is < 1 or > 20)
            {
                ChatUtil.ShowPrefixedError($"Invalid party size: {value}.");
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
                ComparisonOperation.GREATER_OR_EQUAL => partySize >= iValue,
                ComparisonOperation.GREATER => partySize > iValue,
                ComparisonOperation.LESS_OR_EQUAL => partySize <= iValue,
                ComparisonOperation.LESS => partySize < iValue,
                _ => Math.Abs(partySize - iValue) < float.Epsilon
            } ^ flags["n"];

            if (cmd.Count > 0)
            {
                if (match)
                    ChatUtil.SendChatLineToServer(string.Join(" ", cmd));
                return;
            }

            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "Party size: ",
                ChatColour.RESET,
                ChatColour.BLUE,
                partySize,
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
                iValue,
                ChatColour.RESET,
                ChatColour.WHITE,
                ".",
                ChatColour.RESET
            );
            return;
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "Party size: ",
            ChatColour.RESET,
            ChatColour.BLUE,
            partySize,
            ChatColour.RESET,
            ChatColour.WHITE,
            ".",
            ChatColour.RESET
        );
    }

    [Command("/ifbuddy")]
    [Arguments("flags", "command to run...?")]
    [Summary("Run a chat command (or directly send a message) only when having a buddy")]
    [Aliases("/whenbuddy")]
    [HelpMessage(
        "Much like /ifcmd and /ifgp, this command executes a given command when the condition is met.",
        "In this case, the condition is whether or not the size of the player's has a buddy.",
        "If you pass the -p (PET) flag, the buddy must be a job summoned pet.",
        "If you pass the -c (COMPANION) flag, the buddy must be a companion (chocobo).",
        "If you pass the -n (NOT) flag, the match will be inverted."
    )]
    public static void RunIfBuddy(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        bool hasPet = VelaraUtils.BuddyList?.PetBuddyPresent ?? false;
        bool hasCompanion = VelaraUtils.BuddyList?.CompanionBuddyPresent ?? false;

        bool match = hasPet || hasCompanion;
        if (flags["p"])
            match = match && hasPet;
        if (flags["c"])
            match = match && hasCompanion;
        match ^= flags["n"];

        List<string> cmd = CommandArgumentParser.Parse(argLine);
        if (cmd.Count > 0)
        {
            if (match)
                ChatUtil.SendChatLineToServer(string.Join(" ", cmd));
            return;
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "Pet: ",
            ChatColour.RESET,
            hasPet ?
                ChatColour.TRUE :
                ChatColour.FALSE,
            hasPet,
            ChatColour.RESET,
            ChatColour.WHITE,
            ", Companion: ",
            ChatColour.RESET,
            hasCompanion ?
                ChatColour.TRUE :
                ChatColour.FALSE,
            hasCompanion,
            ChatColour.RESET,
            ChatColour.WHITE,
            ".",
            ChatColour.RESET
        );
    }
}
