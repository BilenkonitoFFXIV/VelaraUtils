using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Plugin;
using VelaraUtils.Attributes;

namespace VelaraUtils.Internal.Command;

internal sealed class PluginCommandManager : IDisposable
{
    private readonly List<CommandModule> _commandModules;
    public IEnumerable<CommandModule> CommandModules => _commandModules;

    internal PluginCommandManager(DalamudPluginInterface pluginInterface)
    {
        _commandModules = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where type.IsClass && !type.IsAbstract && type.IsVisible &&
                  type.IsAssignableTo(typeof(ICommandModule)) &&
                  type.GetConstructor(Array.Empty<Type>()) is not null
            let attr = type.GetCustomAttribute<CommandModuleAttribute>()
            where attr is not null
            let moduleImpl = (ICommandModule?)Activator.CreateInstance(type)
            where moduleImpl is not null
            let module = new CommandModule(attr.Name, attr.Prefix, moduleImpl)
            where module is not null
            select module).ToList();

        _commandModules.ForEach(module => module.Load(pluginInterface));
    }

    #region IDisposable Support
    private void Dispose(bool disposing)
    {
        if (!disposing) return;

        _commandModules.ForEach(module => module.Unload());
        _commandModules.Clear();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~PluginCommandManager()
    {
        Dispose(false);
    }
    #endregion
}
