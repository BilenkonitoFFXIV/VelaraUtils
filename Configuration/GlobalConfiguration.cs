using Dalamud.Configuration;

namespace VelaraUtils.Configuration;

public class GlobalConfiguration : IPluginConfiguration
{
    public int Version { get; set; }

     public void Initialize()
     {
         Save();
     }

     public void Save() =>
         VelaraUtils.PluginInterface!.SavePluginConfig(this);
}
