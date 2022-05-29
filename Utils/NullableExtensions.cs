using System;
using JetBrains.Annotations;

namespace VelaraUtils.Utils;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class NullableExtensions
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private class NullableWrapper<T> where T : struct
    {
        private readonly T? _value;

        private NullableWrapper(T? value) => _value = value;

        public T Default(T fallback = default) => _value ?? fallback;

        public TResult Ensure<TResult>(Func<T, TResult> predicate, TResult fallback = default) where TResult : struct =>
            _value.HasValue ?
                predicate(_value.Value) :
                fallback;

        public static implicit operator NullableWrapper<T>(T? other) => new(other);
        public static implicit operator T(NullableWrapper<T> other) => other._value ?? default;
    }

    private static NullableWrapper<T> Wrap<T>(this T? source) where T : struct =>
        source;

    public static T Default<T>(this T? source, T fallback = default) where T : struct =>
        source
            .Wrap()
            .Default(fallback);

    public static T? Default<T>(this T? source, T? fallback = null) where T : class =>
        source ?? fallback;

    public static TResult Ensure<T, TResult>(this T? source, Func<T, TResult> predicate, TResult fallback)
        where T : struct
        where TResult : struct =>
        source
            .Wrap()
            .Ensure(predicate, fallback);

    public static bool Value(this bool? source) =>
        source ?? false;
}
