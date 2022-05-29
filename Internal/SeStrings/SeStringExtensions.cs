using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text.SeStringHandling;
using VelaraUtils.Internal.Payloads;

namespace VelaraUtils.Internal.SeStrings;

public static class SeStringExtensions
{
    public static SeString ToSeString(this IEnumerable<Payload> values) => values.ToArray().ToSeString();
    public static SeString ToSeString(this Payload[] values) => new(values);
    public static SeString ToSeString(this IEnumerable<object?> values) => new(new PayloadCollection(values));
}
