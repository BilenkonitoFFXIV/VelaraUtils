using System;
using System.Collections.Generic;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Internal.Command;
using VelaraUtils.Internal.Hotbars;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

[CommandModule("Hotbar", "hotbar")]
public unsafe class HotbarModule : ICommandModule
{
    private NativePointer<RaptureHotbarModule> _raptureHotbarModule;

    public bool Load(DalamudPluginInterface pluginInterface)
    {
        _raptureHotbarModule = Framework.Instance()->GetUiModule()->GetRaptureHotbarModule();
        return _raptureHotbarModule.IsValid;

        // return true;
    }

    public void Unload()
    {
        _raptureHotbarModule = null;
    }

    [Command("print")]
    [Summary("Prints hotbar debug information")]
    [Arguments("hotbar index?", "slot index?")]
    [HelpMessage("")]
    public void PrintCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        List<string> _ = CommandArgumentParser.Parse(argLine, VelaraUtils.VariablesConfiguration.Variables, out string hotbarStr, out string slotStr);

        if (!int.TryParse(hotbarStr, out int hotbarIdx))
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid hotbar index: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                hotbarStr,
                ChatColour.RESET);
            return;
        }

        if (!int.TryParse(slotStr, out int slotIdx))
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid hotbar index: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                slotStr,
                ChatColour.RESET);
            return;
        }

        RaptureHotbarModule* raptureHotbarModule = _raptureHotbarModule;

        HotBar* hotbar = raptureHotbarModule->HotBar[hotbarIdx];
        if ((IntPtr)hotbar == IntPtr.Zero)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid hotbar ", hotbarIdx.ToString(), ": ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                "0x0",
                ChatColour.RESET);
            return;
        }

        HotBarSlot* slot = hotbar->Slot[slotIdx];
        if ((IntPtr)slot == IntPtr.Zero)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid hotbar ", hotbarIdx.ToString(), " slot ", slotIdx.ToString(), ": ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                "0x0",
                ChatColour.RESET);
            return;
        }

        ((NativePointer<HotBarSlot>)slot).Print();

        // int hotbarIdx = 0;
        // bool allHotbars = string.IsNullOrWhiteSpace(hotbarStr) && string.IsNullOrWhiteSpace(slotStr) || hotbarStr.Equals("*", StringComparison.InvariantCultureIgnoreCase);
        // if (!allHotbars && (!int.TryParse(hotbarStr, out hotbarIdx) || hotbarIdx is < 0 or > 17))
        // {
        //     ChatUtil.ShowPrefixedError(
        //         ChatColour.ERROR,
        //         "Invalid hotbar id: ",
        //         ChatColour.RESET,
        //         ChatColour.CONDITION_FAILED,
        //         hotbarStr,
        //         ChatColour.RESET);
        //     return;
        // }
        //
        // int slotIdx = 0;
        // bool allSlots = string.IsNullOrWhiteSpace(slotStr) && string.IsNullOrWhiteSpace(hotbarStr) || slotStr.Equals("*", StringComparison.InvariantCultureIgnoreCase);
        // if (!allSlots && (!int.TryParse(slotStr, out slotIdx) || slotIdx is < 0 or > 15))
        // {
        //     ChatUtil.ShowPrefixedError(
        //         ChatColour.ERROR,
        //         "Invalid slot id: ",
        //         ChatColour.RESET,
        //         ChatColour.CONDITION_FAILED,
        //         slotStr,
        //         ChatColour.RESET);
        //     return;
        // }
        // if (allHotbars)
        //     if (allSlots)
        //         _raptureHotbarModule.Print();
        //     else
        //         _raptureHotbarModule.Print(null, slotIdx);
        // else if (allSlots)
        //     _raptureHotbarModule.Print(hotbarIdx);
        // else
        //     _raptureHotbarModule.Print(hotbarIdx, slotIdx);
    }
}
