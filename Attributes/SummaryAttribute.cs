using System;

namespace VelaraUtils.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class SummaryAttribute: Attribute {
    public string Summary { get; }

    public SummaryAttribute(string helpMessage) {
        Summary = helpMessage;
    }
}
