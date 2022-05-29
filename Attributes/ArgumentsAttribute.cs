using System;
using System.Linq;

namespace VelaraUtils.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class ArgumentsAttribute: Attribute {
    public string ArgumentDescription => string.Join(" ", Arguments.Select(a => a.EndsWith("?") ? $"[{a.TrimEnd('?')}]" : $"<{a}>"));
    public string[] Arguments { get; }
    public int RequiredArguments => Arguments.Where(a => !a.EndsWith("?")).Count();
    public int MaxArguments => Arguments.Length;

    public ArgumentsAttribute(params string[] args) {
        Arguments = args;
    }
}