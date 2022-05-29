using System;
using System.Runtime.InteropServices;

namespace VelaraUtils.Utils;

public static class PlayLib
{
    private static SendActionDelegate? _sendActionNative;
    private static Func<string, IntPtr>? _getWindowByName;
    private static SetToneUiDelegate? _setToneUi;

    public static void Init()
    {
        _getWindowByName = (Func<string, IntPtr>)(s => VelaraUtils.GameGui!.GetAddonByName(s, 1));
        IntPtr ptr;
        try
        {
            ptr = VelaraUtils.Scanner!.ScanText("48 8B C4 44 88 48 20 53");
        }
        catch
        {
            ptr = VelaraUtils.Scanner!.ScanText("E8 ?? ?? ?? ?? 8B 44 24 20 C1 E8 05");
        }

        _sendActionNative = Marshal.GetDelegateForFunctionPointer<SendActionDelegate>(ptr);
        _setToneUi = Marshal.GetDelegateForFunctionPointer<SetToneUiDelegate>(VelaraUtils.Scanner.ScanText("83 FA 04 77 4E"));
    }

    private static unsafe void SendAction(IntPtr ptr, params ulong[] param)
    {
        if (param.Length % 2 != 0)
            throw new ArgumentException("The parameter length must be an integer multiple of 2.");
        if (ptr == IntPtr.Zero)
            throw new ArgumentException("input pointer is null");
        int a2 = param.Length / 2;
        fixed (ulong* a3 = param)
        {
            _sendActionNative!((long)ptr, a2, a3, 1);
        }
    }

    public static bool PressKey(int keynumber, ref int offset, ref int octave)
    {
        if (!TargetWindowPtr(out var miniMode, out var targetWindowPtr))
            return false;
        offset = 0;
        octave = 0;
        if (miniMode)
            keynumber = ConvertMiniKeyNumber(keynumber, ref offset, ref octave);
        SendAction(targetWindowPtr, 3UL, 1UL, 4UL, (ulong)keynumber);
        return true;
    }

    public static bool ReleaseKey(int keynumber)
    {
        if (!TargetWindowPtr(out var miniMode, out var targetWindowPtr))
            return false;
        if (miniMode)
            keynumber = ConvertMiniKeyNumber(keynumber);
        SendAction(targetWindowPtr, 3UL, 2UL, 4UL, (ulong)keynumber);
        return true;
    }

    private static int ConvertMiniKeyNumber(int keynumber)
    {
        keynumber -= 12;
        if (keynumber >= 0)
        {
            if (keynumber > 12)
                keynumber -= 12;
        }
        else
            keynumber += 12;

        return keynumber;
    }

    private static int ConvertMiniKeyNumber(int keynumber, ref int offset, ref int octave)
    {
        keynumber -= 12;
        if (keynumber >= 0)
        {
            if (keynumber <= 12) return keynumber;

            keynumber -= 12;
            offset = 12;
            octave = 1;
        }
        else
        {
            keynumber += 12;
            offset = -12;
            octave = -1;
        }

        return keynumber;
    }

    private static bool TargetWindowPtr(out bool miniMode, out IntPtr targetWindowPtr)
    {
        targetWindowPtr = _getWindowByName!("PerformanceMode");
        if (targetWindowPtr != IntPtr.Zero)
        {
            miniMode = true;
            return true;
        }

        targetWindowPtr = _getWindowByName("PerformanceModeWide");
        if (targetWindowPtr != IntPtr.Zero)
        {
            miniMode = false;
            return true;
        }

        miniMode = false;
        return false;
    }

    public static bool ConfirmReceiveReadyCheck()
    {
        IntPtr ptr = _getWindowByName!("PerformanceReadyCheckReceive");
        if (ptr == IntPtr.Zero)
            return false;
        SendAction(ptr, 3UL, 2UL);
        return true;
    }

    public static bool GuitarSwitchTone(int tone)
    {
        IntPtr num = _getWindowByName!("PerformanceToneChange");
        if (num == IntPtr.Zero)
            return false;
        SendAction(num, 3UL, 0UL, 3UL, (ulong)tone);
        _setToneUi!((long)num, (uint)tone);
        return true;
    }

    public static bool BeginReadyCheck()
    {
        IntPtr ptr = _getWindowByName!("PerformanceMetronome");
        if (ptr == IntPtr.Zero)
            return false;
        SendAction(ptr, 3UL, 2UL, 2UL, 0UL);
        return true;
    }

    public static bool ConfirmBeginReadyCheck()
    {
        IntPtr ptr = _getWindowByName!("PerformanceReadyCheck");
        if (ptr == IntPtr.Zero)
            return false;
        SendAction(ptr, 3UL, 2UL);
        return true;
    }

    public static bool CancelReadyCheck()
    {
        IntPtr ptr = _getWindowByName!("SelectYesno");
        if (ptr == IntPtr.Zero)
            return false;
        SendAction(ptr, 3UL, 0UL, 3UL, 0UL);
        return true;
    }

    private unsafe delegate byte SendActionDelegate(long a1, int a2, void* a3, byte a4);

    private delegate void SetToneUiDelegate(long windowPtr, uint tone);
}
