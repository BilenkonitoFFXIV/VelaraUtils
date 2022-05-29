using System;
using System.Collections.Generic;
using VelaraUtils.Chat;

namespace VelaraUtils.Internal.Payloads;

internal static class PayloadExtensions
{
    public static GenericPayload ToPayload(this object? obj) => new(obj);
    public static PayloadCollection ToPayload(this IEnumerable<object?> obj) => new(obj);
    public static PayloadCollection ToPayload(this ValueTuple<string, object?> obj) => new(
        ChatColour.WHITE,
        obj.Item1,
        ": ",
        ChatColour.RESET,
        obj.Item2 switch
        {
            null => ChatColour.ORANGE,
            false or 0 or "false" or "0" or "" => ChatColour.FALSE,
            _ => ChatColour.TRUE
        },
        obj.Item2?.ToString(),
        ChatColour.RESET,
        ChatColour.WHITE,
        ".",
        ChatColour.RESET
    );
}
