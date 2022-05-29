using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Game.Command;
using Dalamud.Logging;
using JetBrains.Annotations;
using VelaraUtils.Attributes;
using VelaraUtils.Chat;
using VelaraUtils.Utils;

namespace VelaraUtils.Internal.Command;

internal delegate void PluginCommandDelegate(string command, string rawArguments, FlagMap flags, ref bool showHelp);

internal delegate void PluginCommandInvocationErrorHandlerDelegate(params object[] payloads);

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal class PluginCommand
{
    private readonly ICommandModule _module;
    private readonly PluginCommandDelegate _handler;
    private readonly PluginCommandInvocationErrorHandlerDelegate _error;

    private readonly string _prefix;
    private readonly string _commandBase;
    private readonly string[] _aliasesBase;

    public CommandInfo MainCommandInfo => new(Dispatch)
    {
        HelpMessage = Summary,
        ShowInHelp = ShowInDalamud,
    };

    public CommandInfo AliasCommandInfo => new(Dispatch)
    {
        HelpMessage = Summary,
        ShowInHelp = false,
    };

    public string CommandComparable => Command.TrimStart('/').ToLower();
    public IEnumerable<string> AliasesComparable => Aliases.Select(s => s.TrimStart('/').ToLower()).ToArray();
    public IEnumerable<string> HelpLines => Help.Split('\r', '\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
    public string Command =>
        string.IsNullOrWhiteSpace(_prefix) ?
            $"/{_commandBase}" :
            $"/{_prefix}.{_commandBase}";

    public string ArgumentDescription { get; }
    public string Summary { get; }
    public string Help { get; }
    public bool UseHelpFlag { get; }
    public string Usage => $"{Command} {ArgumentDescription}".Trim();
    public IEnumerable<string> Aliases =>
        (string.IsNullOrWhiteSpace(_prefix) ?
            _aliasesBase.Select(alias => $"/{alias}") :
            _aliasesBase.Select(alias => $"/{_prefix}.{alias}"))
        .ToList();
    public bool ShowInDalamud { get; }
    public bool ShowInListing { get; }

    public PluginCommand(ICommandModule module, string prefix, string command, MethodInfo method, PluginCommandInvocationErrorHandlerDelegate onError)
    {
        if (string.IsNullOrEmpty(command))
        {
            throw new ArgumentException("Invalid command name", nameof(command));
        }

        _module = module;
        _prefix = prefix;
        _commandBase = command.TrimStart('/');
        _aliasesBase = method.GetCustomAttribute<AliasesAttribute>()?.Aliases ?? Array.Empty<string>();
        _handler = (PluginCommandDelegate)Delegate.CreateDelegate(typeof(PluginCommandDelegate), method.IsStatic ? null : _module, method);
        _error = onError;

        Summary = method.GetCustomAttribute<SummaryAttribute>()?.Summary ?? string.Empty;
        ArgumentDescription = method.GetCustomAttribute<ArgumentsAttribute>()?.ArgumentDescription.Trim() ?? string.Empty;
        Help = method.GetCustomAttribute<HelpMessageAttribute>()?.HelpMessage ?? string.Empty;
        UseHelpFlag = method.GetCustomAttribute<NoHelpFlagAttribute>() is null;
        ShowInDalamud = method.GetCustomAttribute<DoNotShowInHelpAttribute>() is null || string.IsNullOrEmpty(Summary);
        ShowInListing = method.GetCustomAttribute<HideInCommandListingAttribute>() is null;
    }

    public void Dispatch(string command, string argline)
    {
        try
        {
            (FlagMap flags, string rawArgs) = ArgumentParser.ExtractFlags(argline);
            bool showHelp = false;
            if (UseHelpFlag && flags["h"])
            {
                if (!_module.GetHelp(command, rawArgs, flags, ref showHelp))
                    CommandModule.DefaultGetHelp(command, rawArgs, flags, ref showHelp);
                return;
            }

            _handler(command, rawArgs, flags, ref showHelp);
            if (showHelp && !_module.GetHelp(command, rawArgs, flags, ref showHelp))
                CommandModule.DefaultGetHelp(command, rawArgs, flags, ref showHelp);
        }
        catch (Exception? e)
        {
            while (e is not null)
            {
                PluginLog.Error(e, "");
                _error(
                    $"{e.GetType().Name}: {e.Message}\n",
                    ChatColour.QUIET,
                    e.TargetSite?.DeclaringType is not null ? $"at {e.TargetSite.DeclaringType.FullName} in {e.TargetSite.DeclaringType.Assembly}" : "at unknown location",
                    ChatColour.RESET
                );
                e = e.InnerException;
            }
        }
    }
}
