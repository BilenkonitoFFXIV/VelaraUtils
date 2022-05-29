using System;
using JetBrains.Annotations;

namespace VelaraUtils.Attributes;

[UsedImplicitly(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithInheritors)]
[MeansImplicitUse(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithInheritors)]
[AttributeUsage(AttributeTargets.Method)]
internal class NoHelpFlagAttribute : Attribute
{
}
