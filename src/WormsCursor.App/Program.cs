using System.Diagnostics;
using Velopack;
using WormsCursor.Core;

namespace WormsCursor.App;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        // Agent-notifier bridge: `WormsCursor.exe hook …` is a throwaway invocation fired by an
        // AI agent's hook. Handle it FIRST — before Velopack and the single-instance guard — so
        // it starts lean and never spins up UI: it forwards one event over the pipe and exits 0.
        // Fail-silent (it must never block or crash the agent), so this can't throw.
        if (AgentHookBridge.IsHookInvocation(args))
        {
            AgentHookBridge.Run(args);
            return;
        }

        // Velopack hijacks Main when invoked with --veloapp-* args during
        // install / update / uninstall: it runs the matching hook and exits
        // before any UI spins up. Must stay the very first call in Main.
        // No-ops cleanly for dev builds run straight out of bin\.
        VelopackApp.Build()
            // Restore the real cursors on uninstall. This hook runs in a FRESH, short-lived
            // process (WormsCursor.exe --veloapp-uninstall) that never themed anything, while the
            // tray instance is a SEPARATE process still re-applying our cursors every frame — so a
            // bare RestoreDefaultCursors() here would be overwritten on the tray's next tick. Hence
            // RestoreCursorsOnUninstall() stops the tray first (which also unlocks WormsCursor.exe
            // so Velopack can delete it), THEN reloads the genuine scheme. Without this, uninstalling
            // leaves the themed cursors live until the user resets them by hand.
            .OnBeforeUninstallFastCallback(_ => RestoreCursorsOnUninstall())
            .Run();

        // Single-instance guard, before any UI: if another instance is already
        // running, Acquire() signals it to open Preferences and returns null, so we
        // exit immediately — no tray icon, no cursor takeover.
        using var single = SingleInstance.Acquire();
        if (single is null) return;

        // High-DPI mode etc. comes from ApplicationHighDpiMode in the .csproj.
        ApplicationConfiguration.Initialize();

        // No main window: the whole app lives in the tray.
        var tray = new TrayApplicationContext();
        single.StartListening(tray.OpenPreferences);
        Application.Run(tray);
    }

    // Uninstall cleanup, run from the --veloapp-uninstall hook process. Kill any OTHER running
    // WormsCursor instances (the tray) so nothing re-themes the cursors behind us, then reload the
    // user's real scheme. RestoreDefaultCursors is registry-based (SetSystemCursor never touches the
    // registry), so it restores the genuine cursors even though this process never set them.
    // Fail-silent and quick: a throw or a hang here would just burn the 30 s the fast callback is
    // allowed before Velopack force-kills us — and the cursors are restored last regardless.
    static void RestoreCursorsOnUninstall()
    {
        try
        {
            int self = Environment.ProcessId;
            foreach (var p in Process.GetProcessesByName("WormsCursor"))
                using (p)
                {
                    if (p.Id == self) continue;
                    try { p.Kill(); p.WaitForExit(3000); } catch { /* already exiting / access denied */ }
                }
        }
        catch { /* enumeration failed — restore anyway */ }

        CursorEngine.RestoreDefaultCursors();
    }
}
