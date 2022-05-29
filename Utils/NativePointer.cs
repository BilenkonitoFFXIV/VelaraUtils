using System;
using JetBrains.Annotations;

namespace VelaraUtils.Utils;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public readonly unsafe struct NativePointer<T> where T : unmanaged
{
    private readonly IntPtr _ptr;
    public T* Value => (T*)_ptr;
    public bool IsValid => _ptr != IntPtr.Zero;

    private NativePointer(T* ptr)
    {
        // if ((IntPtr)ptr == IntPtr.Zero) throw new ArgumentNullException(nameof(ptr));
        _ptr = (IntPtr)ptr;
    }

    public T2* Cast<T2>()
        where T2 : unmanaged => (T2*)_ptr;

    public static implicit operator T*(NativePointer<T> obj) => obj.Value;
    public static implicit operator NativePointer<T>(T* obj) => new(obj);

    public T this[short i] => Value[i];
    public T this[ushort i] => Value[i];
    public T this[int i] => Value[i];
    public T this[uint i] => Value[i];
    public T this[long i] => Value[i];
    public T this[ulong i] => Value[i];
    public T this[byte i] => Value[i];
    public T this[nint i] => Value[i];
    public T this[nuint i] => Value[i];

    public static T* operator +(NativePointer<T> obj, short other) => obj.Value + other;
    public static T* operator +(NativePointer<T> obj, ushort other) => obj.Value + other;
    public static T* operator +(NativePointer<T> obj, int other) => obj.Value + other;
    public static T* operator +(NativePointer<T> obj, uint other) => obj.Value + other;
    public static T* operator +(NativePointer<T> obj, long other) => obj.Value + other;
    public static T* operator +(NativePointer<T> obj, ulong other) => obj.Value + other;
    public static T* operator +(NativePointer<T> obj, byte other) => obj.Value + other;
    public static T* operator +(NativePointer<T> obj, nint other) => obj.Value + other;
    public static T* operator +(NativePointer<T> obj, nuint other) => obj.Value + other;

    public static T* operator -(NativePointer<T> obj, short other) => obj.Value - other;
    public static T* operator -(NativePointer<T> obj, ushort other) => obj.Value - other;
    public static T* operator -(NativePointer<T> obj, int other) => obj.Value - other;
    public static T* operator -(NativePointer<T> obj, uint other) => obj.Value - other;
    public static T* operator -(NativePointer<T> obj, long other) => obj.Value - other;
    public static T* operator -(NativePointer<T> obj, ulong other) => obj.Value - other;
    public static T* operator -(NativePointer<T> obj, byte other) => obj.Value - other;
    public static T* operator -(NativePointer<T> obj, nint other) => obj.Value - other;
    public static T* operator -(NativePointer<T> obj, nuint other) => obj.Value - other;

    public static T* operator *(NativePointer<T> obj, short other) => obj.Value + other * sizeof(T);
    public static T* operator *(NativePointer<T> obj, ushort other) => obj.Value + other * sizeof(T);
    public static T* operator *(NativePointer<T> obj, int other) => obj.Value + other * sizeof(T);
    public static T* operator *(NativePointer<T> obj, uint other) => obj.Value + other * sizeof(T);
    public static T* operator *(NativePointer<T> obj, long other) => obj.Value + other * sizeof(T);
    public static T* operator *(NativePointer<T> obj, byte other) => obj.Value + other * sizeof(T);
    public static T* operator *(NativePointer<T> obj, nint other) => obj.Value + other * sizeof(T);
}
