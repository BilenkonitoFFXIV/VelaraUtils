using System;
using System.Collections.Generic;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Internal.Action;
using VelaraUtils.Internal.Command;
using VelaraUtils.Internal.Macro;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

[CommandModule("Macro", "macro")]
public unsafe class MacroModule : ICommandModule
{
    private NativePointer<RaptureHotbarModule> _raptureHotbarModule;
    private ActionManager? _actionManager;
    private MacroManager? _macroManager;

    public bool Load(DalamudPluginInterface pluginInterface)
    {
        _raptureHotbarModule = Framework.Instance()->GetUiModule()->GetRaptureHotbarModule();
        _actionManager = VelaraUtils.ActionManager;
        _macroManager = VelaraUtils.MacroManager;

        PluginLog.LogInformation($"_raptureHotbarModule.IsValid: {_raptureHotbarModule.IsValid.ToString()}");
        PluginLog.LogInformation($"_raptureHotbarModulePtr.Value: {((IntPtr)_raptureHotbarModule.Value).ToString("X")}");
        PluginLog.LogInformation($"_actionManager: {(_actionManager is null ? "null" : _actionManager.ToString())}");
        PluginLog.LogInformation($"_macroManager: {(_macroManager is null ? "null" : _macroManager.ToString())}");

        return _raptureHotbarModule.IsValid && _actionManager is not null && _macroManager is not null;
        // return true;
    }

    public void Unload()
    {
        _macroManager = null;
        _actionManager = null;
        _raptureHotbarModule = null;
    }

    [Command("print")]
    [Summary("Prints macro debug information")]
    [Arguments("macro page (0/1)", "macro index (0-99)")]
    [HelpMessage("")]
    public static void PrintCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        CommandArgumentParser.Parse(argLine, VelaraUtils.VariablesConfiguration.Variables, out string macroPageStr, out string macroIdStr);

        if (!int.TryParse(macroPageStr, out int macroPage) || macroPage is < 0 or > 1)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid macro page: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                macroPageStr,
                ChatColour.RESET);
            return;
        }

        if (!int.TryParse(macroIdStr, out int macroId) || macroId is < 0 or > 99)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid macro index: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                macroIdStr,
                ChatColour.RESET);
            return;
        }

        GameMacro gameMacro = GameMacro.Get((uint)macroPage, (uint)macroId);
        if (!gameMacro.IsValid)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid macro: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                macroPage.ToString(), "/", macroId.ToString(),
                ChatColour.RESET);
            return;
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "Macro ", macroPage.ToString(), "/", macroId.ToString(), ":",
            ChatColour.RESET);
        foreach (IReadOnlyList<object> chatLines in gameMacro.ToChatMessage())
            ChatUtil.ShowMessage(chatLines);
    }

    [Command("exec")]
    [Summary("Executes a macro")]
    [Arguments("macro page (0/1)", "macro index (0-99)")]
    [HelpMessage("")]
    public static void ExecCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        CommandArgumentParser.Parse(argLine, VelaraUtils.VariablesConfiguration.Variables, out string macroPageStr, out string macroIdStr);

        if (!int.TryParse(macroPageStr, out int macroPage) || macroPage is < 0 or > 1)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid macro page: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                macroPageStr,
                ChatColour.RESET);
            return;
        }

        if (!int.TryParse(macroIdStr, out int macroId) || macroId is < 0 or > 99)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid macro index: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                macroIdStr,
                ChatColour.RESET);
            return;
        }

        GameMacro gameMacro = GameMacro.Get((uint)macroPage, (uint)macroId);
        if (!gameMacro.IsValid)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid macro: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                macroPage.ToString(), "/", macroId.ToString(),
                ChatColour.RESET);
            return;
        }

        gameMacro.Execute();
    }

    [Command("seticon")]
    [Summary("Sets the icon of a macro in every hotbar slot it's placed on")]
    [Arguments("macro page (0/1)", "macro index (0-99)", "Icon (icon id)", "IconA (action id)", "IconTypeA (HotbarSlotType string key)")]
    [HelpMessage("")]
    public void SetIconCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        List<string> _ = CommandArgumentParser.Parse(argLine, VelaraUtils.VariablesConfiguration.Variables,
            out string macroPageStr, out string macroIdStr,
            out string iconStr, out string iconAStr, out string iconTypeAStr);

        if (!int.TryParse(macroPageStr, out int macroPage) || macroPage is < 0 or > 1)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid macro page: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                macroPageStr,
                ChatColour.RESET);
            return;
        }

        if (!int.TryParse(macroIdStr, out int macroId) || macroId is < 0 or > 99)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid macro index: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                macroIdStr,
                ChatColour.RESET);
            return;
        }

        if (!int.TryParse(iconStr, out int icon) || icon < 0)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid Icon: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                iconStr,
                ChatColour.RESET);
            return;
        }

        if (!int.TryParse(iconAStr, out int iconA) || iconA < 0)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid IconA: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                iconAStr,
                ChatColour.RESET);
            return;
        }

        if (!Enum.TryParse(iconTypeAStr, true, out HotbarSlotType iconTypeA))
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid IconTypeA: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                iconTypeA.ToString(),
                ChatColour.RESET);
            return;
        }

        // int actionId = null;
        // if (int.TryParse(actionIdStr, out int actionIdInt) && actionIdInt >= 0)
        // actionId = actionIdInt;

        // ChatUtil.ShowPrefixedMessage($"macroPage:{macroPage.ToString()}");
        // ChatUtil.ShowPrefixedMessage($"macroId:{macroId.ToString()}");
        // ChatUtil.ShowPrefixedMessage($"iconId:{iconId.ToString()}");
        // ChatUtil.ShowPrefixedMessage($"actionId:{actionId.ToString()}");

        GameMacro gameMacro = GameMacro.Get((uint)macroPage, (uint)macroId);
        if (!gameMacro.IsValid)
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Invalid macro: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                macroPage.ToString(), "/", macroId.ToString(),
                ChatColour.RESET);
            return;
        }

        RaptureHotbarModule* raptureHotbarModule = _raptureHotbarModule;
        for (int i = 0; i < 18; i++)
        {
            HotBar* hotbar = raptureHotbarModule->HotBar[i];
            if ((IntPtr)hotbar == IntPtr.Zero) continue;

            for (int j = 0; j < (i < 10 ? 12 : 16); j++)
            {
                HotBarSlot* slot = hotbar->Slot[j];
                if ((IntPtr)slot == IntPtr.Zero) continue;

                if (slot->CommandType != HotbarSlotType.Macro) continue;
                if (slot->CommandId < 256 && macroPage == 1) continue;
                if (slot->CommandId > 255 && macroPage == 0) continue;
                if (slot->CommandId % 256 != macroId) continue;

                if (flags["b"])
                {
                    slot->Icon = icon;
                    slot->IconB = (uint)iconA;
                    slot->IconTypeB = iconTypeA;

                    if (flags["l"])
                        slot->LoadIconFromSlotB();

                    if (flags["g"])
                        slot->Icon = slot->GetIconIdForSlot(slot->IconTypeB, slot->IconB);
                }
                else
                {
                    slot->Icon = icon;
                    slot->IconA = (uint)iconA;
                    slot->IconTypeA = iconTypeA;

                    if (flags["l"])
                        slot->LoadIconFromSlotB();

                    if (flags["g"])
                        slot->Icon = slot->GetIconIdForSlot(slot->IconTypeB, slot->IconB);
                }

                if (iconTypeA == HotbarSlotType.PvPCombo)
                {
                    slot->UNK_0xC4 = 0;
                    slot->UNK_0xCA = 0;
                    slot->UNK_0xCB = 1;
                    slot->UNK_0xDE = 3;
                }

                // ((NativePointer<HotBarSlot>)slot).Print();
            }
        }

        // _raptureHotbarModule.ForEach((hotbar, i) =>
        // {
        //     if (!hotbar.IsValid) return;
        //     hotbar.ForEach((slot, j) =>
        //     {
        //         if (!slot.IsValid) return;
        //         if (slot.Value->CommandType != HotbarSlotType.Macro) return;
        //         if (slot.Value->CommandId < 256 && macroPage == 1) return;
        //         if (slot.Value->CommandId > 255 && macroPage == 0) return;
        //         if (slot.Value->CommandId % 256 != macroId) return;
        //
        //         if (actionId is not null)
        //         {
        //             slot.Value->IconA = (uint)actionId.Value;
        //             slot.Value->IconTypeA = HotbarSlotType.Action;
        //
        //             // slot.Value->IconB = (uint)actionId.Value;
        //             // slot.Value->IconTypeB = HotbarSlotType.Action;
        //         }
        //
        //         slot.Value->Icon = iconId;
        //         // slot.Value->UNK_0xCA = 0;
        //         // slot.Value->LoadIconFromSlotB();
        //
        //         _raptureHotbarModule.Print(i, j);
        //     });
        // });

        // List<NativePointer<HotBarSlot>> slots = _raptureHotbarModule.FindSlot(slot =>
        //     slot.Value->CommandType == HotbarSlotType.Macro &&
        //     macroPage == (slot.Value->CommandId > 255 ? 1 : 0) &&
        //     macroId == slot.Value->CommandId % 256).ToList();
        // ChatUtil.ShowMessage($"#slots: {slots.Count}");
        // foreach (HotBarSlot* slot in slots)
        // {
        //     if (actionId is not null)
        //     {
        //         slot->IconA = (uint)actionId.Value;
        //         slot->IconTypeA = HotbarSlotType.Action;
        //
        //         // slot->IconB = (uint)actionId.Value;
        //         // slot->IconTypeB = HotbarSlotType.Action;
        //     }
        //
        //     slot->Icon = iconId;
        //     // slot.Value->UNK_0xCA = 0;
        //     // slot.Value->LoadIconFromSlotB();
        //
        //     ((NativePointer<HotBarSlot>)slot).Print();
        // }
    }
}
