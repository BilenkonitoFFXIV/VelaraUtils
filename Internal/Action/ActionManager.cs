using System;
using System.Runtime.InteropServices;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Game.Network;
using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Plugin;
using JetBrains.Annotations;
using VelaraUtils.Utils;
using ActionManagerStruct = FFXIVClientStructs.FFXIV.Client.Game.ActionManager;
using Status = FFXIVClientStructs.FFXIV.Client.Game.Status;

namespace VelaraUtils.Internal.Action;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public unsafe class ActionManager : IDisposable
{
    [StructLayout(LayoutKind.Explicit)]
    private struct FieldStruct
    {
        [FieldOffset(0x8)] public float AnimationLock;
        [FieldOffset(0x28)] public bool IsCasting;
        [FieldOffset(0x30)] public float ElapsedCastTime;
        [FieldOffset(0x34)] public float CastTime;
        [FieldOffset(0x60)] public float RemainingComboTime;
        [FieldOffset(0x68)] public bool IsQueued;
        [FieldOffset(0x110)] public ushort CurrentSequence;
        [FieldOffset(0x112)] public ushort LastReceivedSequence;
        [FieldOffset(0x610)] public bool IsGCDRecastActive;
        [FieldOffset(0x614)] public uint CurrentGCDAction;
        [FieldOffset(0x618)] public float ElapsedGCDRecastTime;
        [FieldOffset(0x61C)] public float GCDRecastTime;
    }

    private float AnimationLock
    {
        get => _ptr.Cast<FieldStruct>()->AnimationLock;
        set => _ptr.Cast<FieldStruct>()->AnimationLock = value;
    }

    private bool IsCasting
    {
        get => _ptr.Cast<FieldStruct>()->IsCasting;
        set => _ptr.Cast<FieldStruct>()->IsCasting = value;
    }

    private float ElapsedCastTime
    {
        get => _ptr.Cast<FieldStruct>()->ElapsedCastTime;
        set => _ptr.Cast<FieldStruct>()->ElapsedCastTime = value;
    }

    private float CastTime
    {
        get => _ptr.Cast<FieldStruct>()->CastTime;
        set => _ptr.Cast<FieldStruct>()->CastTime = value;
    }

    private float RemainingComboTime
    {
        get => _ptr.Cast<FieldStruct>()->RemainingComboTime;
        set => _ptr.Cast<FieldStruct>()->RemainingComboTime = value;
    }

    private bool IsQueued
    {
        get => _ptr.Cast<FieldStruct>()->IsQueued;
        set => _ptr.Cast<FieldStruct>()->IsQueued = value;
    }

    private ushort CurrentSequence
    {
        get => _ptr.Cast<FieldStruct>()->CurrentSequence;
        set => _ptr.Cast<FieldStruct>()->CurrentSequence = value;
    }

    private ushort LastReceivedSequence
    {
        get => _ptr.Cast<FieldStruct>()->LastReceivedSequence;
        set => _ptr.Cast<FieldStruct>()->LastReceivedSequence = value;
    }

    private bool IsGCDRecastActive
    {
        get => _ptr.Cast<FieldStruct>()->IsGCDRecastActive;
        set => _ptr.Cast<FieldStruct>()->IsGCDRecastActive = value;
    }

    private uint CurrentGCDAction
    {
        get => _ptr.Cast<FieldStruct>()->CurrentGCDAction;
        set => _ptr.Cast<FieldStruct>()->CurrentGCDAction = value;
    }

    private float ElapsedGCDRecastTime
    {
        get => _ptr.Cast<FieldStruct>()->ElapsedGCDRecastTime;
        set => _ptr.Cast<FieldStruct>()->ElapsedGCDRecastTime = value;
    }

    private float GCDRecastTime
    {
        get => _ptr.Cast<FieldStruct>()->GCDRecastTime;
        set => _ptr.Cast<FieldStruct>()->GCDRecastTime = value;
    }

    private NativePointer<ActionManagerStruct> _ptr;

    [PluginService] internal ClientState? ClientState { get; set; }
    [PluginService] internal SigScanner? SigScanner { get; set; }
    [PluginService] internal GameNetwork? GameNetwork { get; set; }

    private ActionManager()
    {
        _ptr = ActionManagerStruct.Instance();
    }

    internal static ActionManager Initialize(DalamudPluginInterface pluginInterface)
    {
        ActionManager actionManager = new ActionManager();
        pluginInterface.Inject(actionManager);

        if (actionManager.ClientState is null || actionManager.SigScanner is null || actionManager.GameNetwork is null) throw new NullReferenceException();

        // actionManager._useActionHook = new Hook<UseActionDelegate>((IntPtr)ActionManagerStruct.fpUseAction, actionManager.UseActionDetour);
        // actionManager._useActionLocationHook = new Hook<UseActionLocationDelegate>((IntPtr)ActionManagerStruct.fpUseActionLocation, actionManager.UseActionLocationDetour);
        // actionManager._castBeginHook = new Hook<CastBeginDelegate>(actionManager.SigScanner.ScanText("40 55 56 48 81 EC ?? ?? ?? ?? 48 8B EA"), actionManager.CastBeginDetour);
        // actionManager._castInterruptHook = new Hook<CastInterruptDelegate>(actionManager.SigScanner.ScanText("E8 ?? ?? ?? ?? 8B 4B 28 8D 41 FF"), actionManager.CastInterruptDetour);
        // actionManager._receiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(actionManager.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 8D F0 03 00 00"), actionManager.ReceiveActionEffectDetour);
        // actionManager._updateStatusHook = new Hook<UpdateStatusDelegate>(actionManager.SigScanner.ScanText("E8 ?? ?? ?? ?? FF C6 48 8D 5B 0C"), actionManager.UpdateStatusDetour);

        // actionManager._defaultClientAnimationLockPtr = actionManager.SigScanner.ScanModule("33 33 B3 3E ?? ?? ?? ?? ?? ?? 00 00 00 3F") + 0xA;
        // // actionManager.DefaultClientAnimationLock = 0.5f;

        // actionManager.GameNetwork.NetworkMessage += actionManager.NetworkMessage;

        // actionManager._useActionHook.Enable();
        // actionManager._useActionLocationHook.Enable();
        // actionManager._castBeginHook.Enable();
        // actionManager._castInterruptHook.Enable();
        // actionManager._receiveActionEffectHook.Enable();
        // actionManager._updateStatusHook.Enable();

        return actionManager;
    }

    private void ReleaseUnmanagedResources()
    {
        // if (GameNetwork is not null)
        //     GameNetwork.NetworkMessage -= NetworkMessage;

        // _useActionHook?.Dispose();
        // OnUseAction = null;
        // _useActionLocationHook?.Dispose();
        // OnUseActionLocation = null;
        // _castBeginHook?.Dispose();
        // OnCastBegin = null;
        // _castInterruptHook?.Dispose();
        // OnCastInterrupt = null;
        // _receiveActionEffectHook?.Dispose();
        // OnReceiveActionEffect = null;
        // _updateStatusHook?.Dispose();
        // OnUpdateStatusList = null;

        // // OnUpdate = null;

        // // DefaultClientAnimationLock = 0.5f;

        _ptr = null;
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~ActionManager()
    {
        ReleaseUnmanagedResources();
    }

    // private IntPtr _defaultClientAnimationLockPtr;
    // public float DefaultClientAnimationLock
    // {
    //     get => *(float*)_defaultClientAnimationLockPtr;
    //     set
    //     {
    //         if (_defaultClientAnimationLockPtr != IntPtr.Zero)
    //             SafeMemory.WriteBytes(_defaultClientAnimationLockPtr, BitConverter.GetBytes(value));
    //     }
    // }

    // public delegate void UseActionEventDelegate(IntPtr actionManager, uint actionType, uint actionID, long targetedActorID, uint param, uint useType, int pvp, IntPtr a8);
    // public event UseActionEventDelegate? OnUseAction;
    // private delegate byte UseActionDelegate(IntPtr actionManager, uint actionType, uint actionID, long targetedActorID, uint param, uint useType, int pvp, IntPtr a8);
    // private Hook<UseActionDelegate>? _useActionHook;
    // private byte UseActionDetour(IntPtr actionManager, uint actionType, uint actionID, long targetedActorID, uint param, uint useType, int pvp, IntPtr a8)
    // {
    //     byte? ret = _useActionHook?.Original(actionManager, actionType, actionID, targetedActorID, param, useType, pvp, a8);
    //     if (ret > 0)
    //         OnUseAction?.Invoke(actionManager, actionType, actionID, targetedActorID, param, useType, pvp, a8);
    //     return ret ?? default;
    // }
    //
    // public delegate void UseActionLocationEventDelegate(IntPtr actionManager, uint actionType, uint actionID, long targetedActorID, IntPtr vectorLocation, uint param);
    // public event UseActionLocationEventDelegate? OnUseActionLocation;
    // private delegate byte UseActionLocationDelegate(IntPtr actionManager, uint actionType, uint actionID, long targetedActorID, IntPtr vectorLocation, uint param);
    // private Hook<UseActionLocationDelegate>? _useActionLocationHook;
    // private byte UseActionLocationDetour(IntPtr actionManager, uint actionType, uint actionID, long targetedActorID, IntPtr vectorLocation, uint param)
    // {
    //     byte? ret = _useActionLocationHook?.Original(actionManager, actionType, actionID, targetedActorID, vectorLocation, param);
    //     if (ret > 0)
    //         OnUseActionLocation?.Invoke(actionManager, actionType, actionID, targetedActorID, vectorLocation, param);
    //     return ret  ?? default;
    // }
    //
    // private bool _invokeCastInterrupt;
    // public delegate void CastBeginDelegate(ulong objectID, IntPtr packetData);
    // public event CastBeginDelegate? OnCastBegin;
    // private Hook<CastBeginDelegate>? _castBeginHook;
    // private void CastBeginDetour(ulong objectID, IntPtr packetData)
    // {
    //     _castBeginHook?.Original(objectID, packetData);
    //     if (objectID != ClientState?.LocalPlayer?.ObjectId) return;
    //     OnCastBegin?.Invoke(objectID, packetData);
    //     _invokeCastInterrupt = true;
    // }
    //
    // public delegate void CastInterruptDelegate(IntPtr actionManager, uint actionType, uint actionID);
    // public event CastInterruptDelegate? OnCastInterrupt;
    // private Hook<CastInterruptDelegate>? _castInterruptHook;
    // private void CastInterruptDetour(IntPtr actionManager, uint actionType, uint actionID)
    // {
    //     _castInterruptHook?.Original(actionManager, actionType, actionID);
    //     if (!_invokeCastInterrupt) return;
    //     OnCastInterrupt?.Invoke(actionManager, actionType, actionID);
    //     _invokeCastInterrupt = false;
    // }
    //
    // public delegate void ReceiveActionEffectEventDelegate(int sourceActorID, IntPtr sourceActor, IntPtr vectorPosition, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail, float oldLock, float newLock);
    // public event ReceiveActionEffectEventDelegate? OnReceiveActionEffect;
    // private delegate void ReceiveActionEffectDelegate(int sourceActorID, IntPtr sourceActor, IntPtr vectorPosition, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
    // private Hook<ReceiveActionEffectDelegate>? _receiveActionEffectHook;
    // private void ReceiveActionEffectDetour(int sourceActorID, IntPtr sourceActor, IntPtr vectorPosition, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail)
    // {
    //     float oldLock = AnimationLock;
    //     _receiveActionEffectHook?.Original(sourceActorID, sourceActor, vectorPosition, effectHeader, effectArray, effectTrail);
    //     OnReceiveActionEffect?.Invoke(sourceActorID, sourceActor, vectorPosition, effectHeader, effectArray, effectTrail, oldLock, AnimationLock);
    // }
    //
    // public delegate void UpdateStatusListEventDelegate(StatusList statusList, short slot, ushort statusID, float remainingTime, ushort stackParam, uint sourceID);
    // public event UpdateStatusListEventDelegate? OnUpdateStatusList;
    // private delegate void UpdateStatusDelegate(IntPtr status, short slot, ushort statusID, float remainingTime, ushort stackParam, uint sourceID, bool individualUpdate);
    // private Hook<UpdateStatusDelegate>? _updateStatusHook;
    // private void UpdateStatusDetour(IntPtr statusList, short slot, ushort statusID, float remainingTime, ushort stackParam, uint sourceID, bool individualUpdate)
    // {
    //     Status* statusPtr = (Status*)(statusList + 0x8 + 0xC * slot);
    //     ushort oldStatusID = statusPtr->StatusID;
    //     uint oldSourceID = statusPtr->SourceID;
    //     _updateStatusHook?.Original(statusList, slot, statusID, remainingTime, stackParam, sourceID, individualUpdate);
    //
    //     if (ClientState?.LocalPlayer is not { } p || statusList.ToInt64() != p.StatusList.Address.ToInt64()) return;
    //
    //     if (statusID != 0 && (oldStatusID != statusID || oldSourceID != sourceID))
    //         OnUpdateStatusList?.Invoke(p.StatusList, slot, statusID, remainingTime, stackParam, sourceID);
    //
    //     if (!individualUpdate && slot == p.StatusList.Length - 1)
    //         OnUpdateStatusList?.Invoke(p.StatusList, -1, 0, 0, 0, 0);
    // }

    // public event GameNetwork.OnNetworkMessageDelegate? OnNetworkMessage;
    // private void NetworkMessage(IntPtr dataPtr, ushort opCode, uint sourceActorId, uint targetActorId, NetworkMessageDirection direction) =>
    //     OnNetworkMessage?.Invoke(dataPtr, opCode, sourceActorId, targetActorId, direction);

    // public event System.Action? OnUpdate;
    // public void Update() => OnUpdate?.Invoke();
}
