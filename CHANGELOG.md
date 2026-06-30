# Changelog

Notable changes to WormsCursor. Roughly follows
[Keep a Changelog](https://keepachangelog.com/); version numbers match the git tags and
GitHub releases. The release workflow pulls the matching section into each release's notes.

## 0.8.2 - 2026-06-30

### Fixed
- **Uninstalling now restores your real cursors.** Removing WormsCursor while it was running
  left the themed cursors in place — you had to reset them by hand in Windows settings. The
  uninstaller now stops the running tray instance and reloads the genuine cursor scheme before
  the app is removed.

## 0.8.1 - 2026-06-15

### Fixed
- **The agent logo now clears on `/clear` and `/exit`.** If the Claude Code hooks had been
  registered by an older build (before the session-lifecycle events were added), ending a
  session left the logo hanging until the linger timeout instead of clearing at once.
  **Preferences → Agent settings…** now detects an out-of-date registration and shows
  **"Update available" → "Re-register"**; refreshing it restores the full event set so the
  logo disappears the moment a session ends.

## 0.8.0 - 2026-06-15

### Added
- **Agent notifier — a waiting AI agent's logo on your cursor.** When an AI coding agent
  (Claude Code) is waiting on you, the cursor sprouts a small tool-logo charm that hangs and
  swings on the same pendulum as the busy ring / help "?", with a frameless **"+N"** when more
  than one agent waits at once — an ambient, peripheral-vision nudge, no toast and no extra
  window. Set it up in **Preferences → Agent settings…**: register Claude Code (WormsCursor
  writes a `hook` into `~/.claude/settings.json` with backup-and-merge, never overwriting your
  own hooks), toggle the logo, and choose how long a stuck logo lingers before it clears itself.
  The logo appears when the agent is blocked on you (a permission/idle prompt) or finishes its
  turn, and clears the moment you reply or the session ends — a hard-killed agent's logo lingers
  until the timeout, since no exit hook fires.

### Changed
- **Lower idle CPU on the animated cursors.** The engine now skips its per-frame work when
  nothing's happening: a settled resize / I-beam / move / "unavailable" cursor reuses a cached
  frame instead of re-compositing, and the spring/clock physics is skipped entirely when an idle
  pointer sits on a still mouse — plus a pass that cut per-frame allocations and Win32 calls across
  the whole render path. Purely internal; the cursors look and animate exactly as before.

## 0.7.3 - 2026-06-13

### Changed
- **The I-beam typing bounce has its own on/off toggle.** "Click feedback" now covers just
  the pointer + crosshair squash & pop; the text I-beam's hop/shiver is a separate checkbox so
  it can be turned off on its own. Typing feedback is also calmer — a gentler hop and a small
  rate limit between keystrokes — so typing fast no longer "recoils" like a machine gun. The
  keyboard hook behind it is now installed only while the I-beam toggle is on.

## 0.7.2 - 2026-06-13

### Added
- **"What's new" changelog in the app**: a *What's new* link in Preferences opens a dialog
  that lists the published releases (newest first) and shows the selected version's notes,
  fetched from the GitHub Releases API (each release body is its CHANGELOG section). The
  notes get light markdown formatting; if the fetch fails (offline / rate-limited / dev
  build) it falls back to a *View on GitHub* button. After an update, the new version's
  notes pop up once automatically.

## 0.7.1 - 2026-06-13

### Added
- **Per-cursor on/off in Preferences**: every tile in the cursor preview now has a
  checkbox (all ticked by default). Untick one to leave that cursor as the Windows
  default instead of theming it — WormsCursor simply stops touching that slot. Disabled
  tiles are shown faded so it's clear what's off. **Showtime** cycles only the enabled
  cursors, and the setting persists with the rest of your preferences.
- **Click feedback** (Preferences, on by default): the pointer and the crosshair do a
  quick "squash & pop" while a mouse button is held — a tactile press-and-release — and
  the text I-beam hops and shivers slightly side-to-side as you type. The effects also
  play in the Preferences demo (Test cursor / hover-to-try / Showtime). One checkbox
  toggles it. Mouse
  state is polled (no mouse hook); the
  typing hop uses a low-level keyboard hook that reads only *that* a key fired (never
  which — nothing is logged), installed only while the feature is on.

## 0.7.0 - 2026-06-13

### Added
- **Showtime mode in Preferences**: a hands-free demo for recording. After a 3-2-1
  lead-in (time to start your capture) it forces each test cursor on screen in turn
  (1.5s each), looping until you click **Stop** or close the dialog — so the whole set
  can be recorded reacting to mouse movement without clicking through the *Test cursor*
  combo by hand. Move the mouse while it runs so the motion-driven animations play; the
  dropdown follows along (disabled) to label the live cursor, and the button doubles as
  Stop.

## 0.6.1 - 2026-06-03

### Added
- **Wider, two-column Preferences layout**: the cursor preview is now a **7×2 grid** (matching
  the README sheet) instead of 5 columns, and the controls below sit in two columns — sliders
  on the left, colours + the *Test cursor* combo on the right — so the window is wide-and-short
  rather than a tall stack. The colour buttons now show their hex value in a contrasting ink so
  a black/white swatch still reads as a colour picker.
- **Hover-to-try in the Preferences preview**: moving the mouse over a cursor tile borrows
  that cursor onto your real pointer (live), and the tile empties to a dashed pocket so it's
  clear where it went. Moving off the grid hands the pointer back to whatever the *Test
  cursor* combo has selected. Like the *Test cursor* combo, the on-screen cursor reflects the
  currently **applied** appearance — click **Apply** to preview unsaved size/colour edits.

### Fixed
- The Preferences window now rescales when dragged between monitors with different display
  scaling (e.g. 150% ↔ 200%). It's a hand-coded form that never set `AutoScaleMode`, so
  WinForms didn't re-scale it on a DPI change (despite the app being PerMonitorV2) and it
  looked cut off on the other monitor; it now uses `AutoScaleMode.Font`.

### Known issues
- **Animated cursors flicker on mixed-DPI multi-monitor setups** (e.g. a 125% screen next to a
  150% one). The animated cursors re-install via `SetSystemCursor` every frame; on a monitor
  whose scale differs from the one the cursor was created for, Windows re-composites the global
  cursor on each swap, which flashes. Static cursors (the rotating arrow/hand at rest) are
  unaffected. Workaround: match both monitors' scale, or enable Pointer trails (shortest). A
  proper fix needs the animated cursors drawn in our own overlay instead of `SetSystemCursor`.

## 0.5.0 - 2026-06-03

### Added
- **Crosshair / precision cursor** (`OCR_CROSS`): a reticle — centre dot, four axis ticks
  and a slowly-rotating broken ring — whose ticks "breathe" and spread outward (recoil)
  when you move fast, then settle.
- **Text / I-beam cursor** (`OCR_IBEAM`): a flexible beam — the bottom is rigid but the
  top sways opposite to motion on a soft underdamped spring, so it wobbles like jelly as
  you move and settles afterwards.
- **Resize cursors** (`OCR_SIZEWE` / `SIZENS` / `SIZENWSE` / `SIZENESW`) and the **move
  cursor** (`OCR_SIZEALL`): stretched-taffy double-arrows — drag along the axis and the
  shaft necks thin while the heads fly apart on an underdamped spring, then blob back. Move
  crosses a horizontal and a vertical taffy arrow into a 4-way glyph whose arms stretch with
  motion along each axis.
- **Unavailable cursor** (`OCR_NO`): a red circle-with-slash whose ring is a jelly blob — it
  deforms into an egg along the direction of travel and wobbles back to round when you stop.
- **Alternate-select** (`OCR_UP`) is themed with the same rotating arrow as the normal pointer.
  WormsCursor now covers **all 14 standard system cursors**.
- **`WormsCursor.Preview`** project: renders a labelled showcase of every themed cursor to
  PNG (a dark sheet plus a transparent one) for the README / docs, used in the new
  **Cursors** section of the README.

### Changed
- Preferences preview now shows **every** cursor in a single neutral 5-column grid, drawn at
  **real size (never scaled)**; the window is sized once to fit and the render is debounced so
  dragging the size slider stays smooth (was a side-by-side dark/light strip of just arrow + hand).
- The help cursor's **"?"** is now hand-drawn — a rounded hook plus a separate round dot —
  instead of a font glyph, for cleaner rounding and a correctly-aligned, smaller dot.

### Fixed
- Animated cursors (busy, app-starting, help, crosshair, text) now actually animate while
  on screen in real apps, not just under the Preferences "Test cursor". `SetSystemCursor`
  destroys the handle you pass it, so the on-screen-detection check was comparing against
  a dead handle and never matched; it now matches the live system handle (`LoadCursor`).

## 0.4.0 - 2026-06-02

### Added
- **Busy / progress cursors** are now themed to match. *App-starting* (`OCR_APPSTARTING`)
  is the rotating arrow plus a spinning comet ring that hangs off its tail on a springy
  string under gravity — it swings out as you move and settles straight down when you
  stop. *Wait* (`OCR_WAIT`) is the same ring centred on the pointer (spin only, no
  physics, so you can always see where you're pointing). They animate only while actually
  on screen (checked via `GetCursorInfo`), so an idle tray uses no CPU.
- The **help cursor** (`OCR_HELP`) is themed too: the rotating arrow with a "?" that hangs
  upside-down off the tail on the same pendulum string, swinging to the sides as you move.
- Preferences: a **Test cursor** control that forces a chosen cursor (arrow, hand, wait,
  app-starting, help) on screen, so you can preview the animated cursors on demand; it
  clears automatically when the dialog closes.
- Preferences: an **Apply** button that commits edits to the live cursor without closing
  the dialog (so you can tune size/colour and watch the test cursor update). "Check for
  updates" moved down beside the version to make room for it.

## 0.3.0 - 2026-06-02

### Added
- The **hand / link cursor** (`OCR_HAND`) now rotates to follow movement too. It's drawn
  the same way as the arrow — a filled silhouette plus a pen-stroked outline, finger
  separators and knuckle creases — so it shares the arrow's fill/outline colour, size and
  outline thickness.

### Changed
- Outline thickness now scales every hand line together (outline + finger creases),
  exactly 1:1 with the arrow, and the slider is capped at 4 px (beyond that the lines just
  merge). At thickness 0 the hand is a bare fill, like the arrow.
- Preferences: corner radius is now labelled **arrow only** (it never applied to the hand).

### Removed
- The hand's source SVG and its bake script (`tools/`); `HandShape.cs` is now the baked
  source of truth (recoverable from git history if the shape ever needs regenerating).

## 0.2.1 - 2026-06-02

### Fixed
- Release deltas: `vpk download` now fetches the previous release into the same
  directory `vpk pack` writes to, so updates ship a small delta package instead of the
  full payload every time.

### Changed
- Preferences: the **Fill** and **Outline** colour pickers now share a single row,
  side by side.

### Added
- This CHANGELOG, plus a release step that extracts the matching section into the
  GitHub release notes.

## 0.2.0 - 2026-06-02

### Added
- The arrow icon is embedded in the Velopack installer and updater
  (`Setup.exe` / `Update.exe`) instead of the Velopack default.

### Changed
- Preferences footer split over two lines: version + update status on top, the repo
  link on its own line below (no more truncated URL).

## 0.1.0 - 2026-06-02

Initial release — a Worms 3D-style rotating cursor for Windows, packaged as a tray app.

### Added
- Cursor engine (`WormsCursor.Core`, UI-agnostic): the system arrow rotates to follow
  mouse-movement direction, with jitter-resistant aiming and smoothly animated turns.
- System-tray app (WinForms, no main window): Enabled toggle, double-click to toggle.
- Live Preferences dialog: cursor size, fill/outline colour, outline thickness and
  corner radius, with a side-by-side dark/light preview and a Defaults reset.
- Settings saved as JSON in `%LocalAppData%\WormsCursor\` (persist across restarts and
  updates).
- "Start with Windows" autostart via a per-user `HKCU\…\Run` entry (no admin).
- Single instance only: launching a second time opens Preferences instead of starting a
  duplicate.
- Cursor restored on quit and on crash, and re-checked on every launch (self-heal);
  `tools/RestoreCursor.ps1` as a manual fallback.
- Monochrome arrow app/tray icon (white glyph + black frame), generated by
  `tools/generate-icon.py`.
- Velopack installer + auto-update; "Check for updates" in the tray and in Preferences.
- CI build workflow and a tag-triggered release workflow producing `Setup.exe` and
  `Portable.zip`.
- Demo video embedded in the README, and an MIT license.
