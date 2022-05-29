using System;
using System.Diagnostics;
using System.Threading;

using Dalamud.Logging;

namespace VelaraUtils.Internal;

internal static class Logger {
    private static string MsgPrefix {
        get {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            Thread runner = Thread.CurrentThread;
            string threadDesc = $"{runner.Name ?? "unknown thread"}#{runner.ManagedThreadId}";
            return $"[{timestamp}/{threadDesc}]";
        }
    }
    [Conditional("DEBUG")]
    internal static void Debug(string tmpl, params object[] args)
        => PluginLog.Debug($"{MsgPrefix} {string.Format(tmpl, args)}");
    [Conditional("DEBUG")]
    internal static void Verbose(string tmpl, params object[] args)
        => PluginLog.Verbose($"{MsgPrefix} {string.Format(tmpl, args)}");
    internal static void Info(string tmpl, params object[] args)
        => PluginLog.Information($"{MsgPrefix} {string.Format(tmpl, args)}");
    internal static void Warning(string tmpl, params object[] args)
        => PluginLog.Warning($"{MsgPrefix} {string.Format(tmpl, args)}");
    internal static void Error(string tmpl, params object[] args)
        => PluginLog.Error($"{MsgPrefix} {string.Format(tmpl, args)}");
    internal static void Fatal(string tmpl, params object[] args)
        => PluginLog.Fatal($"{MsgPrefix} {string.Format(tmpl, args)}");
    [Conditional("DEBUG")]
    internal static void Debug(Exception ex, string tmpl, params object[] args)
        => PluginLog.Debug(ex, $"{MsgPrefix} {string.Format(tmpl, args)}");
    [Conditional("DEBUG")]
    internal static void Verbose(Exception ex, string tmpl, params object[] args)
        => PluginLog.Verbose(ex, $"{MsgPrefix} {string.Format(tmpl, args)}");
    internal static void Info(Exception ex, string tmpl, params object[] args)
        => PluginLog.Information(ex, $"{MsgPrefix} {string.Format(tmpl, args)}");
    internal static void Warning(Exception ex, string tmpl, params object[] args)
        => PluginLog.Warning(ex, $"{MsgPrefix} {string.Format(tmpl, args)}");
    internal static void Error(Exception ex, string tmpl, params object[] args)
        => PluginLog.Error(ex, $"{MsgPrefix} {string.Format(tmpl, args)}");
    internal static void Fatal(Exception ex, string tmpl, params object[] args)
        => PluginLog.Fatal(ex, $"{MsgPrefix} {string.Format(tmpl, args)}");
}
