// using System;
// using System.Linq;
// using FFXIVClientStructs.FFXIV.Client.Game;
// using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
// using VelaraUtils.Attributes;
// using VelaraUtils.Chat;
// using VelaraUtils.Utils;
//
// namespace VelaraUtils.Commands
// {
//     public static partial class PluginCommands
//     {
//         [Command("/ifastcard")]
//         [Arguments("condition flags", "card names to match against", "command to run...?")]
//         [Summary("Run a chat command (or directly send a message) only when holding any of the given Astrologian cards")]
//         [Aliases("/whenastcard")]
//         [HelpMessage(
//             "Much like /ifcmd and /ifgp, this command executes a given command when the condition is met.",
//             "In this case, the condition is whether or not the player is holding any of the given Astrologian cards.",
//             "Use the names of the cards separated by commas.",
//             "If you pass the -n (NOT) flag, the match will be inverted."
//         )]
//         public static unsafe void RunIfAstCard(string command, string argLine, FlagMap flags, ref bool showHelp)
//         {
//             if (VelaraUtils.Client?.LocalPlayer is null) {
//                 ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
//                 return;
//             }
//
//             if (VelaraUtils.Client.LocalPlayer.ClassJob.GameData?.Abbreviation.ToString().ToUpper() != "AST")
//             {
//                 ChatUtil.ShowPrefixedError("You're currently not playing as Astrologian.");
//                 return;
//             }
//
//             JobGaugeManager* jobGaugeManager = JobGaugeManager.Instance();
//             AstrologianGauge gauge = jobGaugeManager->Astrologian;
//
//             string args = !string.IsNullOrEmpty(argLine) ? argLine : string.Empty;
//
//             string cardNames = args.Split()[0];
//             AstrologianCard[] cardArray = cardNames.Split(',')
//                 .Select<string, AstrologianCard?>(card => Enum.TryParse(card, true, out AstrologianCard result) ? result : null)
//                 .Where(card => card != null)
//                 .Select(card => card!.Value)
//                 .ToArray();
//
//             string cmd = args[cardNames.Length..].Trim();
//             bool match = cardArray.Contains(gauge.CurrentCard);
//
//             if (match ^ flags["n"] && cmd.Length > 0) {
//                 ChatUtil.SendChatLineToServer(cmd);
//             }
//         }
//     }
// }
