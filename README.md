# Flip Clock Screensaver for Windows

A clean, fullscreen retro digital flip clock screensaver for Windows 10 & 11. Runs 100% offline with zero telemetry, native system time, and full multi-monitor support.

![Fullscreen Flip Clock Screensaver](actual_clock.png)

---

## Overview

If you love the classic look of a mechanical split-flap clock on your desk, this screensaver turns your idle PC or multi-monitor setup into a minimalist, full-screen digital flip clock.

* **Classic Flip Clock Style:** Retro flip-down animation with clean typography that scales to any screen resolution (1080p, 2K, 4K, and ultrawide).
* **100% Offline & Private:** Zero internet connection required. No background network requests, no Google Tag Manager, no analytics.
* **12h & 24h Time Formats:** Defaults to 24-hour time (`17:30`), with easy one-click switching to 12-hour format with AM/PM indicators.
* **Multi-Monitor Support:** Displays seamlessly across all active displays.
* **Customizable:** Easily tweak clock scale and tile brightness to match your workspace lighting.
* **Standard Windows Screen Saver:** Packaged as a native `.scr` executable with standard Windows Screen Saver Settings integration.

---

## Why This Exists

For years, the go-to flip clock screensaver on Windows was [Fliqlo](https://fliqlo.com/). However, recent official Windows releases (v1.5.1) introduced an unwanted change: the `.scr` executable was converted into an online web wrapper that loaded remote assets from `https://fliqlo.app/` on every run. If your PC went offline or lost Wi-Fi, the screensaver would fail with a blank black screen or an exclamation point icon. It also included external Google Analytics trackers and blocked rendering whenever Windows reported no active network.

This release fixes all of that:
* **All assets in-memory:** HTML, CSS, 3D flip animations, JavaScript, and custom fonts are embedded directly inside the binary.
* **In-process loopback server:** A lightweight in-memory HTTP server on `127.0.0.1` serves assets locally, bypassing Internet Explorer webview font restrictions without writing temporary files to disk.
* **Direct system clock:** Reads the exact time straight from Windows.
* **No network required:** Disconnect Wi-Fi, enable airplane mode, or run on an air-gapped machine—it works immediately every time.

---

## Download

Get the prebuilt screensaver directly:

* **[Download Fliqlo.scr (Direct Download)](https://github.com/andreysdsgroup/flip-clock-screensaver/releases/download/v1.5.2/Fliqlo.scr)**
* **[Download Full Release (.zip)](https://github.com/andreysdsgroup/flip-clock-screensaver/releases/download/v1.5.2/Fliqlo-Offline-v1.5.2.zip)**
* You can also find the prebuilt binary in the [`dist/`](dist/Fliqlo.scr) directory of this repository.

---

## Installation

1. Download **`Fliqlo.scr`** from the link above.
2. Right-click **`Fliqlo.scr`** and select **Install**.
3. Windows will open the **Screen Saver Settings** panel with the clock screensaver selected.
4. Click **OK** to apply. That's it!

*(Optional manual install)*: Move `Fliqlo.scr` to your preferred folder (such as `C:\Windows\System32\` or `C:\Users\<YourUser>\AppData\Local\Programs\Fliqlo\`), right-click the file and click **Install**.

---

## Changing Time Format (12h vs 24h) & Settings

The screensaver defaults to **24-hour format** out of the box, but you can customize it anytime:

1. Open Windows **Screen Saver Settings** (press `Win + S`, search for `Change screen saver`, and press Enter).
2. Click the **Settings...** button.  
   *(Alternatively, right-click `Fliqlo.scr` in File Explorer and click **Configure**, or run `Fliqlo.scr /c`).*
3. In the settings window:
   * **Time Format:** Choose **24h**, **12h** (with AM/PM indicator), or **24h without leading zero**.
   * **Scale:** Adjust the slider to make the clock smaller or larger.
   * **Brightness:** Adjust background tile brightness.
4. Click **OK** to save your preferences.

---

## Launch on Demand (Desktop Shortcut)

To start the flip clock instantly without waiting for the Windows idle timer:

1. Right-click your desktop and choose **New > Shortcut**.
2. Set the target to your screensaver file followed by `/s`:
   ```cmd
   "C:\Path\To\Fliqlo.scr" /s
   ```
3. Name it **Flip Clock Screensaver**.
4. Double-click the shortcut anytime to launch the clock fullscreen across your displays.

---

## Building from Source

Requirements: Windows 10/11 with the .NET SDK or Visual Studio build tools.

### Build via Batch Script
```cmd
build.bat
```
This detects your Roslyn `csc.exe` compiler and outputs the standalone executable to `dist\Fliqlo.scr`.

### Build via .NET CLI
```cmd
dotnet build Fliqlo.csproj -c Release
```

---

## Credits & License

* Original flip clock concept, visual design, and typography by **Yuji Adachi** ([9031.com](https://9031.com/) / [fliqlo.com](https://fliqlo.com/)).
* Standalone offline Windows release by [andreysdsgroup](https://github.com/andreysdsgroup).
* Released under the [MIT License](LICENSE).
