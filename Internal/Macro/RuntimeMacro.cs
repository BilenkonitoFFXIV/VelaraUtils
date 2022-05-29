using System;
using System.Collections.Generic;
using System.Text;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace VelaraUtils.Internal.Macro;

using Macro = RaptureMacroModule.Macro;

public sealed unsafe class RuntimeMacro : GameMacro, IDisposable
{
    private RuntimeMacro(Macro* ptr)
        : base(ptr)
    {
    }

    public RuntimeMacro(string title, params string[] lines)
        : this(lines, title)
    {
    }

    public RuntimeMacro(IReadOnlyList<string> lines, string title, uint icon = 66001, uint key = 1)
    {
        Ptr = (Macro*)IMemorySpace.GetDefaultSpace()->Malloc<Macro>();
        if ((IntPtr)Ptr == IntPtr.Zero) throw new OutOfMemoryException();

        Ptr->IconId = icon;
        Ptr->Unk = key;

        Ptr->Name.Ctor();
        fixed (byte* cStr = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(title) ? "\0" : title + "\0"))
            Ptr->Name.SetString(cStr);

        for (int i = 0; i < 15; i++)
        {
            Utf8String* linePtr = Ptr->Line[i];
            linePtr->Ctor();
            fixed (byte* cStr = Encoding.UTF8.GetBytes(i < lines.Count ? lines[i] + "\0" : "\0"))
                linePtr->SetString(cStr);
        }
    }

    private void ReleaseUnmanagedResources()
    {
        // Ptr->Name.Dtor();
        // for (int i = 0; i < 15; i++)
        //     Ptr->Line[i]->Dtor();
        if (!IsValid) return;
        IMemorySpace.Free(Ptr);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~RuntimeMacro()
    {
        ReleaseUnmanagedResources();
    }

    public static implicit operator RuntimeMacro(Macro* other) => new(other);
    public static implicit operator Macro*(RuntimeMacro other) => other.Ptr;
}


// [UsedImplicitly(ImplicitUseTargetFlags.Members)]
// [StructLayout(LayoutKind.Sequential, Size = 0x688)]
// internal readonly struct MacroStruct : IDisposable
// {
//     public const int NumLines = 15;
//     public const int Size = 0x8 + Utf8String.Size * (NumLines + 1);
//
//     public readonly uint Icon;
//     public readonly uint Key;
//     public readonly Utf8String Title;
//
//     [MarshalAs(UnmanagedType.ByValArray, SizeConst = NumLines)]
//     public readonly Utf8String[] Lines;
//
//     public MacroStruct(IntPtr basePtr, string title, IReadOnlyList<string> commands, uint icon = 66001, uint key = 1)
//     {
//         Icon = icon;
//         Key = key;
//         Title = new Utf8String(basePtr + 0x8, title);
//         Lines = new Utf8String[NumLines];
//         for (int i = 0; i < NumLines; i++)
//         {
//             string command = commands.Count > i ? commands[i] : string.Empty;
//             Lines[i] = new Utf8String(basePtr + 0x8 + Utf8String.Size * (i + 1), command);
//         }
//     }
//
//     public void Dispose()
//     {
//         Title.Dispose();
//         for (int i = 0; i < NumLines; i++)
//             Lines[i].Dispose();
//     }
// }
