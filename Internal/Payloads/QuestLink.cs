using Dalamud.Game.Text.SeStringHandling.Payloads;
using Lumina.Excel.GeneratedSheets;

namespace VelaraUtils.Internal.Payloads;

public class QuestLink
{
    public Quest Quest { get; set; }
    public uint QuestId => Quest.RowId;

    public QuestLink(Quest quest)
    {
        Quest = quest;
    }

    public QuestLink(uint questId)
        : this(VelaraUtils.DataManager!.GetExcelSheet<Quest>()!.GetRow(questId)!) { }

    public QuestPayload ToPayload() =>
        new(QuestId);
}
