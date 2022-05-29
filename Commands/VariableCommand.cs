using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dalamud.Game.ClientState.Conditions;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Commands;

public partial class UtilsModule
{
    [Command("/setvar")]
    [Arguments("variable name", "variable value")]
    [Summary("")]
    [HelpMessage("")]
    public static void SetVarCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        string value = string.Join(' ', CommandArgumentParser.Parse(argLine, out string name)).Trim();
        name = name.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);
        value = value.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables).Trim();

        if (string.IsNullOrEmpty(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Invalid variable name", ChatColour.RESET);
            return;
        }

        if (string.IsNullOrEmpty(value))
        {
            if (VelaraUtils.VariablesConfiguration.Variables.ContainsKey(name))
            {
                VelaraUtils.VariablesConfiguration.Variables.Remove(name);
            }
            else
            {
                ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, $"Variable '{name}' name is not set", ChatColour.RESET);
            }

            return;
        }

        VelaraUtils.VariablesConfiguration.Variables[name] = value;
    }

    [Command("/printvars")]
    [Arguments("flags")]
    [Summary("")]
    [Aliases("/getvars")]
    [HelpMessage("Supported flags: (n)o vars, condition (f)lags, (q)olbar")]
    public static void PrintVarsCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        if (!flags["n"])
            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "Variables:\n",
                ChatColour.RESET,
                VelaraUtils.VariablesConfiguration.Variables
                    .SelectMany(kv => new object[]{(kv.Key, kv.Value), "\n"})
                    .SkipLast(1),
                ChatColour.RESET
            );

        if (flags["f"])
            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "Variables:\n",
                ChatColour.RESET,
                Enum.GetValues<ConditionFlag>()
                    .SelectMany(k => new object[]{(Enum.GetName(k)?.ToString() ?? "null", VelaraUtils.Conditions?[k]), "\n"}),
                ChatColour.RESET
            );

        if (flags["q"])
            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                "Variables:\n",
                ChatColour.RESET,
                VelaraUtils.QolBar?.ConditionSets
                    .SelectMany(cSet => new object[]{(cSet.Name, cSet.State), "\n"})
                    .SkipLast(1),
                ChatColour.RESET
            );
    }

    [Command("/getvar")]
    [Arguments("variable name")]
    [Summary("")]
    [HelpMessage("")]
    public static void GetVarCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        CommandArgumentParser.Parse(argLine, out string name);
        name = name.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);

        if (string.IsNullOrEmpty(name))
        {
            bool supress = false;
            PrintVarsCommand(string.Empty, string.Empty, new FlagMap(), ref supress);
            return;
        }

        ChatUtil.ShowPrefixedPair(
            name,
            VelaraUtils.VariablesConfiguration.Variables.ContainsKey(name) ?
                VelaraUtils.VariablesConfiguration.Variables[name] :
                null);
    }

    [Command("/clearvars")]
    [Arguments]
    [Summary("")]
    [Aliases("/cleanvars")]
    [HelpMessage("")]
    public static void ClearVarsCommand(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        int count = VelaraUtils.VariablesConfiguration.Variables.Count;
        VelaraUtils.VariablesConfiguration.Variables.Clear();

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "Cleared ",
            ChatColour.RESET,
            count > 0 ?
                ChatColour.TRUE :
                ChatColour.FALSE,
            count.ToString(),
            ChatColour.RESET,
            ChatColour.WHITE,
            " variables.",
            ChatColour.RESET
        );
    }

    [Command("/ifvar")]
    [Arguments("flags", "variable name", "variable value", "commands to run on true", "!commands to run on false?")]
    [Summary("")]
    [HelpMessage("")]
    public static void RunIfVar(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        List<string> cmd = CommandArgumentParser.Parse(argLine, out string name, out string value);
        name = name.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);
        value = value.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);

        if (string.IsNullOrEmpty(value))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Too few arguments", ChatColour.RESET);
            return;
        }

        if (string.IsNullOrEmpty(name))
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Invalid variable name", ChatColour.RESET);
            return;
        }

        bool match = (VelaraUtils.VariablesConfiguration.Variables.ContainsKey(name) &&
                      (value.Trim() == "*" ||
                       VelaraUtils.VariablesConfiguration.Variables[name].Equals(value, flags["i"] ?
                           StringComparison.InvariantCultureIgnoreCase :
                           StringComparison.InvariantCulture)))
                     ^ flags["n"];
        if (cmd.Count < 1)
        {
            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                $"{name}: ",
                ChatColour.RESET,
                match ?
                    ChatColour.FALSE :
                    ChatColour.TRUE,
                value,
                ChatColour.RESET,
                ChatColour.WHITE,
                ".",
                ChatColour.RESET
            );
            return;
        }

        string cmdString = string.Join(" ", cmd);
        string[] cmdCases = cmdString.Split('!', 2,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (!cmdString.Contains('!'))
        {
            if (!match) return;
        }
        else
            cmdString = cmdCases[match ? 0 : 1];

        cmdString = cmdString.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);
        if (cmdString.Length > 0)
            ChatUtil.SendChatLineToServer(cmdString);
    }

    [Command("/do")]
    [Arguments("command to run...?")]
    [Summary("")]
    [HelpMessage("")]
    public static void RunDo(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        // List<string> cmd = CommandArgumentParser.Parse(argLine);
        // if (cmd.Count < 1) return;

        // ChatUtil.SendChatLineToServer(string.Join(" ", cmd));

        string cmd = argLine.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);
        if (cmd.Length < 1)
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Too few arguments", ChatColour.RESET);
            return;
        }
        ChatUtil.SendChatLineToServer(cmd);
    }

    private static readonly Regex RunIfEquCmdTruePattern = new(@"(?<!\\)\@", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex RunIfEquCmdFalsePattern = new(@"(?<!\\)\!", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    [Command("/ifgt")]
    [Arguments("flags", "value 1", "value 2", "@", "commands to run on true", "!commands to run on false?")]
    [Summary("")]
    [HelpMessage("")]
    public static void RunIfGt(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        string[] argArr = RunIfEquCmdTruePattern.Split(argLine, 2);
        if (argArr.Length < 1)
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Too few arguments", ChatColour.RESET);
            return;
        }

        string[] valueArr = argArr[0].Trim().Split(' ', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (valueArr.Length < 2)
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Too few arguments", ChatColour.RESET);
            return;
        }

        string value1 = valueArr[0].ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);
        string value2 = valueArr[1].ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);

        string vParsed1;
        string vParsed2;

        bool match;
        if (flags["f"])
        {
            float val1 =
                (flags["s"] || value1.ToLowerInvariant() is not "" and not "null" and not "false") &&
                float.TryParse(value1, out float v1) ?
                    flags["u"] ?
                        MathF.Max(0, v1) :
                        v1 :
                    0;

            float val2 =
                (flags["s"] || value2.ToLowerInvariant() is not "" and not "null" and not "false") &&
                float.TryParse(value2, out float v2) ?
                    flags["u"] ?
                        MathF.Max(0, v2) :
                        v2 :
                    0;

            match = val1 > val2;
            vParsed1 = val1.ToString("F3");
            vParsed2 = val2.ToString("F3");
        }
        else
        {
            int val1 =
                (flags["s"] || value1.ToLowerInvariant() is not "" and not "null" and not "false") &&
                int.TryParse(value1, out int v1) ?
                    flags["u"] ?
                        Math.Max(0, v1) :
                        v1 :
                    0;

            int val2 =
                (flags["s"] || value2.ToLowerInvariant() is not "" and not "null" and not "false") &&
                int.TryParse(value2, out int v2) ?
                    flags["u"] ?
                        Math.Max(0, v2) :
                        v2 :
                    0;

            match = val1 > val2;
            vParsed1 = val1.ToString("D");
            vParsed2 = val2.ToString("D");
        }
        match ^= flags["n"];

        if (argArr.Length < 2)
        {
            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                value1,
                value1 != vParsed1 ?
                    $"({vParsed1}) " :
                    " ",
                ChatColour.RESET,
                match ?
                    ChatColour.TRUE :
                    ChatColour.FALSE,
                match ?
                    ">" :
                    "<=",
                ChatColour.RESET,
                ChatColour.WHITE,
                $" {value2}",
                value2 != vParsed2 ?
                    $"({vParsed2})." :
                    ".",
                ChatColour.RESET
            );
            return;
        }

        string[] cmdArr = RunIfEquCmdFalsePattern.Split(argArr[1].Trim(), 2);
        string cmd = (match ?
            cmdArr[0] :
            cmdArr.Length > 1 ?
                cmdArr[1] :
                string.Empty).Trim();
        if (cmd.Length < 1) return;

        cmd = cmd.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);
        if (cmd.Length < 1) return;

        ChatUtil.SendChatLineToServer(cmd);
    }

    [Command("/ifequ")]
    [Arguments("flags", "value 1", "value 2", "@","commands to run on true", "!commands to run on false?")]
    [Summary("")]
    [HelpMessage("")]
    public static void RunIfEqu(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        string[] argArr = RunIfEquCmdTruePattern.Split(argLine, 2);
        // for (int i = 0; i < argArr.Length; i++)
        //     ChatUtil.ShowPrefixedMessage(ChatColour.WHITE, $"argArr[{i.ToString()}]: {argArr[i]}", ChatColour.RESET);
        if (argArr.Length < 1)
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Too few arguments", ChatColour.RESET);
            return;
        }

        string[] valueArr = argArr[0].Trim().Split(' ', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        // for (int i = 0; i < valueArr.Length; i++)
        //     ChatUtil.ShowPrefixedMessage(ChatColour.WHITE, $"valueArr[{i.ToString()}]: {valueArr[i]}", ChatColour.RESET);
        if (valueArr.Length < 2)
        {
            ChatUtil.ShowPrefixedError(ChatColour.CONDITION_FAILED, "Too few arguments", ChatColour.RESET);
            return;
        }

        string value1 = valueArr[0].ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);
        string value2 = valueArr[1].ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);

        string val1 =
            !flags["s"] && value1.ToLowerInvariant() is "" or "null" or "false" or "0" ?
                "false" :
                flags["i"] ?
                    value1.ToLowerInvariant() :
                    value1;
        string val2 =
            !flags["s"] && value2.ToLowerInvariant() is "" or "null" or "false" or "0" ?
                "false" :
                flags["i"] ?
                    value2.ToLowerInvariant() :
                    value2;
        bool match = string.Equals(val1, val2, StringComparison.InvariantCulture) ^ flags["n"];

        if (argArr.Length < 2)
        {
            ChatUtil.ShowPrefixedMessage(
                ChatColour.WHITE,
                $"{value1} ",
                ChatColour.RESET,
                match ?
                    ChatColour.TRUE :
                    ChatColour.FALSE,
                match ?
                    "==" :
                    "!=",
                ChatColour.RESET,
                ChatColour.WHITE,
                $" {value2}.",
                ChatColour.RESET
            );
            return;
        }

        string[] cmdArr = RunIfEquCmdFalsePattern.Split(argArr[1].Trim(), 2);
        // for (int i = 0; i < cmdArr.Length; i++)
        //     ChatUtil.ShowPrefixedMessage(ChatColour.WHITE, $"cmdArr[{i.ToString()}]: {cmdArr[i]}", ChatColour.RESET);
        string cmd = (match ?
            cmdArr[0] :
            cmdArr.Length > 1 ?
                cmdArr[1] :
                string.Empty).Trim();
        // ChatUtil.ShowPrefixedMessage(ChatColour.WHITE, $"cmd: {cmd}", ChatColour.RESET);
        if (cmd.Length < 1) return;

        cmd = cmd.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);
        if (cmd.Length < 1) return;

        ChatUtil.SendChatLineToServer(cmd);
    }

    [Command("/testexpr")]
    [Arguments("expression")]
    [Summary("")]
    [HelpMessage("")]
    public static void RunTestExpression(string command, string argLine, FlagMap flags, ref bool showHelp)
    {
        if (VelaraUtils.Client?.LocalPlayer is null) {
            ChatUtil.ShowPrefixedError("Can't find player object - this should be impossible unless you're not logged in.");
            return;
        }

        string expr = argLine.ExpandTokens(VelaraUtils.VariablesConfiguration.Variables);
        if (string.IsNullOrWhiteSpace(expr)) expr = "null";

        ChatUtil.ShowPrefixedMessage(
            ChatColour.WHITE,
            "Expression test result",
            ChatColour.RESET);

        ChatUtil.ShowMessage(
            ChatColour.PREFIX,
            "Source: ",
            ChatColour.RESET,
            ChatColour.OUTGOING_TEXT,
            argLine,
            ChatColour.RESET);

        ChatUtil.ShowMessage(
            ChatColour.PREFIX,
            "Result: ",
            ChatColour.RESET,
            ChatColour.OUTGOING_TEXT,
            expr,
            ChatColour.RESET);
    }
}
