﻿using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/playsound")]
    [Summary("Plays one of the sixteen <se.##> sound effects")]
    [Aliases("/playsfx")]
    [HelpMessage(
        "This lets you play the <se.##> sound effects in your chat without needing to /echo them.",
        "It just helps keep things a little cleaner."
    )]
    public static void PlayChatSound(string command, string args, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Sfx is null || !VelaraUtils.Sfx.Valid)
        {
            ChatUtil.ShowPrefixedError("Unable to play sounds, the relevant game function couldn't be located");
            return;
        }

        if (!int.TryParse(args, out int idx))
        {
            ChatUtil.ShowPrefixedError("Invalid value, must provide a sound ID from 1-16, inclusive");
            return;
        }

        if (idx is < 1 or > 16)
        {
            ChatUtil.ShowPrefixedError("Invalid sound ID, must be 1-16 inclusive");
            return;
        }

        VelaraUtils.Sfx.Play(SoundsExtensions.FromGameIndex(idx));
    }
}
