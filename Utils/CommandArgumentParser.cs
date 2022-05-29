using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Dalamud.Game.ClientState.Conditions;
using VelaraUtils.Chat;
using VelaraUtils.Internal.IPC;

namespace VelaraUtils.Utils;

internal static class CommandArgumentParser
{
    private class Parser : DynamicObject
    {
        private static string[] SplitSource(char separator, string source) =>
            source.Length switch
            {
                < 1 => Array.Empty<string>(),
                > 0 => source.Split(separator)
            };

        private static string[] SplitSource(string source) =>
            SplitSource(' ', source);

        public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
        {
            result = null;
            if (args is null || args.Length < 1)
                return false;

            int restBeginIndex = 0;

            string[] source;
            switch (args[restBeginIndex])
            {
                case char c when args.Length > 1 && args[1] is string s:
                    source = SplitSource(c, s);
                    restBeginIndex++;
                    break;
                case string s:
                    source = SplitSource(s);
                    break;
                default:
                    return false;
            }
            restBeginIndex++;

            IDictionary<string, string>? tokenMap = null;
            if (args.Length > restBeginIndex && args[restBeginIndex] is IDictionary<string, string> d)
            {
                tokenMap = d;
                restBeginIndex++;
            }

            Regex? pattern = null;
            if (args.Length > restBeginIndex && args[restBeginIndex] is Regex r)
            {
                pattern = r;
                restBeginIndex++;
            }

            List<string> rest = new();
            for (int i = 0; i < source.Length; i++)
            {
                string arg = ExpandTokens(source[i].Trim(), tokenMap, pattern).Trim();

                if (restBeginIndex + i < args.Length)
                    args[restBeginIndex + i] = arg;
                else
                    rest.Add(arg);
            }
            result = rest;
            return true;
        }
    }
    public static readonly dynamic Parse = new Parser();

    // private static readonly Regex VarCommandPattern = new(@"(?<!%)%(\w+)%(?!%)|(?<!\$)\$\{(?:(?:VAR\.)|(?!\w+\.))(\w+)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly List<Regex> VarCommandPattern = new()
    {
        new Regex(@"(?<!%)%(\w+)%(?!%)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(?<!\\)\$\{(\w+)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled)
    };

    // private static readonly Regex FlagCommandPattern = new(@"(?<!\$)\$(\w+)\$(?!\$)|(?<!\$)\$\{FLAG\.(\w+)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly List<Regex> FlagCommandPattern = new()
    {
        new Regex(@"(?<!\$)\$(\w+)\$(?!\$)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(?<!\\)\$CF\{(\w+)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled)
    };

    // private static readonly Regex QolBarCommandPattern = new(@"(?<!\¤)\¤(\w+)\¤(?!\¤)|(?<!\$)\$\{QB\.(\w+)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly List<Regex> QolBarNameCommandPattern = new()
    {
        new Regex(@"(?<!\¤)\¤((?:\w|[\\][x]\d+)+)\¤(?!\¤)", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new Regex(@"(?<!\\)\$QB\{((?:\w|[\\][x]\d+)+)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled)
    };
    private static readonly List<Regex> QolBarIndexCommandPattern = new()
    {
        new Regex(@"(?<!\\)\$QB\#\{(\d+)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled)
    };
    private static readonly char[] ReservedChars = { '%' };

    // private static string UnescapeTokens(this string source)
    // {
    //     return ReservedChars.Aggregate(source, (current, reservedChar) => current.Replace(string.Format("{0}{0}", reservedChar), reservedChar.ToString()));
    // }

    public static string ExpandTokens(this string source, IDictionary<string, string>? tokenMap = null, Regex? pattern = null)
    {
        if (string.IsNullOrEmpty(source))
            return source;
        tokenMap ??= VelaraUtils.VariablesConfiguration.Variables;

        string result = source;
        if (pattern is not null)
        {
            result = pattern.Replace(result, m =>
                m.Groups.Count != 2 ?
                    m.Value :
                    tokenMap.TryGetValue(m.Groups[1].Value, out string? value) ?
                        value :
                        string.Empty).Trim(); //.UnescapeTokens();
        }
        else
        {
            foreach (Regex regex in VarCommandPattern)
            {
                result = regex.Replace(result, m =>
                    m.Groups.Count != 2 ?
                        m.Value :
                        tokenMap.TryGetValue(m.Groups[1].Value, out string? value) ?
                            value :
                            string.Empty).Trim(); //.UnescapeTokens();
            }

            if (VelaraUtils.Conditions is not null)
            {
                foreach (Regex regex in FlagCommandPattern)
                {
                    result = regex.Replace(result, m =>
                        m.Groups.Count != 2 ?
                            m.Value :
                            Enum.TryParse(m.Groups[1].Value, true, out ConditionFlag flag) ?
                                VelaraUtils.Conditions[flag] ?
                                    "true" :
                                    "false" :
                                m.Value).Trim(); //.UnescapeTokens();
                }
            }

            if (VelaraUtils.QolBar?.Enabled ?? false)
            {
                foreach (Regex regex in QolBarIndexCommandPattern)
                {
                    result = regex.Replace(result, m =>
                    {
                        if (m.Groups.Count != 2)
                            return m.Value;
                        if (!int.TryParse(m.Groups[1].Value, NumberStyles.Integer, null, out int index))
                            return m.Value;
                        QolBar.IConditionSet? conditionSet = VelaraUtils.QolBar[Math.Max(0, index - 1)];
                        // if (conditionSet is not null) ChatUtil.ShowMessage(conditionSet.Name);
                        return conditionSet is null ?
                            "null" :
                            conditionSet.State ?
                                "true" :
                                "false";
                    }).Trim();
                    // result = regex.Replace(result, m =>
                    //     m.Groups.Count != 2 ?
                    //         m.Value :
                    //         int.TryParse(m.Groups[1].Value, NumberStyles.Integer, null, out int index) ?
                    //             VelaraUtils.QolBar[index + 1]?.State ?? false ?
                    //                 "true" :
                    //                 "false" :
                    //             m.Value).Trim();
                        //.UnescapeTokens();
                }

                foreach (Regex regex in QolBarNameCommandPattern)
                {
                    result = regex.Replace(result, m =>
                    {
                        if (m.Groups.Count != 2)
                            return m.Value;
                        QolBar.IConditionSet? conditionSet = VelaraUtils.QolBar[m.Groups[1].Value.Replace("\\x20", " ")];
                        // if (conditionSet is not null) ChatUtil.ShowMessage(conditionSet.Name);
                        return conditionSet is null ?
                            "null" :
                            conditionSet.State ?
                                "true" :
                                "false";
                    }).Trim();
                        // m.Groups.Count != 2 ?
                        //     m.Value :
                        //     VelaraUtils.QolBar[m.Groups[1].Value.Trim()]?.State ?
                        //         "true" :
                        //         "false"
                        //.UnescapeTokens();
                }
            }
        }

        return result;
    }

    public static string ExpandTokens(this string source, Regex pattern) =>
        source.ExpandTokens(null, pattern);
}
