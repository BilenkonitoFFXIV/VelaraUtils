using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace VelaraUtils.Internal;

public class TerritoryManager
{
    public class TerritoryDetail
    {
        public string Name { get; init; } = null!;
        public uint TerritoryType { get; init; }
        public uint MapId { get; init; }
        public ushort SizeFactor { get; init; }
    }

    private List<TerritoryDetail> _territoryDetails;

    public TerritoryManager()
    {
        _territoryDetails = new List<TerritoryDetail>();
    }

    public TerritoryDetail? GetByZoneName(string zone, bool matchPartial = true)
    {
        if (!_territoryDetails.Any())
            LoadTerritoryDetails();

        return _territoryDetails
            .Where(x =>
                x.Name.Equals(zone, StringComparison.OrdinalIgnoreCase) ||
                matchPartial && x.Name.ToLower().Contains(zone.ToLower()))
            .OrderBy(x => x.Name.Length)
            .FirstOrDefault();
    }

    public TerritoryDetail? GetByTerritoryType(ushort territoryType)
    {
        if (!_territoryDetails.Any())
            LoadTerritoryDetails();

        return _territoryDetails.FirstOrDefault(x => x.TerritoryType == territoryType);
    }

    private void LoadTerritoryDetails()
    {
        ExcelSheet<TerritoryType>? sheet = VelaraUtils.DataManager?.GetExcelSheet<TerritoryType>();
        if (sheet is null)
            return;

        _territoryDetails = (from territoryType in sheet
            let type = territoryType.Bg.RawString.Split('/')
            where type.Length >= 3
            where type[2] == "twn" || type[2] == "fld" || type[2] == "hou"
            where !string.IsNullOrWhiteSpace(territoryType.Map.Value?.PlaceName.Value?.Name ?? " ")
            select new TerritoryDetail
            {
                TerritoryType = territoryType.RowId,
                MapId = territoryType.Map.Value?.RowId ?? 0,
                SizeFactor = territoryType.Map.Value?.SizeFactor ?? 0,
                Name = territoryType.Map.Value?.PlaceName.Value?.Name ?? ""
            }).ToList();
    }
}
