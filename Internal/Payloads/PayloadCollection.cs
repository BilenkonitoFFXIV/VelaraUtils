using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text.SeStringHandling;
using VelaraUtils.Utils;

namespace VelaraUtils.Internal.Payloads;

public class PayloadCollection : List<Payload>, IList<object>
{
    public bool IsReadOnly => false;

    public PayloadCollection() { }

    public PayloadCollection(IEnumerable<Payload> elements)
        : base(elements) { }

    public PayloadCollection(params Payload[] elements)
        : base(elements) { }

    public PayloadCollection(IEnumerable<object?> elements)
    {
        foreach (object? element in elements)
        {
            switch (element)
            {
                case IEnumerable<Payload> v:
                    base.AddRange(v);
                    break;
                case Payload v:
                    base.Add(v);
                    break;
                case IEnumerable<object?> v:
                    base.AddRange(new PayloadCollection(v));
                    break;
                case (string k, {} v):
                    base.AddRange((k, v).ToPayload());
                    break;
                case (string k, null):
                    base.AddRange((k, (object?)null).ToPayload());
                    break;
                default:
                    base.Add(new GenericPayload(element));
                    break;
            }
        }
    }

    public PayloadCollection(params object?[] elements)
        : this(elements.AsEnumerable()) { }

    public SeString ToSeString() => new(this);

    public void AddRange(IEnumerable<object?> collection) =>
        base.AddRange(new PayloadCollection(collection));

    public void AddRange(params object?[] collection) =>
        AddRange(collection.AsEnumerable());

    public void AddRange(PayloadCollection collection) =>
        base.AddRange(collection);

    IEnumerator<object> IEnumerable<object>.GetEnumerator() =>
        base.GetEnumerator();

    void ICollection<object>.Add(object? item) =>
        base.Add(new GenericPayload(item));

    bool ICollection<object>.Contains(object? item) =>
        base.Contains(new GenericPayload(item));

    void ICollection<object>.CopyTo(object?[] array, int arrayIndex) =>
        base.CopyTo(array.ToPayload().ToArray(), arrayIndex);

    bool ICollection<object>.Remove(object? item) =>
        base.Remove(new GenericPayload(item));

    int IList<object>.IndexOf(object? item) =>
        base.IndexOf(new GenericPayload(item));

    void IList<object>.Insert(int index, object? item) =>
        base.Insert(index, new GenericPayload(item));

    object IList<object>.this[int index]
    {
        get => base[index];
        set => base[index] = new GenericPayload(value);
    }
}
