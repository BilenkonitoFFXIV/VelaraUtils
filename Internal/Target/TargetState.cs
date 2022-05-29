using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.IoC;
using Dalamud.Plugin;
using JetBrains.Annotations;

namespace VelaraUtils.Internal.Target;

public enum TargetType
{
    NormalTarget,
    SoftTarget,
    MouseOverTarget,
    FocusTarget,
    PreviousTarget
}

[UsedImplicitly]
public class TargetState : DynamicObject
{
    // public override IEnumerable<string> GetDynamicMemberNames() =>
    //     typeof(TargetManager).GetMembers(BindingFlags.Instance | BindingFlags.Public).Select(m => m.Name);
    public override bool TryGetIndex(GetIndexBinder binder, object?[] indices, out object? result)
    {
        switch (indices.Length)
        {
            case < 1:
                result = null;
                return false;
            case 1:
                result = GetTarget(indices[0]);
                break;
            default:
            {
                GameObject?[] res = new GameObject[indices.Length];
                for (int i = 0; i < indices.Length; i++)
                    res[i] = GetTarget(indices[i]);
                result = res;
                break;
            }
        }

        return true;
    }

    public override bool TrySetIndex(SetIndexBinder binder, object?[] indices, object? value)
    {
        switch (indices.Length)
        {
            case < 1:
                return false;
            case 1:
                SetTarget(indices[0], value);
                break;
            default:
            {
                foreach (object? t in indices)
                    SetTarget(t, value);
                break;
            }
        }

        return true;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        result = null;

        MemberInfo? mi = typeof(TargetManager).GetMember(binder.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).FirstOrDefault();
        if (mi is null) return false;

        switch (mi.MemberType)
        {
            case MemberTypes.Property:
            {
                PropertyInfo pi = (PropertyInfo)mi;
                if (!pi.CanRead) return false;

                try
                {
                    result = pi.GetValue(TargetManager);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            case MemberTypes.Field:
                FieldInfo fi = (FieldInfo)mi;

                try
                {
                    result = fi.GetValue(TargetManager);
                    return true;
                }
                catch
                {
                    return false;
                }
            case MemberTypes.Method:
                MethodInfo mei = (MethodInfo)mi;

                try
                {
                    result = mei.CreateDelegate<Action<object?[]?>>(
                        mei.IsStatic ?
                            null :
                            TargetManager);
                    return true;
                }
                catch
                {
                    return false;
                }
            case MemberTypes.Event:
            case MemberTypes.Constructor:
            case MemberTypes.TypeInfo:
            case MemberTypes.Custom:
            case MemberTypes.NestedType:
            case MemberTypes.All:
            default:
                return false;
        }
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        MemberInfo? mi = typeof(TargetManager).GetMember(binder.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).FirstOrDefault();
        if (mi is null) return false;

        switch (mi.MemberType)
        {
            case MemberTypes.Property:
            {
                PropertyInfo pi = (PropertyInfo)mi;
                if (!pi.CanWrite) return false;

                try
                {
                    pi.SetValue(TargetManager, value);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            case MemberTypes.Field:
                FieldInfo fi = (FieldInfo)mi;

                try
                {
                    fi.SetValue(TargetManager, value);
                    return true;
                }
                catch
                {
                    return false;
                }
            case MemberTypes.Method:
            case MemberTypes.Event:
            case MemberTypes.Constructor:
            case MemberTypes.TypeInfo:
            case MemberTypes.Custom:
            case MemberTypes.NestedType:
            case MemberTypes.All:
            default:
                return false;
        }
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        try
        {
            result = typeof(TargetManager).InvokeMember(binder.Name, BindingFlags.InvokeMethod, null, TargetManager, args);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        if (binder.Type == typeof(TargetManager))
            return base.TryConvert(binder, out result);
        result = TargetManager;
        return true;
    }

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1822", MessageId = "Mark members as static")]
    public GameObject? Target
    {
        get => GetTarget(TargetType.NormalTarget);
        set => SetTarget(TargetType.NormalTarget, value);
    }

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1822", MessageId = "Mark members as static")]
    public GameObject? MouseOverTarget
    {
        get => GetTarget(TargetType.MouseOverTarget);
        set => SetTarget(TargetType.MouseOverTarget, value);
    }

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1822", MessageId = "Mark members as static")]
    public GameObject? FocusTarget
    {
        get => GetTarget(TargetType.FocusTarget);
        set => SetTarget(TargetType.FocusTarget, value);
    }

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1822", MessageId = "Mark members as static")]
    public GameObject? PreviousTarget
    {
        get => GetTarget(TargetType.PreviousTarget);
        set => SetTarget(TargetType.PreviousTarget, value);
    }

    [UsedImplicitly]
    [SuppressMessage("Performance", "CA1822", MessageId = "Mark members as static")]
    public GameObject? SoftTarget
    {
        get => GetTarget(TargetType.SoftTarget);
        set => SetTarget(TargetType.SoftTarget, value);
    }

    [PluginService] private static TargetManager? TargetManager { get; [UsedImplicitly] set; }

    public static TargetState Initialize(DalamudPluginInterface pluginInterface) =>
        pluginInterface.Create<TargetState>()!;

    private static GameObject? GetTarget(TargetType targetType) => GetTarget((object?)targetType);

    private static GameObject? GetTarget(object? targetType) =>
        targetType switch
        {
            TargetType.NormalTarget => TargetManager?.Target,
            TargetType.SoftTarget => TargetManager?.SoftTarget,
            TargetType.MouseOverTarget => TargetManager?.MouseOverTarget,
            TargetType.FocusTarget => TargetManager?.FocusTarget,
            TargetType.PreviousTarget => TargetManager?.PreviousTarget,
            null => throw new ArgumentNullException(nameof(targetType)),
            _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null)
        };

    private static void SetTarget(TargetType targetType, GameObject? value) => SetTarget((object?)targetType, value);

    private static void SetTarget(object? targetType, object? value)
    {
        switch (targetType)
        {
            case TargetType.NormalTarget:
                TargetManager?.SetTarget((GameObject?)value);
                break;
            case TargetType.SoftTarget:
                TargetManager?.SetSoftTarget((GameObject?)value);
                break;
            case TargetType.MouseOverTarget:
                TargetManager?.SetMouseOverTarget((GameObject?)value);
                break;
            case TargetType.FocusTarget:
                TargetManager?.SetFocusTarget((GameObject?)value);
                break;
            case TargetType.PreviousTarget:
                TargetManager?.SetPreviousTarget((GameObject?)value);
                break;
            case null:
                throw new ArgumentNullException(nameof(targetType));
            default:
                throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
        }
    }
}
