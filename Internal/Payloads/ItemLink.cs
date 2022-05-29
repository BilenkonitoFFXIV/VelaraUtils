using Dalamud.Game.Text.SeStringHandling.Payloads;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace VelaraUtils.Internal.Payloads;

public class ItemLink
{
    public uint ItemId { get; set; }
    public ItemPayload.ItemKind Kind { get; set; }
    public bool IsHq => Kind == ItemPayload.ItemKind.Hq;
    public string? DisplayNameOverride { get; set; }

    public ItemLink(uint itemId, bool isHq = false, string? displayNameOverride = null)
    {
        ItemId = itemId;
        Kind = isHq ?
            ItemPayload.ItemKind.Hq :
            ItemPayload.ItemKind.Normal;
        DisplayNameOverride = displayNameOverride;
    }

    public ItemLink(uint itemId, ItemPayload.ItemKind kind = ItemPayload.ItemKind.Normal, string? displayNameOverride = null)
    {
        ItemId = itemId;
        Kind = kind;
        DisplayNameOverride = displayNameOverride;
    }

    public ItemPayload ToPayload() => new(ItemId, Kind, DisplayNameOverride);
}
