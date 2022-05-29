using System;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using JetBrains.Annotations;
using VelaraUtils.Utils;

namespace VelaraUtils.Internal.Macro;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MacroManager : IDisposable
{
    private NativePointer<RaptureShellModule> _raptureShellModule;
    private NativePointer<RaptureMacroModule> _raptureMacroModule;

    // [PluginService] internal SigScanner? SigScanner { get; set; }

    private MacroManager(
        NativePointer<RaptureShellModule> raptureShellModule,
        NativePointer<RaptureMacroModule> raptureMacroModule)
    {
        _raptureShellModule = raptureShellModule;
        _raptureMacroModule = raptureMacroModule;
    }

    internal static unsafe MacroManager Initialize(DalamudPluginInterface pluginInterface)
    {
        MacroManager macroManager = new MacroManager(RaptureShellModule.Instance, RaptureMacroModule.Instance);
        pluginInterface.Inject(macroManager);

        // if (macroManager.SigScanner is null) throw new NullReferenceException();

        // macroManager._executeMacroHook = new Hook<ExecuteMacroDelegate>((IntPtr)RaptureShellModule.fpExecuteMacro, macroManager.ExecuteMacroDetour);
        // macroManager._executeMacroHook.Enable();
        //
        // macroManager._setMacroLinesHook = new Hook<SetMacroLinesDelegate>((IntPtr)RaptureMacroModule.fpSetMacroLines, macroManager.SetMacroLinesDetour);
        // macroManager._setMacroLinesHook.Enable();

        return macroManager;
    }

    private void ReleaseUnmanagedResources()
    {
        // _executeMacroHook?.Dispose();
        // OnExecuteMacro = null;
        // _setMacroLinesHook?.Dispose();
        // OnSetMacroLines = null;

        _raptureShellModule = null;
        _raptureMacroModule = null;
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~MacroManager()
    {
        ReleaseUnmanagedResources();
    }

    public unsafe int CurrentLine =>
        _raptureShellModule.IsValid ?
            _raptureShellModule.Value->MacroCurrentLine :
            -1;

    public bool IsRunning =>
        CurrentLine >= 0;

    public unsafe bool Locked =>
        _raptureShellModule.IsValid &&
        _raptureShellModule.Value->MacroLocked;

    public unsafe bool Execute(GameMacro macro)
    {
        bool res = macro.IsValid && _raptureShellModule.IsValid && !Locked;
        try
        {
            if (res)
                _raptureShellModule.Value->ExecuteMacro(macro);
        }
        catch
        {
            res = false;
        }
        return res;
    }

    // public bool IsMacroRunning => _raptureShellModule.Value->MacroCurrentLine >= 0;
    // public bool IsMacroLocked => _raptureShellModule.Value->MacroLocked;

    // public delegate void ExecuteMacroDelegate(RaptureShellModule* raptureShellModule, GameMacro macro);
    // public event ExecuteMacroDelegate? OnExecuteMacro;
    // private Hook<ExecuteMacroDelegate>? _executeMacroHook;
    // private void ExecuteMacroDetour(RaptureShellModule* raptureShellModule, GameMacro macro)
    // {
    //     ChatUtil.ShowPrefixedMessage(ChatColour.DEBUG, "ExecuteMacroDetour:", ChatColour.RESET);
    //     foreach (IReadOnlyList<object> chatLines in macro.ToChatMessage())
    //         ChatUtil.ShowMessage(chatLines);
    //
    //     _executeMacroHook?.Original(raptureShellModule, macro.Value);
    //     OnExecuteMacro?.Invoke(raptureShellModule, macro);
    // }

    // public delegate void SetMacroLinesDelegate(RaptureMacroModule* raptureMacroModule, GameMacro macro, int lineStartIndex, Utf8String* lines);
    // public event SetMacroLinesDelegate? OnSetMacroLines;
    // private Hook<SetMacroLinesDelegate>? _setMacroLinesHook;
    // private void SetMacroLinesDetour(RaptureMacroModule* raptureMacroModule, GameMacro macro, int lineStartIndex, Utf8String* lines)
    // {
    //     ChatUtil.ShowPrefixedMessage(ChatColour.DEBUG, "SetMacroLinesDetour:", ChatColour.RESET);
    //     foreach (IReadOnlyList<object> chatLines in macro.ToChatMessage())
    //         ChatUtil.ShowMessage(chatLines);
    //
    //     _setMacroLinesHook?.Original(raptureMacroModule, macro.Value, lineStartIndex, lines);
    //     OnSetMacroLines?.Invoke(raptureMacroModule, macro, lineStartIndex, lines);
    // }
}


// [UsedImplicitly(ImplicitUseTargetFlags.Members)]
// public unsafe class MacroManager : IDisposable
// {
//     public delegate void ExecuteMacroDelegate(RaptureShellModule* raptureShellModule, IntPtr macro);
//
//     private UIModule* _uiModule;
//
//     // [Signature("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 8D 4D 28")]
//     // private Hook<ExecuteMacroDelegate> _executeMacroHook = null!;
//
//     private RaptureShellModule* _raptureShellModule;
//     private RaptureMacroModule* _raptureMacroModule;
//
//     // private IntPtr _numCopiedMacroLinesPtr = IntPtr.Zero;
//     // public byte NumCopiedMacroLines
//     // {
//     //     get => *(byte*)_numCopiedMacroLinesPtr;
//     //     set
//     //     {
//     //         if (_numCopiedMacroLinesPtr != IntPtr.Zero)
//     //             SafeMemory.WriteBytes(_numCopiedMacroLinesPtr, new[] { value });
//     //     }
//     // }
//
//     // private IntPtr _numExecutedMacroLinesPtr = IntPtr.Zero;
//     // public byte NumExecutedMacroLines
//     // {
//     //     get => *(byte*)_numExecutedMacroLinesPtr;
//     //     set
//     //     {
//     //         if (_numExecutedMacroLinesPtr != IntPtr.Zero)
//     //             SafeMemory.WriteBytes(_numExecutedMacroLinesPtr, new[] { value });
//     //     }
//     // }
//
//     // public bool IsMacroRunning => *(int*)((IntPtr)_raptureShellModule + 0x2C0) >= 0;
//
//     [PluginService] internal SigScanner SigScanner { get; private set; } = null!;
//
//     public static MacroManager Initialize(DalamudPluginInterface pluginInterface)
//     {
//         MacroManager macroManager = new();
//         macroManager.InitializeInstance(pluginInterface);
//         return macroManager;
//     }
//
//     private MacroManager()
//     {
//         _uiModule = Framework.Instance()->GetUiModule();
//         _raptureShellModule = _uiModule->GetRaptureShellModule();
//         _raptureMacroModule = _uiModule->GetRaptureMacroModule();
//     }
//
//     ~MacroManager()
//     {
//         Dispose(false);
//     }
//
//     private void InitializeInstance(DalamudPluginInterface pluginInterface)
//     {
//         pluginInterface.Inject(this);
//         SignatureHelper.Initialise(this);
//
//         // _numCopiedMacroLinesPtr = SigScanner.ScanText("49 8D 5E 70 BF ?? 00 00 00") + 0x5;
//         // _numExecutedMacroLinesPtr = SigScanner.ScanText("41 83 F8 ?? 0F 8D ?? ?? ?? ?? 49 6B C8 68") + 0x3;
//
//         // _executeMacroHook.Enable();
//     }
//
//     private void ExecuteMacroDetour(RaptureMacroModule.Macro* macro)
//     {
//         // NumCopiedMacroLines = MacroStruct.NumLines;
//         // NumExecutedMacroLines = MacroStruct.NumLines;
//
//         _raptureShellModule->ExecuteMacro(macro);
//         // _executeMacroHook.Original(raptureShellModule, macro);
//     }
//
//     // private void CreateCustomMacro(string name, IReadOnlyList<string> commands, uint iconId = 66001, uint key = 1)
//     // {
//     //     RaptureMacroModule.Macro macro = new RaptureMacroModule.Macro
//     //     {
//     //         IconId = iconId,
//     //         Unk = key,
//     //         Name = Utf8String.FromString(name, IMemorySpace.GetUISpace()).
//     //     };
//     // }
//
//     public void ExecuteMacro(uint set, uint index)
//     {
//         if (index <= 99)
//         {
//             // _executeMacroHook.Original(_raptureShellModule, (IntPtr)_raptureMacroModule + 0x58 + MacroStruct.Size * id);
//             RaptureMacroModule.Macro* macroPtr = _raptureMacroModule->GetMacro(set, index);
//             _raptureShellModule->ExecuteMacro((RaptureMacroModule.Macro*)macroPtr);
//         }
//         else
//         {
//             ChatUtil.ShowPrefixedError(
//                 ChatColour.ERROR,
//                 "Invalid macro ",
//                 ChatColour.RESET,
//                 ChatColour.CONDITION_FAILED,
//                 $"'{set.ToString()}/{index.ToString()}'",
//                 ChatColour.RESET,
//                 ChatColour.ERROR,
//                 ". Macro id must be 0-99.",
//                 ChatColour.RESET);
//         }
//     }
//
//     private void ExecuteCustomMacro(Macro macro)
//     {
//         IntPtr macroPtr = IntPtr.Zero;
//         try
//         {
//             if (commands.Count > 15)
//             {
//                 ChatUtil.ShowPrefixedError("Macros using more than 15 lines are not supported!");
//                 throw new InvalidOperationException();
//             }
//
//             macroPtr = Marshal.AllocHGlobal(Macro.Size);
//             using Macro macro = new Macro(macroPtr, title, commands.ToArray());
//             Marshal.StructureToPtr(macro, macroPtr, false);
//
//             _raptureShellModule->ExecuteMacro((RaptureMacroModule.Macro*)macroPtr);
//
//             // NumCopiedMacroLines = count;
//             // NumExecutedMacroLines = count;
//
//             // _executeMacroHook.Original(_raptureShellModule, macroPtr);
//             //
//             // NumCopiedMacroLines = MacroStruct.NumLines;
//         }
//         catch
//         {
//             ChatUtil.ShowPrefixedError("Failed injecting macro");
//         }
//         finally
//         {
//             if (macroPtr != IntPtr.Zero)
//                 Marshal.FreeHGlobal(macroPtr);
//         }
//     }
//
//     private void Dispose(bool disposing)
//     {
//         if (!disposing) return;
//
//         // _executeMacroHook.Dispose();
//         SigScanner.Dispose();
//     }
//
//     public void Dispose()
//     {
//         Dispose(true);
//         GC.SuppressFinalize(this);
//     }
// }
