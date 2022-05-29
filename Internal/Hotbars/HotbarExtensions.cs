using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using JetBrains.Annotations;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Internal.Hotbars;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static unsafe class HotbarExtensions
{
    private static void ForEach(this HotBars source, Action<NativePointer<HotBar>, int> predicate)
    {
        for (int i = 0; i < 18; i++)
        {
            HotBar* hotbar = source[i];
            if ((IntPtr)hotbar == IntPtr.Zero) continue;
            predicate?.Invoke(hotbar, i);
        }
    }

    private static void ForEach(this HotBars source, Action<NativePointer<HotBar>> predicate) =>
        source.ForEach((hotbar, _) => predicate(hotbar));

    public static void ForEach(this NativePointer<RaptureHotbarModule> source, Action<NativePointer<HotBar>, int> predicate) =>
        source.Value->HotBar.ForEach(predicate);

    public static void ForEach(this NativePointer<RaptureHotbarModule> source, Action<NativePointer<HotBar>> predicate) =>
        source.Value->HotBar.ForEach(predicate);

    private static void ForEach(this HotBarSlots source, Action<NativePointer<HotBarSlot>, int> predicate)
    {
        for (int i = 0; i < 16; i++)
        {
            HotBarSlot* slot = source[i];
            if ((IntPtr)slot == IntPtr.Zero) continue;
            predicate?.Invoke(slot, i);
        }
    }

    private static void ForEach(this HotBarSlots source, Action<NativePointer<HotBarSlot>> predicate) =>
        source.ForEach((slot, _) => predicate(slot));

    public static void ForEach(this NativePointer<HotBar> source, Action<NativePointer<HotBarSlot>, int> predicate) =>
        source.Value->Slot.ForEach(predicate);

    public static void ForEach(this NativePointer<HotBar> source, Action<NativePointer<HotBarSlot>> predicate) =>
        source.Value->Slot.ForEach(predicate);

    public static IEnumerable<NativePointer<HotBarSlot>> FindSlot(this NativePointer<RaptureHotbarModule> source, Func<NativePointer<HotBarSlot>, int, int, bool> selector)
    {
        List<NativePointer<HotBarSlot>> results = new();
        source.ForEach((hotbar, i) => hotbar.ForEach((slot, j) =>
        {
            if (selector.Invoke(slot, i, j))
                results.Add(slot);
        }));
        return results;
    }

    public static IEnumerable<NativePointer<HotBarSlot>> FindSlot(this NativePointer<RaptureHotbarModule> source, Func<NativePointer<HotBarSlot>, bool> selector) =>
        source.FindSlot((slot, _, _) => selector(slot));

    public static void Print(this NativePointer<RaptureHotbarModule> source)
    {
        for (int i = 0; i < 16; i++) source.Print(i);
    }

    public static void Print(this NativePointer<RaptureHotbarModule> source, int hotbarIdx)
    {
        if (hotbarIdx is < 0 or > 17) throw new ArgumentOutOfRangeException(nameof(hotbarIdx));

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "Hotbar ", hotbarIdx.ToString(), ":",
            ChatColour.RESET);
        ((NativePointer<HotBar>)source.Value->HotBar[hotbarIdx]).Print();
    }

    public static void Print(this NativePointer<RaptureHotbarModule> source, int? hotbarIdx, int slotIdx)
    {
        if (hotbarIdx is null)
        {
            for (int i = 0; i < 16; i++)
            {
                ChatUtil.ShowPrefixedMessage(
                    ChatColour.WHITE,
                    "Hotbar ", hotbarIdx.ToString(), ":",
                    ChatColour.RESET);
                ((NativePointer<HotBar>)source.Value->HotBar[i]).Print(slotIdx);
            }
        }
        else
        {
            if (hotbarIdx is < 0 or > 17) throw new ArgumentOutOfRangeException(nameof(hotbarIdx));

            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "Hotbar ", hotbarIdx.ToString(), ":",
                ChatColour.RESET);
            ((NativePointer<HotBar>)source.Value->HotBar[hotbarIdx.Value]).Print(slotIdx);
        }
    }

    public static void Print(this NativePointer<HotBar> source)
    {
        for (int i = 0; i < 16; i++) source.Print(i);
    }

    public static void Print(this NativePointer<HotBar> source, int idx)
    {
        if (idx is < 0 or > 15) throw new ArgumentOutOfRangeException(nameof(idx));

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "   Slot ", idx.ToString(), ":",
            ChatColour.RESET);
        ((NativePointer<HotBarSlot>)source.Value->Slot[idx]).Print();
    }

    public static void Print(this NativePointer<HotBarSlot> source)
    {
        HotBarSlot* slot = source;
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - PopUpHelp: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->PopUpHelp.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - CostText: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->CostText->ToString("X"),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - PopUpKeybindHint: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->PopUpKeybindHint->ToString("X"),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - KeybindHint: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->KeybindHint->ToString("X"),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - CommandId: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->CommandId.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - IconA: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->IconA.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - IconB: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->IconB.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - UNK_0xC4: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->UNK_0xC4.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - CommandType: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->CommandType.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - IconTypeA: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->IconTypeA.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - IconTypeB: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->IconTypeB.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - UNK_0xCA: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->UNK_0xCA.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - UNK_0xCB: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->UNK_0xCB.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - Icon: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->Icon.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - UNK_0xD0: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->UNK_0xD0.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - UNK_0xD4: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->UNK_0xD4.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - UNK_0xD8: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->UNK_0xD8.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - UNK_OxDC: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->UNK_OxDC.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - UNK_OxDD: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->UNK_OxDD.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - UNK_0xDE: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->UNK_0xDE.ToString(),
            ChatColour.RESET);
        ChatUtil.ShowMessage(
            ChatColour.WHITE,
            "      - IsEmpty: ",
            ChatColour.RESET,
            ChatColour.INDIGO,
            slot->IsEmpty.ToString(),
            ChatColour.RESET);
    }
}
