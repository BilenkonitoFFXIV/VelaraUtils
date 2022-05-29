using System;
using System.Threading;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Buddy;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.ClientState.Conditions;
using VelaraUtils.Internal;
using XivCommon;
using Dalamud.Game.ClientState.Party;
using FFXIVClientStructs.FFXIV.Client.Game;
using VelaraUtils.Chat;
using VelaraUtils.Configuration;
using VelaraUtils.Internal.Command;
using VelaraUtils.Internal.IPC;
using VelaraUtils.Internal.Macro;
using VelaraUtils.Internal.Target;
using VelaraUtils.Utils;
using ActionManager = VelaraUtils.Internal.Action.ActionManager;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace VelaraUtils;

// ReSharper disable once ClassNeverInstantiated.Global
public unsafe class VelaraUtils : IDalamudPlugin
{
    public const string PluginName = "VelaraUtils";
    public const string Prefix = "VelaraUtils";
    public string Name => PluginName;

    private bool _disposed;

#nullable disable
    [PluginService] internal static DalamudPluginInterface PluginInterface { get; private set; }
    [PluginService] internal static SigScanner Scanner { get; private set; }
    [PluginService] internal static ClientState Client { get; private set; }
    [PluginService] internal static DataManager DataManager { get; private set; }
    [PluginService] internal static ChatGui Chat { get; private set; }
    [PluginService] internal static GameGui GameGui { get; private set; }
    [PluginService] internal static CommandManager CmdManager { get; private set; }
    [PluginService] internal static Condition Conditions { get; private set; }
    [PluginService] internal static PartyList PartyList { get; private set; }
    [PluginService] internal static BuddyList BuddyList { get; private set; }
    internal static ConfigLoader ConfigLoader { get; private set; }
    internal static GlobalConfiguration GlobalConfiguration { get; private set; }
    internal static IVariablesConfiguration VariablesConfiguration { get; private set; }
    internal static QolBar QolBar { get; private set; }
    internal static XivCommonBase Common { get; private set; }

    // internal static PluginCommandDelegate? PluginHelpCommand { get; private set; }
    internal static PluginCommandManager CommandManager { get; private set; }
    internal static PlaySound Sfx { get; private set; }
    internal static TerritoryManager TerritoryManager { get; private set; }

    internal static dynamic TargetManager { get; private set; }

    // internal static CameraManager* CameraManager { get; private set; }
    internal static ActionManager ActionManager { get; private set; }
    internal static MacroManager MacroManager { get; private set; }
    internal static CancellationTokenSource TaskScheduler { get; private set; }
#nullable restore

    private static delegate* unmanaged<uint, uint, uint> _getActionId;

    public VelaraUtils()
    {
        ConfigLoader = new ConfigLoader(PluginInterface!);

        GlobalConfiguration = (GlobalConfiguration)(PluginInterface!.GetPluginConfig() ?? new GlobalConfiguration());
        GlobalConfiguration.Initialize();

        VariablesConfiguration = ConfigLoader.Load<VariablesConfiguration>("Variables");

        // PluginHelpCommand = Delegate.CreateDelegate(typeof(PluginCommandDelegate), null,
        //     typeof(PluginCommands)
        //         .GetMethods()
        //         .First(m => m.GetCustomAttribute<PluginCommandHelpHandlerAttribute>() is not null)
        // ) as PluginCommandDelegate;
        // if (PluginHelpCommand is null)
        //     Logger.Warning("No plugin command was flagged as the default help/usage text method");
        // CommandManager = new PluginCommandManager(PluginInterface);
        Common = new XivCommonBase(); // just need the chat feature to send commands
        Sfx = new PlaySound();
        TerritoryManager = new TerritoryManager();
        TargetManager = TargetState.Initialize(PluginInterface);
        // CameraManager = (CameraManager*)Scanner?.GetStaticAddressFromSig("48 8D 35 ?? ?? ?? ?? 48 8B 09");
        ActionManager = ActionManager.Initialize(PluginInterface);
        MacroManager = MacroManager.Initialize(PluginInterface);

        TaskScheduler = new CancellationTokenSource();

        _getActionId = (delegate* unmanaged<uint, uint, uint>)Scanner?.ScanText("E8 ?? ?? ?? ?? 44 8B 4B 2C");
        PlayLib.Init();

        OffsetManager.Setup(Scanner!);

        Client!.Login += OnLogin;
        Client.Logout += OnLogout;

        QolBar = new QolBar(PluginInterface);

        if (VariablesConfiguration.Variables.TryGetValue("__init", out var cmd))
        {
            cmd = cmd.ExpandTokens(VariablesConfiguration.Variables);
            if (cmd.Length > 0) ChatUtil.SendChatLineToServer(cmd);
        }

        CommandManager = new PluginCommandManager(PluginInterface);
    }

    private static void OnLogin(object? sender, EventArgs e)
    {
        if (VariablesConfiguration.Variables.TryGetValue("__login", out var cmd))
        {
            cmd = cmd.ExpandTokens(VariablesConfiguration.Variables);
            if (cmd.Length > 0) ChatUtil.SendChatLineToServer(cmd);
        }
    }

    private static void OnLogout(object? sender, EventArgs e)
    {
        if (VariablesConfiguration.Variables.TryGetValue("__logout", out var cmd))
        {
            cmd = cmd.ExpandTokens(VariablesConfiguration.Variables);
            if (cmd.Length > 0) ChatUtil.SendChatLineToServer(cmd);
        }
    }

    public static uint GetActionId(uint actionType, uint actionCategoryId)
    {
        return _getActionId(actionType, actionCategoryId);
    }

    public static uint GetActionId(ActionType actionType, uint actionCategoryId)
    {
        return _getActionId((uint)actionType, actionCategoryId);
    }

    // public static unsafe float GetRecastTime(ActionType actionType, uint actionId) =>
    //     MathF.Min(0, ActionManager.Instance()->GetRecastTime(actionType, actionId));
    //
    // public static float GetRecastTime(byte actionType, uint actionId) =>
    //     GetRecastTime((ActionType)actionType, actionId);

    #region IDisposable Support

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            Client!.Login -= OnLogin;
            Client.Logout -= OnLogout;

            if (VariablesConfiguration.Variables.TryGetValue("__deinit", out var cmd))
            {
                cmd = cmd.ExpandTokens(VariablesConfiguration.Variables);
                if (cmd.Length > 0) ChatUtil.SendChatLineToServer(cmd);
            }

            GlobalConfiguration.Save();
            ConfigLoader.Save("Variables", VariablesConfiguration);

            CommandManager?.Dispose();
            // PluginInterface?.Dispose();

            ActionManager?.Dispose();
            MacroManager?.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~VelaraUtils()
    {
        Dispose(false);
    }

    #endregion
}
