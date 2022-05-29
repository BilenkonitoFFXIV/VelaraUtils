using System.Numerics;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Lumina.Excel.GeneratedSheets;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace VelaraUtils.Internal.Payloads;

public class MapLink
{
    private static readonly Vector2 Unk1 = new(1024f);

    public TerritoryType TerritoryType { get; set; }
    public uint TerritoryTypeId => TerritoryType.RowId;
    public Map Map { get; set; }
    public uint MapId => Map.RowId;
    public Vector2 RawCoordinates { get; set; }

    public MapLink(TerritoryType territoryType, Map map, Vector2 rawCoordinates)
    {
        TerritoryType = territoryType;
        Map = map;
        RawCoordinates = rawCoordinates;
    }

    public MapLink(TerritoryType territoryType, Map map, Vector2 niceCoordinates, float fudgeFactor = 0.05f)
    {
        TerritoryType = territoryType;
        Map = map;
        RawCoordinates = ConvertMapCoordinateToRawPosition(niceCoordinates + new Vector2(fudgeFactor), Map.SizeFactor);
    }

    public MapLink(TerritoryType territoryType, Map map, float rawX, float rawY)
        : this(territoryType, map, new Vector2(rawX, rawY)) { }

    public MapLink(TerritoryType territoryType, Map map, float niceX, float niceY, float fudgeFactor = 0.05f)
        : this(territoryType, map, new Vector2(niceX, niceY), fudgeFactor) { }

    public MapLink(uint territoryTypeId, uint mapId, Vector2 rawCoordinates)
        : this(
            VelaraUtils.DataManager!.GetExcelSheet<TerritoryType>()!.GetRow(territoryTypeId)!,
            VelaraUtils.DataManager.GetExcelSheet<Map>()!.GetRow(mapId)!,
            rawCoordinates
        ) { }

    public MapLink(uint territoryTypeId, uint mapId, Vector2 niceCoordinates, float fudgeFactor = 0.05f)
        : this(
            VelaraUtils.DataManager!.GetExcelSheet<TerritoryType>()!.GetRow(territoryTypeId)!,
            VelaraUtils.DataManager.GetExcelSheet<Map>()!.GetRow(mapId)!,
            niceCoordinates,
            fudgeFactor
        ) { }

    public MapLink(uint territoryTypeId, uint mapId, float rawX, float rawY)
        : this(territoryTypeId, mapId, new Vector2(rawX, rawY)) { }

    public MapLink(uint territoryTypeId, uint mapId, float niceX, float niceY, float fudgeFactor = 0.05f)
        : this(territoryTypeId, mapId, new Vector2(niceX, niceY), fudgeFactor) { }

    public MapLinkPayload ToPayload() => new(TerritoryTypeId, MapId, RawCoordinates.X, RawCoordinates.Y);

    private static Vector2 ConvertMapCoordinateToRawPosition(Vector2 coordinates, float scale)
    {
        scale /= 100f;
        return ((coordinates - Vector2.One) * scale / 41f * 2048f - Unk1) / scale * 1000f;
    }
}
