using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Game.Text.SeStringHandling;
using VelaraUtils.Internal.Payloads;
using VelaraUtils.Internal.SeStrings;
using VelaraUtils.Utils;

namespace VelaraUtils.Chat;

public static class ChatUtil {
    #region Chatlog functions
    public static void ShowMessage(PayloadCollection payloads)
    {
        if (payloads.Count < 1) return;
        VelaraUtils.Chat?.Print(payloads.ToSeString());
    }

    public static void ShowMessage(params Payload[] payloads)
    {
        if (payloads.Length < 1) return;
        VelaraUtils.Chat?.Print(payloads.ToSeString());
    }

    public static void ShowMessage(IEnumerable<object?> payloads) =>
        ShowMessage(new PayloadCollection(payloads));

    public static void ShowMessage(params object?[] payloads) =>
        ShowMessage(new PayloadCollection(payloads));

    public static void ShowPrefixedMessage(IEnumerable<object?> payloads) =>
        ShowMessage(new PayloadCollection(
            ChatColour.PREFIX,
            '[',
            VelaraUtils.Prefix,
            "] ",
            ChatColour.RESET,
            payloads
        ));

    public static void ShowPrefixedMessage(params object?[] payloads) =>
        ShowPrefixedMessage(payloads.AsEnumerable());

    public static void ShowError(PayloadCollection payloads)
    {
        if (payloads.Count < 1) return;
        VelaraUtils.Chat?.PrintError(payloads.ToSeString());
    }

    public static void ShowError(params Payload[] payloads)
    {
        if (payloads.Length < 1) return;
        VelaraUtils.Chat?.PrintError(payloads.ToSeString());
    }

    public static void ShowError(IEnumerable<object?> payloads) =>
        ShowError(new PayloadCollection(payloads));

    public static void ShowError(params object?[] payloads) =>
        ShowError(new PayloadCollection(payloads));

    public static void ShowPrefixedError(IEnumerable<object?> payloads) =>
        ShowError(new PayloadCollection(
            ChatColour.PREFIX,
            '[',
            VelaraUtils.Prefix,
            "] ",
            ChatColour.RESET,
            payloads
        ));

    public static void ShowPrefixedError(params object?[] payloads) =>
        ShowPrefixedError(payloads.AsEnumerable());
    #endregion

    public static void SendChatLineToServer(string line, bool displayInChatlog, bool dryRun = false)
    {
        if (string.IsNullOrEmpty(line)) return;
        if (displayInChatlog || dryRun)
            ShowPrefixedMessage(ChatColour.DEBUG, line, ChatColour.RESET);
        if (!dryRun)
            VelaraUtils.Common?.Functions.Chat.SendMessage(line);
    }

    public static void SendChatLineToServer(string line) =>
        SendChatLineToServer(line, false);

    public static void SendChatLinesToServer(IEnumerable<string> lines) =>
        lines.Where(o => !string.IsNullOrEmpty(o)).ToList().ForEach(SendChatLineToServer);

    // public static async Task SendChatLinesToServer(float delay, IEnumerable<string> lines) =>
    //     await lines.Where(o => !string.IsNullOrEmpty(o)).ToList().ForEachAsync(delay * 1000f, SendChatLineToServer);

    public static async Task SendChatLinesToServer(float delay, IEnumerable<string> lines)
    {
        int delayMs = (int)MathF.Floor(delay * 1000f);

        using IEnumerator<string> enumerator = lines.GetEnumerator();
        while (enumerator.MoveNext())
        {
            string line = enumerator.Current;
            if (string.IsNullOrEmpty(line)) continue;

            SendChatLineToServer(line);

            if (delayMs < 1) continue;
            await Task.Delay(delayMs);
        }
    }

    public static async Task SendChatLinesToServer(float delay, params string[] lines) =>
        await SendChatLinesToServer(delay, lines.AsEnumerable());

    public static void ShowPrefixedPair(string key, object? value) =>
        ShowPrefixedMessage((
            key,
            value is Enum ?
                Enum.GetName(value.GetType(), value) :
                value
        ).ToPayload());

    public static void ShowPair(string key, object? value) =>
        ShowMessage((
            key,
            value is Enum ?
                Enum.GetName(value.GetType(), value) :
                value
        ).ToPayload());
}
