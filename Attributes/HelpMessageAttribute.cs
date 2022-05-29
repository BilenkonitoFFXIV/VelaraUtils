using System;

namespace VelaraUtils.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class HelpMessageAttribute: Attribute {
    public string HelpMessage { get; }

    public HelpMessageAttribute(params string[] helpMessage) {
        HelpMessage = string.Join("\n", helpMessage);
    }
}
