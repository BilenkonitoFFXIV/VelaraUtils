using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dalamud.Plugin;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Internal.Command;
using VelaraUtils.Internal.Macro;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

[CommandModule("RuntimeMacro", "rtmacro")]
public class RuntimeMacroModule : ICommandModule
{
    private MacroManager? _macroManager;
    private Dictionary<string, RuntimeMacro> _runtimeMacros = new();

    public bool Load(DalamudPluginInterface pluginInterface)
    {
        _macroManager = VelaraUtils.MacroManager;
        _runtimeMacros.Clear();

        return _macroManager is not null;
    }

    public void Unload()
    {
        _macroManager = null;

        _runtimeMacros.Values.ForEach(o => o.Dispose());
        _runtimeMacros.Clear();
    }

    [Command("list")]
    [Summary("Lists all Runtime Macros")]
    [Arguments("flags")]
    [HelpMessage(
        "Supported flags:",
        "Default:")]
    public void ListCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "Runtime macros:\n",
            ChatColour.RESET,
            _runtimeMacros
                .SelectMany(
                    kv => new object[]
                    {
                        (kv.Key, kv.Value.IsValid ?
                            kv.Value.Name :
                            "null"),
                        "\n"
                    })
                .SkipLast(1),
            ChatColour.RESET
        );
    }

    [Command("new")]
    [Summary("Creates a new Runtime Macro")]
    [Arguments("flags", "macro name")]
    [HelpMessage(
        "Supported flags: (o)verwrite",
        "Default:")]
    public void NewCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        CommandArgumentParser.Parse(argLine, VelaraUtils.VariablesConfiguration.Variables, out string name);
        name = name.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);

        if (string.IsNullOrEmpty(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Invalid runtime macro name", ChatColour.RESET);
            return;
        }

        if (_runtimeMacros.ContainsKey(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Runtime macro already exists", ChatColour.RESET);
            return;
        }

        _runtimeMacros[name] = new RuntimeMacro(name[..20]);
    }

    [Command("print")]
    [Summary("Prints a Runtime Macro")]
    [Arguments("flags", "macro name")]
    [HelpMessage(
        "Supported flags:",
        "Default:")]
    public void PrintCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        CommandArgumentParser.Parse(argLine, VelaraUtils.VariablesConfiguration.Variables, out string name);
        name = name.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);

        if (string.IsNullOrEmpty(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Invalid runtime macro name", ChatColour.RESET);
            return;
        }

        if (!_runtimeMacros.ContainsKey(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Runtime macro does not exist", ChatColour.RESET);
            return;
        }

        RuntimeMacro runtimeMacro = _runtimeMacros[name];
        if (!runtimeMacro.IsValid)
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Runtime macro is invalid", ChatColour.RESET);
            return;
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "Runtime macro ",
            name,
            ":",
            ChatColour.RESET);
        foreach (IReadOnlyList<object> chatLines in runtimeMacro.ToChatMessage())
            ChatUtil.ShowMessage(chatLines);
    }

    [Command("append")]
    [Summary("Appends a line to a Runtime Macro")]
    [Arguments("flags", "macro name", "line")]
    [HelpMessage(
        "Supported flags:",
        "Default:")]
    public void AppendCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        string line = string.Join(' ', CommandArgumentParser.Parse(argLine, VelaraUtils.VariablesConfiguration.Variables, out string name)).Trim();
        // name = name.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);
        // line = line.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);

        if (string.IsNullOrEmpty(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Invalid runtime macro name", ChatColour.RESET);
            return;
        }

        if (!_runtimeMacros.ContainsKey(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Runtime macro does not exist", ChatColour.RESET);
            return;
        }

        RuntimeMacro runtimeMacro = _runtimeMacros[name];
        if (!runtimeMacro.IsValid)
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Runtime macro is invalid", ChatColour.RESET);
            return;
        }

        if (runtimeMacro.LineCount >= 15)
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Runtime macro has maximum amount of lines", ChatColour.RESET);
            return;
        }

        runtimeMacro[runtimeMacro.LineCount] = line;
    }

    [Command("replace")]
    [Summary("Replaces a line of a Runtime Macro")]
    [Arguments("flags", "macro name", "line index", "line")]
    [HelpMessage(
        "Supported flags:",
        "Default:")]
    public void ReplaceCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        string line = string.Join(' ', CommandArgumentParser.Parse(argLine, VelaraUtils.VariablesConfiguration.Variables, out string name, out string indexStr)).Trim();
        // name = name.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);
        // line = line.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);

        if (string.IsNullOrEmpty(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Invalid runtime macro name", ChatColour.RESET);
            return;
        }

        if (!int.TryParse(indexStr, NumberStyles.Integer | NumberStyles.AllowHexSpecifier, null, out int index) || index is < 0 or > 14)
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Invalid line index", ChatColour.RESET);
            return;
        }

        if (!_runtimeMacros.ContainsKey(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Runtime macro does not exist", ChatColour.RESET);
            return;
        }

        RuntimeMacro runtimeMacro = _runtimeMacros[name];
        if (!runtimeMacro.IsValid)
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Runtime macro is invalid", ChatColour.RESET);
            return;
        }

        runtimeMacro[index] = line;
    }

    [Command("exec")]
    [Summary("Executes a Runtime Macro")]
    [Arguments("flags", "macro name")]
    [HelpMessage(
        "Supported flags:",
        "Default:")]
    public void ExecCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        CommandArgumentParser.Parse(argLine, VelaraUtils.VariablesConfiguration.Variables, out string name);
        name = name.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);

        if (string.IsNullOrEmpty(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Invalid runtime macro name", ChatColour.RESET);
            return;
        }

        if (!_runtimeMacros.ContainsKey(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Runtime macro does not exist", ChatColour.RESET);
            return;
        }

        RuntimeMacro runtimeMacro = _runtimeMacros[name];
        if (runtimeMacro.IsValid)
            runtimeMacro.Dispose();

        if (!_runtimeMacros.Remove(name))
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Failed to delete runtime macro", ChatColour.RESET);
    }

    [Command("delete")]
    [Summary("Deletes a Runtime Macro")]
    [Arguments("flags", "macro name")]
    [HelpMessage(
        "Supported flags:",
        "Default:")]
    public void DeleteCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null)
        {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        CommandArgumentParser.Parse(argLine, VelaraUtils.VariablesConfiguration.Variables, out string name);
        name = name.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);

        if (string.IsNullOrEmpty(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Invalid runtime macro name", ChatColour.RESET);
            return;
        }

        if (!_runtimeMacros.ContainsKey(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Runtime macro does not exist", ChatColour.RESET);
            return;
        }

        if (_runtimeMacros.Remove(name, out RuntimeMacro? runtimeMacro))
            runtimeMacro.Dispose();
        else
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Failed to delete runtime macro", ChatColour.RESET);
    }
}
