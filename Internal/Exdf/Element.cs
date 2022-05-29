// using System;
// using System.Collections.Generic;
//
// namespace VelaraUtils.Internal.Exdf;
//
// public interface IElement<TKey> : IComparable<IElement<TKey>>, IComparable<TKey>, IEquatable<IElement<TKey>>, IEquatable<TKey>
//     where TKey : struct, IComparable<TKey>, IEquatable<TKey>
// {
//     protected class ElementComparer : IComparer<IElement<TKey>>
//     {
//         public int Compare(IElement<TKey>? x, IElement<TKey>? y)
//         {
//             if (ReferenceEquals(x, y)) return 0;
//             if (ReferenceEquals(null, y)) return 1;
//             if (ReferenceEquals(null, x)) return -1;
//             return x.Id.CompareTo(y.Id);
//         }
//
//         public int Compare(TKey x, IElement<TKey>? y) => x.CompareTo(y?.Id ?? default(TKey));
//         public int Compare(IElement<TKey>? x, TKey y) => Compare(y, x) * -1;
//     }
//
//     protected delegate IElement<TKey> ParserDelegate(TKey id, params object[] args);
//
//     public TKey Id { get; protected init; }
//
//     public static abstract implicit operator TKey(IElement<TKey> o) => o.Id;
// }
