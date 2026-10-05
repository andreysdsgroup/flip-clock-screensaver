# Fliqlo (Offline Edition)

A completely offline, telemetry-free flip clock screensaver for Windows.

![Fliqlo 24h Clock](actual_clock.png)

---

## The Story

If you've used the popular [Fliqlo](https://fliqlo.com/) flip clock screensaver on Windows recently (version 1.5.1), you might have noticed something annoying: disconnect from Wi-Fi or go offline, and instead of a clock, you get a black screen or a giant gray exclamation mark.

Under the hood, the official 1.5.1 Windows release was just a thin wrapper hosting an Internet Explorer web view that phoned home to `https://fliqlo.app/` every time it woke up. It loaded external scripts, bundled Google Analytics tracking, and explicitly checked `NetworkInterface.GetIsNetworkAvailable()` to refuse to render if there was no active internet connection.

This fork fixes all of that:
* **Zero network calls:** Everything (HTML, CSS3 3D flip animations, JS logic, and custom typography) is compiled directly into the binary as in-memory resources.
* **No tracking:** Google Tag Manager and external analytics have been removed.
* **Local loopback server:** A tiny in-process HTTP server on `127.0.0.1` serves the assets directly from RAM, cleanly bypassing local Internet Explorer `file:///` font security blocks without creating temp files on disk.
* **Native system time:** Time is read directly from your Windows system clock.
* **Defaults to 24-hour time:** Modern 24h clock out of the box, with easy toggle back to 12h if you prefer.

---

## Download

You don't need to compile anything if you just want the screensaver:

* **[Download Fliqlo.scr (Direct Download)](https://github.com/andreysdsgroup/fliqlo-offline/releases/download/v1.5.2/Fliqlo.scr)**
* **[Download Fliqlo-Offline-v1.5.2.zip](https://github.com/andreysdsgroup/fliqlo-offline/releases/download/v1.5.2/Fliqlo-Offline-v1.5.2.zip)**
* Or grab the prebuilt file straight from the [`dist/`](dist/Fliqlo.scr) directory in this repo.

---

## Installation

1. Grab **`Fliqlo.scr`** from the links above.
2. Right-click **`Fliqlo.scr`** and click **Install**.
3. Windows will open the **Screen Saver Settings** panel with Fliqlo selected as your active screensaver.
4. Click **OK** to save. That's it!

*(Optional manual install)*: Move `Fliqlo.scr` to `C:\Windows\System32` or keep it in any folder (like `C:\Users\<YourUser>\AppData\Local\Programs\Fliqlo\`), right-click and choose Install.

---

## How to Switch Between 12h and 24h Time

This build defaults to **24-hour format** (`16:08`), but you can switch to 12-hour format or customize the look anytime:

1. Open Windows **Screen Saver Settings** (press `Win + S`, type `Change screen saver`, and press Enter).
2. Make sure **Fliqlo** is selected, then click the **Settings...** button.  
   *(Or simply right-click `Fliqlo.scr` in File Explorer and click **Configure**, or run `Fliqlo.scr /c` from the command line).*
3. In the settings panel:
   * **Hour Format**: Pick **24h**, **12h** (with AM/PM indicators), or **24h without leading zero**.
   * **Scale**: Drag the slider to shrink or expand the flip clock on your display.
   * **Brightness**: Adjust the background tile brightness.
4. Click **OK**. Your preferences are saved locally and persist across reboots.

---

## Running Directly from a Shortcut

To start the screensaver on demand without waiting for the idle timeout:
* Create a shortcut to `Fliqlo.scr`.
* Right-click the shortcut, select **Properties**, and add `/s` to the end of the **Target** field:
  ```
  "C:\Path\To\Fliqlo.scr" /s
  ```
* Double-clicking the shortcut will launch the full-screen clock across all your monitors immediately.

---

## Building from Source

Requirements: Windows 10/11 with .NET SDK or Visual Studio / MSBuild installed.

### One-Click Build
Run the included batch script from the repo root:
```cmd
build.bat
```
The script will locate your Roslyn `csc.exe` compiler and generate `dist\Fliqlo.scr`.

### .NET CLI Build
```cmd
dotnet build Fliqlo.csproj -c Release
```

---

## Credits & License

* Original screensaver concept, visual design, and flip clock font by **Yuji Adachi** ([9031.com](https://9031.com/) / [fliqlo.com](https://fliqlo.com/)).
* Offline modification and Windows integration by [andreysdsgroup](https://github.com/andreysdsgroup).
* Distributed under the [MIT License](LICENSE).
