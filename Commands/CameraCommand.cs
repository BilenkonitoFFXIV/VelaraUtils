// using VelaraUtils.Attributes;
// using VelaraUtils.Chat;
// using VelaraUtils.Utils;
//
// namespace VelaraUtils.Commands
// {
// // cameraManager->WorldCamera->Mode
//     public static partial class PluginCommands
//     {
//         [Command("/cameramode")]
//         [Arguments("camera mode")]
//         [Summary("Camera Mode.")]
//         [Aliases("/cmode")]
//         [HelpMessage("")]
//         public static unsafe void CameraModeCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
//         {
//             if (VelaraUtils.Client?.LocalPlayer is null) {
//                 ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
//                 return;
//             }
//
//             if (string.IsNullOrEmpty(argLine))
//             {
//                 ChatUtil.ShowPrefixedMessage(
//                     ChatColour.WHITE,
//                     "Current camera mode is ",
//                     ChatColour.RESET,
//                     ChatColour.GREEN,
//                     VelaraUtils.CameraManager->WorldCamera->Mode,
//                     ChatColour.RESET
//                 );
//                 return;
//             }
//
//             if (!int.TryParse(argLine, out int cameraMode) || cameraMode is < 0 or > 2)
//             {
//                 ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Invalid mode", ChatColour.RESET);
//                 return;
//             }
//
//             VelaraUtils.CameraManager->WorldCamera->Mode = cameraMode;
//         }
//     }
// }
