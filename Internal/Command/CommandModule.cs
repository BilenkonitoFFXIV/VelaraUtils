using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using JetBrains.Annotations;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Internal.Command;

public interface ICommandModule
{
    [UsedImplicitly]
    public bool Load(DalamudPluginInterface pluginInterface)
    {
        return true;
    }

    public void Unload()
    {
    }

    [UsedImplicitly]
    public bool GetHelp(string command, string args, FlagMap flags, ref bool showHelp)
    {
        return false;
    }
}

internal class CommandModule
{
    private readonly List<PluginCommand> _commands;
    private readonly ICommandModule _moduleImpl;
    private bool _loaded;

    [UsedImplicitly]
    public string Name { get; }

    public string Prefix { get; }
    public IEnumerable<PluginCommand> Commands => _commands;

    internal CommandModule(string name, string prefix, ICommandModule moduleImpl)
    {
        Name = name.Trim();
        Prefix = prefix.Trim().TrimStart('/').TrimEnd('.');
        _moduleImpl = moduleImpl;
        _loaded = false;

        _commands =
        (
            from method in _moduleImpl.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            let attr = method.GetCustomAttribute<CommandAttribute>()
            where attr is not null
            select new PluginCommand(
                _moduleImpl,
                Prefix,
                attr.Command,
                method,
                ChatUtil.ShowPrefixedError)).ToList();
    }

    internal void Load(DalamudPluginInterface pluginInterface)
    {
        if (_loaded) return;

        pluginInterface.Inject(this);
        if (!_moduleImpl.Load(pluginInterface))
        {
            ChatUtil.ShowPrefixedError(
                ChatColour.ERROR,
                "Failed to load command module: ",
                ChatColour.RESET,
                ChatColour.CONDITION_FAILED,
                Name,
                ChatColour.RESET);
            return;
        }

        foreach (var cmd in Commands)
        {
            VelaraUtils.CmdManager?.AddHandler(cmd.Command, cmd.MainCommandInfo);
            var hidden = cmd.AliasCommandInfo;
            foreach (var alt in cmd.Aliases)
                VelaraUtils.CmdManager?.AddHandler(alt, hidden);
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.DEBUG,
            "Loaded command module: ",
            ChatColour.RESET,
            ChatColour.CONDITION_FAILED,
            Name,
            ChatColour.RESET);

        _loaded = true;
    }

    internal void Unload()
    {
        if (!_loaded) return;

        _moduleImpl.Unload();
        foreach (var cmd in Commands)
        {
            VelaraUtils.CmdManager?.RemoveHandler(cmd.Command);
            foreach (var alt in cmd.Aliases)
                VelaraUtils.CmdManager?.RemoveHandler(alt);
        }

        ChatUtil.ShowPrefixedMessage(
            ChatColour.DEBUG,
            "Unloaded command module: ",
            ChatColour.RESET,
            ChatColour.CONDITION_FAILED,
            Name,
            ChatColour.RESET);

        _loaded = false;
    }

    internal static readonly PluginCommandDelegate DefaultGetHelp = DefaultGetHelpImpl;

    private static void DefaultGetHelpImpl(string command, string args, FlagMap flags, ref bool showHelp)
    {
        if (args.Length < 1)
        {
            ChatUtil.ShowPrefixedMessage($"{VelaraUtils.PluginName} uses a custom command parser that accepts single-character boolean flags starting with a hyphen.");
            ChatUtil.ShowPrefixedMessage(
                "These flags can be bundled into one argument, such that ",
                ChatColour.HIGHLIGHT,
                "-va",
                ChatColour.RESET,
                " will set both the ",
                ChatColour.HIGHLIGHT,
                "v",
                ChatColour.RESET,
                " and ",
                ChatColour.HIGHLIGHT,
                "a",
                ChatColour.RESET,
                " flags."
            );
            ChatUtil.ShowPrefixedMessage(
                "All plugin commands accept ",
                ChatColour.HIGHLIGHT,
                "-h",
                ChatColour.RESET,
                " to display their built-in help message."
            );
            ChatUtil.ShowPrefixedMessage(
                "To list all commands, use ",
                ChatColour.COMMAND,
                "/vucmds",
                ChatColour.RESET,
                ", optionally with ",
                ChatColour.HIGHLIGHT,
                "-a",
                ChatColour.RESET,
                " to show their aliases and/or ",
                ChatColour.HIGHLIGHT,
                "-v",
                ChatColour.RESET,
                " to show their help messages."
            );
            return;
        }

        foreach (var listing in ArgumentParser.ShellParse(args))
        {
            var wanted = listing.TrimStart('/').ToLower();
            foreach (var cmd in VelaraUtils.CommandManager?.CommandModules.SelectMany(module => module.Commands) ?? Array.Empty<PluginCommand>())
            {
                if (!cmd.CommandComparable.Equals(wanted) && !cmd.AliasesComparable.Contains(wanted)) continue;

                ChatUtil.ShowPrefixedMessage(
                    ChatColour.USAGE_TEXT,
                    cmd.Usage,
                    ChatColour.RESET
                );

                // if (flags["a"] && cmd.Aliases.Count > 0)
                // {
                ChatUtil.ShowPrefixedMessage(
                    ChatColour.QUIET,
                    string.Join(", ", cmd.Aliases),
                    ChatColour.RESET
                );
                // }

                foreach (var line in cmd.HelpLines)
                    ChatUtil.ShowPrefixedMessage(
                        ChatColour.HELP_TEXT,
                        line,
                        ChatColour.RESET
                    );

                return;
            }

            ChatUtil.ShowPrefixedError($"Couldn't find plugin command '/{wanted}'");
        }
    }
}
