using System;
using System.Runtime.InteropServices;

namespace VelaraUtils.Internal;

internal abstract class GameFunctionBase<T> where T : Delegate {
    public IntPtr Address { get; private set; }
    private T? _function;
    public bool Valid => _function is not null || Address != IntPtr.Zero;
    public T? Delegate {
        get {
            if (_function is not null)
                return _function;
            if (Address != IntPtr.Zero) {
                _function = Marshal.GetDelegateForFunctionPointer<T>(Address);
                return _function;
            }
            Logger.Error($"{GetType().Name} invocation FAILED: no pointer available");
            return null;
        }
    }
    internal GameFunctionBase(string sig, int offset = 0) {
        Address = VelaraUtils.Scanner?.ScanText(sig) ?? IntPtr.Zero;
        if (Address != IntPtr.Zero) {
            Address += offset;
#if DEBUG
				ulong totalOffset = VelaraUtils.Scanner != null ? (ulong) Address.ToInt64() - (ulong) VelaraUtils.Scanner.Module.BaseAddress.ToInt64() : 0;
                Logger.Debug($"{GetType().Name} loaded; address = 0x{Address.ToInt64():X16}, base memory offset = 0x{totalOffset:X16}");
#endif
        }
        else {
            Logger.Warning($"{GetType().Name} FAILED, could not find address from signature: ${sig.ToUpper()}");
        }
    }
    public dynamic? Invoke(params dynamic[] parameters)
        => Delegate?.DynamicInvoke(parameters);
}
