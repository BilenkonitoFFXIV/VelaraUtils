using Dalamud.Game.Text.SeStringHandling.Payloads;
using Lumina.Excel.GeneratedSheets;

namespace VelaraUtils.Internal.Payloads;

public class StatusLink
{
    public Status Status { get; set; }
    public uint StatusId => Status.RowId;

    public StatusLink(Status status)
    {
        Status = status;
    }

    public StatusLink(uint statusId)
        : this(VelaraUtils.DataManager!.GetExcelSheet<Status>()!.GetRow(statusId)!) { }

    public StatusPayload ToPayload() =>
        new(StatusId);
}
