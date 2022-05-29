using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dalamud.Plugin;
using Newtonsoft.Json;

namespace VelaraUtils.Configuration;

internal class ConfigLoader
{
    private readonly DirectoryInfo _configDirectory;

    public ConfigLoader(DalamudPluginInterface pluginInterface)
    {
        _configDirectory = new DirectoryInfo(pluginInterface.GetPluginConfigDirectory());
    }

    public T Load<T>(string name)
        where T : class, new()
    {
        FileInfo fi = new FileInfo(Path.Combine(_configDirectory.FullName, name + ".json"));
        return (fi.Exists ?
            JsonConvert.DeserializeObject<T>(File.ReadAllText(fi.FullName)) :
            null) ?? new T();
    }

    public void Save<T>(string name, T value)
    {
        FileInfo fi = new FileInfo(Path.Combine(_configDirectory.FullName, name + ".json"));
        File.WriteAllText(fi.FullName, JsonConvert.SerializeObject(value, Formatting.Indented));
    }
}

// internal class ConfigLoader
// {
//     private static readonly IDictionaryAdapterFactory AdapterFactory = new DictionaryAdapterFactory();
//     private static readonly IDictionary<string, Hashtable> ConfigValues = new Dictionary<string, Hashtable>();
//
//     private readonly DirectoryInfo _configDirectory;
//
//     public ConfigLoader(DalamudPluginInterface pluginInterface)
//     {
//         _configDirectory = new DirectoryInfo(pluginInterface.GetPluginConfigDirectory());
//     }
//
//     public T Load<T>(string name) where T : class
//     {
//         FileInfo fi = new FileInfo(Path.Combine(_configDirectory.FullName, name + ".json"));
//         Hashtable values = ConfigValues[name] = fi.Exists ?
//             JsonConvert.DeserializeObject<Hashtable>(File.ReadAllText(fi.FullName)) ?? new Hashtable() :
//             new Hashtable();
//         return AdapterFactory.GetAdapter<T>(values);
//     }
//
//     public void Save(string name)
//     {
//         FileInfo fi = new FileInfo(Path.Combine(_configDirectory.FullName, name + ".json"));
//         if (!ConfigValues.TryGetValue(name, out Hashtable? values))
//             values = ConfigValues[name] = new Hashtable();
//         File.WriteAllText(fi.FullName, JsonConvert.SerializeObject(values, Formatting.Indented));
//     }
// }
