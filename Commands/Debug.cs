using System;
using System.IO;
using System.Text;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.GeneratedSheets;
using VelaraUtils.Agents;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Internal.Command;
using VelaraUtils.Internal.Macro;
using VelaraUtils.Utils;
using static System.UInt32;
using GameObjectStruct = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;
using FrameworkStruct = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace VelaraUtils.Commands;

[CommandModule("Debug")]
public class DebugModule : ICommandModule
{
    [Command("/vudebug1")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static unsafe void PluginDebugCommand1(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        var args = !string.IsNullOrEmpty(argLine) ?
            argLine :
            string.Empty;
        var argsArr = args.Split();

        if (argsArr.Length < 2)
        {
            ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Too few arguments", ChatColour.RESET);
            return;
        }

        var actionTypeStr = argsArr[0];
        if (!Enum.TryParse(actionTypeStr, true, out ActionType actionType))
        {
            ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Invalid action type", ChatColour.RESET);
            return;
        }

        var actionIdStr = argsArr[1];
        if (!TryParse(actionIdStr, out var actionId) || (actionId = VelaraUtils.GetActionId(actionType, actionId)) == 0)
        {
            ChatUtil.ShowPrefixedMessage(ChatColour.CONDITION_FAILED, "Invalid action", ChatColour.RESET);
            return;
        }

        var actionManager = ActionManager.Instance();
        var player = (GameObjectStruct*)(void*)VelaraUtils.Client.LocalPlayer.Address;

        ChatUtil.ShowPrefixedPair("Range", ActionManager.GetActionRange(actionId));
        ChatUtil.ShowPrefixedPair("In Range", ActionManager.GetActionInRangeOrLoS(actionId, player, player));
        ChatUtil.ShowPrefixedPair("Status", actionManager->GetActionStatus(actionType, actionId));
        ChatUtil.ShowPrefixedPair("Cost", ActionManager.GetActionCost(actionType, actionId, 1, 1, 1, 1));
        ChatUtil.ShowPrefixedPair("Recast Time", actionManager->GetRecastTime(actionType, actionId));
        ChatUtil.ShowPrefixedPair("Recast Time Elapsed", actionManager->GetRecastTimeElapsed(actionType, actionId));
        ChatUtil.ShowPrefixedPair("Recast Time Active", actionManager->IsRecastTimerActive(actionType, actionId));
        ChatUtil.ShowPrefixedPair("Adjusted ActionId", actionManager->GetAdjustedActionId(actionId));
        ChatUtil.ShowPrefixedPair("Adjusted Cast Time", ActionManager.GetAdjustedCastTime(actionType, actionId));
        ChatUtil.ShowPrefixedPair("Adjusted Recast Time", ActionManager.GetAdjustedRecastTime(actionType, actionId));
    }

    [Command("/vudebug2")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static unsafe void PluginDebugCommand2(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        var jobGaugeManager = JobGaugeManager.Instance();

        switch (VelaraUtils.Client.LocalPlayer.ClassJob.GameData?.Abbreviation.ToString().ToUpper())
        {
            case "AST":
            {
                var gauge = jobGaugeManager->Astrologian;

                ChatUtil.ShowPrefixedPair("Timer", gauge.Timer);
                ChatUtil.ShowPrefixedPair("Card", gauge.CurrentCard);
                ChatUtil.ShowPrefixedPair("Seal 0", gauge.CurrentSeals[0]);
                ChatUtil.ShowPrefixedPair("Seal 1", gauge.CurrentSeals[1]);
                ChatUtil.ShowPrefixedPair("Seal 2", gauge.CurrentSeals[2]);

                break;
            }
            case "BRD":
            {
                var gauge = jobGaugeManager->Bard;

                ChatUtil.ShowPrefixedPair("Song Timer", gauge.SongTimer);
                ChatUtil.ShowPrefixedPair("Repertoire", gauge.Repertoire);
                ChatUtil.ShowPrefixedPair("Soul Voice", gauge.SoulVoice);
                ChatUtil.ShowPrefixedPair("Song Flags", gauge.SongFlags);

                break;
            }
            case "NIN":
            {
                var gauge = jobGaugeManager->Ninja;

                ChatUtil.ShowPrefixedPair("Huton Timer", gauge.HutonTimer);
                ChatUtil.ShowPrefixedPair("Ninki", gauge.Ninki);
                ChatUtil.ShowPrefixedPair("Huton Manual Casts", gauge.HutonManualCasts);

                break;
            }
            case "DNC":
            {
                var gauge = jobGaugeManager->Dancer;

                ChatUtil.ShowPrefixedPair("Feathers", gauge.Feathers);
                ChatUtil.ShowPrefixedPair("Esprit", gauge.Esprit);
                ChatUtil.ShowPrefixedPair(
                    "Dance Step 0",
                    gauge.DanceSteps[0] >= 4 ?
                        DanceStep.Finish :
                        (DanceStep)gauge.DanceSteps[0]);
                ChatUtil.ShowPrefixedPair(
                    "Dance Step 1",
                    gauge.DanceSteps[1] >= 4 ?
                        DanceStep.Finish :
                        (DanceStep)gauge.DanceSteps[1]);
                ChatUtil.ShowPrefixedPair(
                    "Dance Step 2",
                    gauge.DanceSteps[2] >= 4 ?
                        DanceStep.Finish :
                        (DanceStep)gauge.DanceSteps[2]);
                ChatUtil.ShowPrefixedPair(
                    "Dance Step 3",
                    gauge.DanceSteps[3] >= 4 ?
                        DanceStep.Finish :
                        (DanceStep)gauge.DanceSteps[3]);
                ChatUtil.ShowPrefixedPair("Step Index", gauge.StepIndex);
                ChatUtil.ShowPrefixedPair("CurrentStep", gauge.CurrentStep);

                break;
            }
            case "GNB":
            {
                var gauge = jobGaugeManager->Gunbreaker;

                ChatUtil.ShowPrefixedPair("Ammo", gauge.Ammo);
                ChatUtil.ShowPrefixedPair("Max Timer Duration", gauge.MaxTimerDuration);
                ChatUtil.ShowPrefixedPair("Ammo Combo Step", gauge.AmmoComboStep);

                break;
            }
        }
    }

    [Command("/vudebug3")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static unsafe void PluginDebugCommand3(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        // GameObjectStruct* playerPtr = (GameObjectStruct*)(void*)VelaraUtils.Client.LocalPlayer.Address;
        // LuaActor luaActor = *playerPtr->LuaActor;

        // ChatUtil.PrintDebug("Lua String", luaActor.LuaString.ToString());

        try
        {
            var framework = FrameworkStruct.Instance();

            var script = File.ReadAllLines(@"D:\Self\repositories\FFXIV\tmp\luatest2.lua", Encoding.UTF8);
            string[] lines = framework->LuaState.DoString(string.Join('\n', script), "luatest2.lua");
            File.WriteAllLines(@"D:\Self\repositories\FFXIV\tmp\luatest2.txt", lines);
            ChatUtil.ShowPrefixedPair("DONE", true);

            // string[] script = File.ReadAllLines(@"D:\Self\repositories\FFXIV\tmp\luatest1.lua", Encoding.UTF8);
            // string[] lines = luaActor.LuaState->DoString(string.Join('\n', script), "luatest1.lua");
            // for (var i = 0; i < lines.Length; i++)
            // {
            //     ChatUtil.PrintDebug($"Lua DoString Line {i.ToString()}", lines[i]);
            // }

            // File.WriteAllLines(@"D:\Self\repositories\FFXIV\tmp\luatest1.txt", lines);
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    // private unsafe delegate AtkStage* GetAtkStageSingleton();
    // private static GetAtkStageSingleton? _getAtkStageSingleton;

    // [StaticAddress("48 8D 05 ?? ?? ?? ?? 48 89 03 48 8D 4B 40")]
    // public static IntPtr MetronomeAgent { get; private set; }

    [Command("/vudebug4")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static void PluginDebugCommand4(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            // _getAtkStageSingleton ??=
            //     Marshal.GetDelegateForFunctionPointer<GetAtkStageSingleton>(VelaraUtils.Scanner!.ScanText("E8 ?? ?? ?? ?? 41 B8 01 00 00 00 48 8D 15 ?? ?? ?? ?? 48 8B 48 20 E8 ?? ?? ?? ?? 48 8B CF"));
            //
            // AtkUnitList* atkUnitListPtr1 = &_getAtkStageSingleton()->RaptureAtkUnitManager->AtkUnitManager.AllLoadedUnitsList;

            // string[] args = (string.IsNullOrEmpty(argLine) ? "" : argLine).Split(' ');
            // string addonName = args.Length > 0 ? args[0] : "";
            // if (!int.TryParse(args.Length > 1 ? args[1] : "", out int addonIndex))
            // {
            //     addonIndex = 1;
            // }
            //
            // IntPtr pAddon = VelaraUtils.GameGui!.GetAddonByName(addonName, addonIndex);
            // ChatUtil.PrintDebug("pAddon", pAddon.ToString());
            //
            // if (pAddon != IntPtr.Zero)
            // {
            //
            // }

            var agentMetronome = new AgentMetronome(AgentManager.Instance.FindAgentInterfaceByVtable(Offsets.MetronomeAgent));
            ChatUtil.ShowPrefixedPair("agentMetronome.EnsembleModeRunning", agentMetronome.EnsembleModeRunning);


            // https://github.com/MgAl2O4/HarpHeroDalamud/blob/68accf8250c337f7f57d920db0d2c9f991b3c1eb/dalamud/plugin/UnsafeMetronomeLink.cs
            // var statePtr = (UIReaderBardMetronome.AgentData*)uiReader.AgentPtr;
            // if (statePtr != null)
            // {
            //     newIsPlaying = statePtr->IsPlaying != 0;
            //
            //     var uiModulePtr = (gameGui != null) ? gameGui.GetUIModule() : IntPtr.Zero;
            //     if (uiModulePtr != IntPtr.Zero)
            //     {
            //         var getMetronomeManagerPtr = new IntPtr(((UIModule*)uiModulePtr)->vfunc[26]);
            //         var getMetronomeManager = Marshal.GetDelegateForFunctionPointer<GetMetronomeManagerDelegate>(getMetronomeManagerPtr);
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/playlib1")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static void PlayLibDebugCommand1(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            PlayLib.BeginReadyCheck();
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/playlib2")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static void PlayLibDebugCommand2(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            PlayLib.ConfirmBeginReadyCheck();
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/playlib3")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static void PlayLibDebugCommand3(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            PlayLib.BeginReadyCheck();
            PlayLib.ConfirmBeginReadyCheck();
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/playlib4")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static void PlayLibDebugCommand4(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            PlayLib.ConfirmReceiveReadyCheck();
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/playlib5")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static void PlayLibDebugCommand5(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            PlayLib.CancelReadyCheck();
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/testinv1")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static unsafe void TestInvCommand1(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            AgentInventoryContext.Instance()->OpenForItemSlot(InventoryType.ArmoryMainHand, 1, /* Armoury Chest: 119, Inventory: 17 */ 119);

            var inventoryManager = InventoryManager.Instance();
            var container = inventoryManager->GetInventoryContainer(InventoryType.ArmoryMainHand);
            if (container->Loaded > 0x00)
            {
                var once = false;
                for (var i = 0; i < container->Size - 1; i++)
                {
                    var item = container->GetInventorySlot(i);
                    ChatUtil.ShowMessage(
                        ChatColour.PURPLE,
                        "[ArmoryMainHand]",
                        ChatColour.RESET,
                        ChatColour.WHITE,
                        "[",
                        item->Slot.ToString(),
                        "]: ",
                        ChatColour.RESET,
                        item->ItemID > 0 ?
                            ChatColour.CONDITION_PASSED :
                            ChatColour.CONDITION_FAILED,
                        item->ItemID.ToString(),
                        ChatColour.RESET);

                    if (item->ItemID == 13604 && !once)
                        // inventoryManager->MoveItemSlot(InventoryType.ArmoryMainHand, (uint)item->Slot, InventoryType.EquippedItems, 0);
                        once = true;
                }
            }
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/testinv2")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static unsafe void TestInvCommand2(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            AgentInventoryContext.Instance()->OpenForItemSlot(InventoryType.ArmoryMainHand, 1, /* Armoury Chest: 119, Inventory: 17 */ 119);

            var inventoryManager = InventoryManager.Instance();
            var container = inventoryManager->GetInventoryContainer(InventoryType.EquippedItems);
            if (container->Loaded > 0x00)
                // ExcelSheet<Item>? itemRow = VelaraUtils.DataManager?.GetExcelSheet<Item>();
                for (var i = 0; i < container->Size - 1; i++)
                {
                    var item = container->GetInventorySlot(i);
                    ChatUtil.ShowMessage(
                        ChatColour.PURPLE,
                        "[EquippedItems]",
                        ChatColour.RESET,
                        ChatColour.WHITE,
                        "[",
                        item->Slot.ToString(),
                        "]: ",
                        ChatColour.RESET,
                        item->ItemID > 0 ?
                            ChatColour.CONDITION_PASSED :
                            ChatColour.CONDITION_FAILED,
                        item->ItemID.ToString(),
                        ChatColour.RESET
                        // item->ItemID > 0 && itemRow is not null ?
                        //     new object[]
                        //     {
                        //         ChatColour.TEAL,
                        //         itemRow.GetRow(item->ItemID)?.EquipSlotCategory.Value,
                        //         ChatColour.RESET
                        //     } : ChatColour.RESET
                    );
                }
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/testinv3")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static void TestInvCommand3(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            var item = VelaraUtils.DataManager?.GetExcelSheet<Item>()?.GetRow(13604);
            var slotCategory = item?.EquipSlotCategory?.Value;

            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "[13604]: ",
                ChatColour.RESET,
                item is not null ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                item?.Name.ToString() ?? "null",
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- Body: ",
                ChatColour.RESET,
                slotCategory?.Body > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.Body.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- Ears: ",
                ChatColour.RESET,
                slotCategory?.Ears > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.Ears.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- Feet: ",
                ChatColour.RESET,
                slotCategory?.Feet > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.Feet.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- Gloves: ",
                ChatColour.RESET,
                slotCategory?.Gloves > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.Gloves.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- Head: ",
                ChatColour.RESET,
                slotCategory?.Head > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.Head.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- Legs: ",
                ChatColour.RESET,
                slotCategory?.Legs > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.Legs.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- Neck: ",
                ChatColour.RESET,
                slotCategory?.Neck > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.Neck.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- Waist: ",
                ChatColour.RESET,
                slotCategory?.Waist > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.Waist.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- Wrists: ",
                ChatColour.RESET,
                slotCategory?.Wrists > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.Wrists.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- FingerL: ",
                ChatColour.RESET,
                slotCategory?.FingerL > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.FingerL.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- FingerR: ",
                ChatColour.RESET,
                slotCategory?.FingerR > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.FingerR.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- MainHand: ",
                ChatColour.RESET,
                slotCategory?.MainHand > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.MainHand.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- OffHand: ",
                ChatColour.RESET,
                slotCategory?.OffHand > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.OffHand.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "- SubRowId: ",
                ChatColour.RESET,
                slotCategory?.SubRowId > 0 ?
                    ChatColour.CONDITION_PASSED :
                    ChatColour.CONDITION_FAILED,
                slotCategory?.SubRowId.ToString(),
                ChatColour.RESET);
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/testtarget1")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static void TestTargetCommand1(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            VelaraUtils.TargetManager.ClearTarget();
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/testmacro1")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static void TestMacroCommand1(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            var argArr = argLine.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (argArr.Length < 1 || !TryParse(argArr[0], out var macroPage) || macroPage > 1) macroPage = 0;
            if (argArr.Length < 2 || !TryParse(argArr[1], out var macroId) || macroId > 99) macroId = 99;
            var gameMacro = GameMacro.Get(macroPage, macroId);

            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "Macro ",
                macroPage,
                "/",
                macroId,
                ":",
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "   - IconId: ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                gameMacro.IconId.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "   - Key: ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                gameMacro.Key.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "   - Name: ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                gameMacro.Name,
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "   - LineCount: ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                gameMacro.LineCount,
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "   - Lines:",
                ChatColour.RESET);
            for (var i = 0; i < Math.Min(gameMacro.LineCount, 15); i++)
                ChatUtil.ShowMessage(
                    ChatColour.WHITE,
                    "       - ",
                    i.ToString(),
                    ": ",
                    ChatColour.RESET,
                    ChatColour.INDIGO,
                    gameMacro[i],
                    ChatColour.RESET);
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/testmacro2")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static void TestMacroCommand2(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            var argArr = argLine.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (argArr.Length < 1 || !TryParse(argArr[0], out var macroPage) || macroPage > 1) macroPage = 0;
            if (argArr.Length < 2 || !TryParse(argArr[1], out var macroId) || macroId > 99) macroId = 99;
            var gameMacro = GameMacro.Get(macroPage, macroId);

            gameMacro.Execute();
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/testmacro3")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static unsafe void TestMacroCommand3(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            var argArr = argLine.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (argArr.Length < 1 || !TryParse(argArr[0], out var macroPage) || macroPage > 1) macroPage = 0;
            if (argArr.Length < 2 || !TryParse(argArr[1], out var macroId) || macroId > 99) macroId = 99;
            // string actionName = argArr.Length < 3 ? "Gnashing Fang" : string.Join(' ', argArr.Skip(2));
            //
            // GameMacro gameMacro = GameMacro.Get(macroPage, macroId);
            // gameMacro[0] = $"/micon \"{actionName}\" pvpaction";

            var raptureHotbarModule = FrameworkStruct.Instance()->GetUiModule()->GetRaptureHotbarModule();
            for (var i = 0; i < 18; i++)
            {
                var hotbar = raptureHotbarModule->HotBar[i];
                if ((IntPtr)hotbar == IntPtr.Zero) continue;

                for (var j = 0;
                     j <
                     (i < 10 ?
                         12 :
                         16);
                     j++)
                {
                    var slot = hotbar->Slot[j];
                    if ((IntPtr)slot == IntPtr.Zero) continue;

                    if (slot->CommandType != HotbarSlotType.Macro) continue;
                    if (slot->CommandId < 256 && macroPage == 1) continue;
                    if (slot->CommandId > 255 && macroPage == 0) continue;
                    if (slot->CommandId % 256 != macroId) continue;

                    if (flags["a"])
                    {
                        slot->IconA = 29102;
                        slot->IconTypeA = HotbarSlotType.Action;
                    }

                    if (flags["b"])
                    {
                        slot->IconB = 29102;
                        slot->IconTypeB = HotbarSlotType.Action;
                    }

                    if (flags["a"] || flags["b"])
                        slot->Icon = 9364;

                    if (flags["c"])
                        slot->UNK_0xCA = 0;

                    if (flags["l"])
                        slot->LoadIconFromSlotB();

                    ChatUtil.ShowPrefixedMessage(
                        ChatColour.WHITE,
                        "Hotbar ",
                        i.ToString(),
                        " Slot ",
                        j.ToString(),
                        ":",
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - PopUpHelp: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->PopUpHelp.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - CostText: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->CostText->ToString("X"),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - PopUpKeybindHint: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->PopUpKeybindHint->ToString("X"),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - KeybindHint: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->KeybindHint->ToString("X"),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - CommandId: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->CommandId.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - IconA: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->IconA.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - IconB: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->IconB.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - UNK_0xC4: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->UNK_0xC4.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - CommandType: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->CommandType.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - IconTypeA: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->IconTypeA.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - IconTypeB: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->IconTypeB.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - UNK_0xCA: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->UNK_0xCA.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - UNK_0xCB: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->UNK_0xCB.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - Icon: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->Icon.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - UNK_0xD0: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->UNK_0xD0.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - UNK_0xD4: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->UNK_0xD4.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - UNK_0xD8: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->UNK_0xD8.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - UNK_OxDC: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->UNK_OxDC.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - UNK_OxDD: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->UNK_OxDD.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - UNK_0xDE: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->UNK_0xDE.ToString(),
                        ChatColour.RESET);
                    ChatUtil.ShowMessage(
                        ChatColour.WHITE,
                        "   - IsEmpty: ",
                        ChatColour.RESET,
                        ChatColour.INDIGO,
                        slot->IsEmpty.ToString(),
                        ChatColour.RESET);
                }
            }
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }

    [Command("/testmacro4")]
    [HelpMessage("Specifically for dev use")]
    [DoNotShowInHelp]
    [HideInCommandListing]
    public static void TestMacroCommand4(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        try
        {
            using var runtimeMacro = new RuntimeMacro(
                new[]
                {
                    "/micon \"Gnashing Fang\" pvpaction",
                    "/pvpaction \"Gnashing Fang\"",
                    "/echo YEET"
                },
                "Test1");

            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "Runtime Macro:",
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "   - IconId: ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                runtimeMacro.IconId.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "   - Key: ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                runtimeMacro.Key.ToString(),
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "   - Name: ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                runtimeMacro.Name,
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "   - LineCount: ",
                ChatColour.RESET,
                ChatColour.INDIGO,
                runtimeMacro.LineCount,
                ChatColour.RESET);
            ChatUtil.ShowMessage(
                ChatColour.WHITE,
                "   - Lines:",
                ChatColour.RESET);
            for (var i = 0; i < Math.Min(runtimeMacro.LineCount, 15); i++)
                ChatUtil.ShowMessage(
                    ChatColour.WHITE,
                    "       - ",
                    i.ToString(),
                    ": ",
                    ChatColour.RESET,
                    ChatColour.INDIGO,
                    runtimeMacro[i],
                    ChatColour.RESET);

            if (flags["e"])
                runtimeMacro.Execute();
        }
        catch (Exception e)
        {
            ChatUtil.ShowPrefixedError(e.ToString());
        }
    }
}
