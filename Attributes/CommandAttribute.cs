using System;
using JetBrains.Annotations;

namespace VelaraUtils.Attributes;

[MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
[AttributeUsage(AttributeTargets.Method)]
internal class CommandAttribute : Attribute
{
    public string Command { get; }

    public CommandAttribute(string command)
    {
        Command = command;
    }
}
