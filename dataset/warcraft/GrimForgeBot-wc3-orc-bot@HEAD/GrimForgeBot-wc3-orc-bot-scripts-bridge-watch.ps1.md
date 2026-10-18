---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "scripts/bridge-watch.ps1"
ref: HEAD
---

$log = "$env:TEMP\bridge-watch.log"
Remove-Item $log -ErrorAction SilentlyContinue
"Watcher started $(Get-Date -Format HH:mm:ss)" | Out-File $log
1..120 | ForEach-Object {
    $ts = Get-Date -Format HH:mm:ss
    $procs = Get-Process Warcraft*, W3Champ* -ErrorAction SilentlyContinue | ForEach-Object {
        "$($_.Name)[$($_.Id)]:$([math]::Round($_.WS / 1MB))MB"
    }
    $ports = Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue |
        Where-Object { $_.LocalAddress -in '127.0.0.1', '0.0.0.0', '::', '::1' -and $_.LocalPort -gt 1024 } |
        ForEach-Object {
            try { $p = (Get-Process -Id $_.OwningProcess -ErrorAction Stop).Name } catch { $p = "?" }
            if ($p -match "Warcraft|W3Champ|bridge") {
                "$($p)[$($_.OwningProcess)]:$($_.LocalPort)"
            }
        } | Sort-Object -Unique
    "$ts | procs: $($procs -join ',') | ports: $($ports -join ',')" | Out-File $log -Append
    Start-Sleep -Seconds 2
}
"Watcher finished $(Get-Date -Format HH:mm:ss)" | Out-File $log -Append
