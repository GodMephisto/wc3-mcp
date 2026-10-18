---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "scripts/screenshot-named.ps1"
ref: HEAD
---

param([string]$name, [string]$out)

Add-Type -AssemblyName System.Drawing

Add-Type -TypeDefinition @"
using System;
using System.Runtime.InteropServices;
using System.Drawing;
public class WCapture {
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT r);
    [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);
    public struct RECT { public int Left, Top, Right, Bottom; }
    public const uint PW_RENDERFULLCONTENT = 0x2;
    public static Bitmap Capture(IntPtr hWnd) {
        RECT r; GetWindowRect(hWnd, out r);
        int w = r.Right - r.Left, h = r.Bottom - r.Top;
        var bmp = new Bitmap(w, h);
        using (var g = Graphics.FromImage(bmp)) {
            IntPtr hdc = g.GetHdc();
            try { PrintWindow(hWnd, hdc, PW_RENDERFULLCONTENT); }
            finally { g.ReleaseHdc(hdc); }
        }
        return bmp;
    }
}
"@ -ReferencedAssemblies System.Drawing

$proc = Get-Process $name -ErrorAction SilentlyContinue | Where-Object MainWindowHandle -ne 0 | Select-Object -First 1
if (-not $proc) { Write-Error ("no " + $name + " window"); exit 1 }

$r = New-Object WCapture+RECT
[WCapture]::GetWindowRect($proc.MainWindowHandle, [ref]$r) | Out-Null
Write-Output ("rect L=" + $r.Left + " T=" + $r.Top + " w=" + ($r.Right - $r.Left) + " h=" + ($r.Bottom - $r.Top))
$bmp = [WCapture]::Capture($proc.MainWindowHandle)
$bmp.Save($out, [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-Output ("saved: " + $out + " bytes=" + (Get-Item $out).Length)
