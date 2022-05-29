// using System;
// using System.Runtime.InteropServices;
// using System.Text;
// using JetBrains.Annotations;
//
// namespace VelaraUtils.Internal.Macro;
//
// [UsedImplicitly(ImplicitUseTargetFlags.Members)]
// [StructLayout(LayoutKind.Sequential, Size = 0x68)]
// public readonly struct Utf8String : IDisposable
// {
//     public const int Size = 0x68;
//
//     public readonly IntPtr StringPtr;
//     public readonly ulong Capacity;
//     public readonly ulong Length;
//     public readonly ulong Unknown;
//     public readonly byte IsEmpty;
//     public readonly byte NotReallocated;
//
//     [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
//     public readonly byte[] Str;
//
//     public Utf8String(IntPtr loc, string text) : this(loc, Encoding.UTF8.GetBytes(text))
//     {
//     }
//
//     public Utf8String(IntPtr loc, byte[] text)
//     {
//         Capacity = 0x40;
//         Length = (ulong)text.Length + 1;
//         Str = new byte[Capacity];
//
//         if (Length > Capacity)
//         {
//             StringPtr = Marshal.AllocHGlobal(text.Length + 1);
//             Capacity = Length;
//             Marshal.Copy(text, 0, StringPtr, text.Length);
//             Marshal.WriteByte(StringPtr, text.Length, 0);
//             NotReallocated = 0;
//         }
//         else
//         {
//             StringPtr = loc + 0x22;
//             text.CopyTo(Str, 0);
//             NotReallocated = 1;
//         }
//
//         IsEmpty = (byte)(Length == 1 ? 1 : 0);
//         Unknown = 0;
//     }
//
//     public void Dispose()
//     {
//         if (NotReallocated == 0)
//             Marshal.FreeHGlobal(StringPtr);
//     }
// }
