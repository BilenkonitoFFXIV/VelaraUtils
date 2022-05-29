using System.Collections.Generic;
using Dalamud.Game.ClientState;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Gauge;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Internal.Command;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

[CommandModule("JobGauge", "gauge")]
public unsafe class JobGaugeModule : ICommandModule
{
    private NativePointer<JobGaugeManager> _jobGaugeManager;

    public bool Load(DalamudPluginInterface pluginInterface)
    {
        _jobGaugeManager = JobGaugeManager.Instance();
        return _jobGaugeManager.IsValid;
    }

    public void Unload()
    {
        _jobGaugeManager = null;
    }

    [Command("machinist")]
    [Summary("")]
    [Arguments("")]
    [Aliases("mch")]
    [NoHelpFlag]
    public void MchActiveCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        if (VelaraUtils.Client.LocalPlayer.ClassJob.GameData?.Abbreviation.ToString().ToUpper() != "MCH")
        {
            ChatUtil.ShowPrefixedError("You're currently not playing as Machinist.");
            return;
        }

        MachinistGauge guage = _jobGaugeManager.Value->Machinist;

        List<string> _ = CommandArgumentParser.Parse(argLine, VelaraUtils.VariablesConfiguration.Variables, out string varName);
        if (!string.IsNullOrWhiteSpace(varName))
        {
            if (flags["o"])
            {
                VelaraUtils.VariablesConfiguration.Variables[varName] = guage.OverheatTimeRemaining.ToString();
                return;
            }

            if (flags["s"])
            {
                VelaraUtils.VariablesConfiguration.Variables[varName] = guage.SummonTimeRemaining.ToString();
                return;
            }

            if (flags["h"])
            {
                VelaraUtils.VariablesConfiguration.Variables[varName] = guage.Heat.ToString();
                return;
            }

            if (flags["b"])
            {
                VelaraUtils.VariablesConfiguration.Variables[varName] = guage.Battery.ToString();
                return;
            }

            if (flags["l"])
            {
                VelaraUtils.VariablesConfiguration.Variables[varName] = guage.LastSummonBatteryPower.ToString();
                return;
            }

            if (flags["t"])
            {
                VelaraUtils.VariablesConfiguration.Variables[varName] = guage.TimerActive.ToString();
                return;
            }
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "Machinist Job Gauge:",
            ChatColour.RESET);
        ChatUtil.ShowPair("[o]OverheatTimeRemaining", guage.OverheatTimeRemaining);
        ChatUtil.ShowPair("[s]SummonTimeRemaining", guage.SummonTimeRemaining);
        ChatUtil.ShowPair("[h]Heat", guage.Heat);
        ChatUtil.ShowPair("[b]Battery", guage.Battery);
        ChatUtil.ShowPair("[l]LastSummonBatteryPower", guage.LastSummonBatteryPower);
        ChatUtil.ShowPair("[t]TimerActive", guage.TimerActive);
    }
}
