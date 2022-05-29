using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using JetBrains.Annotations;
using VelaraUtils.Chat;

namespace VelaraUtils.Internal.IPC;

public class QolBar : IDisposable
{
    public interface IConditionSet
    {
        public int Index { get; }
        public string Name { get; }
        public bool State { get; }
    }

    private class ConditionSet : IConditionSet
    {
        public int Index { get; }
        public string Name { get; }
        public bool State => VelaraUtils.QolBar?.CheckConditionSet(Index) ?? false;

        public ConditionSet(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }

    public bool Enabled { get; private set; }
    private readonly ICallGateSubscriber<string>? _getVersionProvider;
    private readonly ICallGateSubscriber<int>? _getIpcVersionSubscriber;
    private readonly ICallGateSubscriber<string[]>? _getConditionSetsProvider;
    private readonly ICallGateSubscriber<int, bool>? _checkConditionSetProvider;
    private readonly ICallGateSubscriber<int, int, object>? _movedConditionSetProvider;
    private readonly ICallGateSubscriber<int, object>? _removedConditionSetProvider;

    public int IpcVersion
    {
        get
        {
            try
            {
                return _getIpcVersionSubscriber?.InvokeFunc() ?? 0;
            }
            catch
            {
                return 0;
            }
        }
    }

    public string Version
    {
        get
        {
            try
            {
                return _getVersionProvider?.InvokeFunc() ?? "0.0.0.0";
            }
            catch
            {
                return "0.0.0.0";
            }
        }
    }

    public IEnumerable<IConditionSet> ConditionSets
    {
        get
        {
            List<ConditionSet> res = new();
            try
            {
                res.AddRange(
                    (_getConditionSetsProvider?.InvokeFunc() ?? Array.Empty<string>())
                    .Select((t, i) => new ConditionSet(i, t)));
            }
            catch (Exception e)
            {
                ChatUtil.ShowPrefixedError(
                    ChatColour.CONDITION_FAILED,
                    e.ToString(),
                    ChatColour.RESET);
            }
            return res;
        }
    }

    public IConditionSet? this[int i]
    {
        get
        {
            string[] conditionSets = _getConditionSetsProvider?.InvokeFunc() ?? Array.Empty<string>();
            return i >= 0 && i < conditionSets.Length ?
                new ConditionSet(i, conditionSets[i]) :
                null;
        }
    }

    public IConditionSet? this[string name]
    {
        get
        {
            name = name.Trim();
            string[] conditionSets = _getConditionSetsProvider?.InvokeFunc() ?? Array.Empty<string>();
            for (int i = 0; i < conditionSets.Length; i++)
            {
                string setName = conditionSets[i].Trim();
                if (string.Equals(name, setName, StringComparison.InvariantCultureIgnoreCase))
                    return new ConditionSet(i, setName);
            }
            return null;
        }
    }

    public delegate void OnMovedConditionSetDelegate(int from, int to);
    public event OnMovedConditionSetDelegate? OnMovedConditionSet;

    public delegate void OnRemovedConditionSetDelegate(int removed);
    public event OnRemovedConditionSetDelegate? OnRemovedConditionSet;

    internal QolBar(DalamudPluginInterface pluginInterface)
    {
        _getIpcVersionSubscriber = pluginInterface.GetIpcSubscriber<int>("QoLBar.GetIPCVersion");
        if (IpcVersion != 1) return;

        _getVersionProvider = pluginInterface.GetIpcSubscriber<string>("QoLBar.GetVersion");
        _getConditionSetsProvider = pluginInterface.GetIpcSubscriber<string[]>("QoLBar.GetConditionSets");
        _checkConditionSetProvider = pluginInterface.GetIpcSubscriber<int, bool>("QoLBar.CheckConditionSet");
        _movedConditionSetProvider = pluginInterface.GetIpcSubscriber<int, int, object>("QoLBar.MovedConditionSet");
        _removedConditionSetProvider = pluginInterface.GetIpcSubscriber<int, object>("QoLBar.RemovedConditionSet");

        _movedConditionSetProvider.Subscribe(OnMovedConditionSetHandler);
        _removedConditionSetProvider.Subscribe(OnRemovedConditionSetHandler);

        Enabled = true;
    }

    private bool CheckConditionSet(int i)
    {
        try
        {
            return i >= 0 && (_checkConditionSetProvider?.InvokeFunc(i) ?? false);
        }
        catch
        {
            return false;
        }
    }

    private void OnMovedConditionSetHandler(int from, int to) =>
        OnMovedConditionSet?.Invoke(from, to);
    // {
    //     foreach (var preset in Cammy.Config.Presets)
    //     {
    //         if (preset.ConditionSet == from)
    //             preset.ConditionSet = to;
    //         else if (preset.ConditionSet == to)
    //             preset.ConditionSet = from;
    //     }
    //     Cammy.Config.Save();
    // }

    private void OnRemovedConditionSetHandler(int removed) =>
        OnRemovedConditionSet?.Invoke(removed);
    // {
    //     foreach (var preset in Cammy.Config.Presets)
    //     {
    //         if (preset.ConditionSet > removed)
    //             preset.ConditionSet -= 1;
    //         else if (preset.ConditionSet == removed)
    //             preset.ConditionSet = -1;
    //     }
    //     Cammy.Config.Save();
    // }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources()
    {
        if (!Enabled) return;
        _movedConditionSetProvider?.Unsubscribe(OnMovedConditionSetHandler);
        _removedConditionSetProvider?.Unsubscribe(OnRemovedConditionSetHandler);
        Enabled = false;
    }

    ~QolBar()
    {
        ReleaseUnmanagedResources();
    }
}
