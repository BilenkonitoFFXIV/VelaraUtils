using System;
using JetBrains.Annotations;

namespace VelaraUtils.Attributes;

[MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
[AttributeUsage(AttributeTargets.Class)]
public class CommandModuleAttribute : Attribute
{
    public string Name { get; }
    public string Prefix { get; }

    public CommandModuleAttribute(string name, string? prefix = null)
    {
        Name = name.Trim();
        Prefix = prefix?.Trim() ?? string.Empty;
    }
}
