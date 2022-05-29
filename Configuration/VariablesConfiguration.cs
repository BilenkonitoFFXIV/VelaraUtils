using System.Collections.Generic;

namespace VelaraUtils.Configuration;

public interface IVariablesConfiguration
{
    public Dictionary<string, string> Variables { get; }
}

public class VariablesConfiguration : IVariablesConfiguration
{
    public Dictionary<string, string> Variables { get; set; } = new();
}
