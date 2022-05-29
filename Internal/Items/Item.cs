// using System;
// using System.Collections.Generic;
// using System.Linq;
//
// namespace VelaraUtils.Internal.Items;
//
// public interface IItem : IComparable<IItem>, IComparable<uint>, IEquatable<IItem>, IEquatable<uint>
// {
//     private class ItemComparer : IComparer<IItem>
//     {
//         public int Compare(IItem? x, IItem? y)
//         {
//             if (ReferenceEquals(x, y)) return 0;
//             if (ReferenceEquals(null, y)) return 1;
//             if (ReferenceEquals(null, x)) return -1;
//             return x.Id.CompareTo(y.Id);
//         }
//
//         public int Compare(uint x, IItem? y) => x.CompareTo(y?.Id);
//         public int Compare(IItem? x, uint y) => Compare(y, x) * -1;
//     }
//
//     private static readonly ItemComparer Comparer = new();
//     private static readonly SortedSet<IItem> Cache = new(Comparer);
//
//     public uint Id { get; protected init; }
//
//     int IComparable<IItem>.CompareTo(IItem? other) => Comparer.Compare(this, other);
//     int IComparable<uint>.CompareTo(uint other) => Comparer.Compare(this, other);
//
//     bool IEquatable<IItem>.Equals(IItem? other) => CompareTo(other) == 0;
//     bool IEquatable<uint>.Equals(uint other) => CompareTo(other) == 0;
//
//     public static T Get<T>(uint id) where T : IItem, new()
//     {
//         if (Cache.FirstOrDefault(item => item.Equals()))
//     }
// }
