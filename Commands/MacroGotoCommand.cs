// using VelaraUtils.Attributes;
// using VelaraUtils.Chat;
// using VelaraUtils.Utils;
// using FrameworkStruct = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;
// using RaptureMacroModuleStruct = FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureMacroModule;
// using MacroStruct = FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureMacroModule.Macro;
// using MacroPageStruct = FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureMacroModule.MacroPage;
// using RaptureShellModuleStruct = FFXIVClientStructs.FFXIV.Client.UI.Shell.RaptureShellModule;
// using UIModuleStruct = FFXIVClientStructs.FFXIV.Client.UI.UIModule;
//
// namespace VelaraUtils.Commands;
//
// public partial class UtilsModule
// {
//     [Command("/macrogoto")]
//     [Arguments("macro page", "macro id", "label name")]
//     [Summary("Macro goto.")]
//     [Aliases("/mgoto")]
//     [HelpMessage("")]
//     public static void MacroGotoCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
//     {
//         if (VelaraUtils.Client?.LocalPlayer is null)
//         {
//             ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
//             return;
//         }
//
//         string args = !string.IsNullOrEmpty(argLine) ? argLine : string.Empty;
//         string[] argsArr = args.Split(' ');
//
//         if (argsArr.Length < 4)
//         {
//             ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Too few arguments", ChatColour.RESET);
//         }
//
//         // FrameworkStruct* framework = FrameworkStruct.Instance();
//         // UIModuleStruct* uiModule = framework->GetUiModule();
//         // RaptureMacroModuleStruct* raptureMacroModule = uiModule->GetRaptureMacroModule();
//         // RaptureShellModuleStruct* raptureShellModule = uiModule->GetRaptureShellModule();
//
//         // string macroPageName = argsArr[0].ToLowerInvariant();
//         // MacroPageStruct? macroPage = macroPageName switch
//         // {
//         //     "individual" => raptureMacroModule->Individual,
//         //     "shared" => raptureMacroModule->Shared,
//         //     _ => null
//         // };
//         // if (macroPage is null)
//         // {
//         //     ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Invalid macro type, must be 'individual' or 'shared'", ChatColour.RESET);
//         //     return;
//         // }
//         //
//         // if (!int.TryParse(argsArr[1], out int macroId))
//         // {
//         //     ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Invalid macro id", ChatColour.RESET);
//         //     return;
//         // }
//         //
//         // MacroStruct* macro = macroPage.Value[macroId];
//         // if (macro is null)
//         // {
//         //     ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Invalid macro id", ChatColour.RESET);
//         //     return;
//         // }
//         //
//         // string macroLabel = argsArr[2];
//         // if (string.IsNullOrEmpty(macroLabel))
//         // {
//         //     ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Invalid macro label", ChatColour.RESET);
//         //     return;
//         // }
//         //
//         // Utf8String* line;
//         // int lineIdx = 0;
//         // do
//         // {
//         //     line = macro->Line[lineIdx];
//         //     if (Regex.IsMatch(line->ToString(), $"^\\/(?:macrolabel|mlabel)\\s[\"]?{macroLabel}[\"]?"))
//         //         break;
//         //     lineIdx++;
//         // } while (line is not null);
//         //
//         // if (line is null)
//         // {
//         //     ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Macro label not found", ChatColour.RESET);
//         //     return;
//         // }
//         //
//         // // raptureShellModule->MacroCurrentLine = lineIdx;
//         // ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, $"MacroGotoTest1: {raptureShellModule->MacroCurrentLine.ToString()}", ChatColour.RESET);
//     }
// }
