using System;

namespace VelaraUtils.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class AliasesAttribute: Attribute {
    public string[] Aliases { get; }

    public AliasesAttribute(params string[] aliases) {
        Aliases = aliases;
    }
}