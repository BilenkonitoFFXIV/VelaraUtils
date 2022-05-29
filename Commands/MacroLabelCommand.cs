// using VelaraUtils.Attributes;
// using VelaraUtils.Chat;
// using VelaraUtils.Utils;
//
// namespace VelaraUtils.Commands;
//
// public partial class UtilsModule
// {
//     [Command("/macrolabel")]
//     [Arguments("label name")]
//     [Summary("Macro label.")]
//     [Aliases("/mlabel")]
//     [HelpMessage("")]
//     public static void MacroLabelCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
//     {
//         if (VelaraUtils.Client?.LocalPlayer is null)
//         {
//             ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
//             return;
//         }
//
//         string args = !string.IsNullOrEmpty(argLine) ? argLine : string.Empty;
//         if (args.Length < 2)
//         {
//             ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Too few arguments", ChatColour.RESET);
//         }
//     }
// }
