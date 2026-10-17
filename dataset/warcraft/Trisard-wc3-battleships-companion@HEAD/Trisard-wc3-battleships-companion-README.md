---
type: vendor-doc
source_repo: "https://github.com/Trisard/wc3-battleships-companion"
source_path: "README.md"
ref: HEAD
---

<p align="center">
  <img src="./src/assets/logo.png" alt="Battleships Companion Logo" width="120" />
</p>

<h1 align="center">Battleships Companion</h1>

<p align="center">
  Companion application for <strong>Battleships Crossfire</strong> custom games on Warcraft III Reforged.
</p>

---

## Features

*   **Team Display**: Lists players in the game lobby grouped by their teams.
*   **Balance Command**: Copies the balancing command to the clipboard as `?balance Player1 Player2 ...`.
*   **Portable Version**: Single standalone executable file (`.exe`), no installation required.

---

## Usage

1.  Start **Warcraft III Reforged** and log in to Battle.net.
2.  Launch **`battleships-companion.exe`**.
    *   *Note: If connection fails, run the application as **Administrator** to allow scanning Warcraft III local ports.*
3.  Join a Battleships lobby.
4.  Click the **"Copy balance command"** button in the top-right corner to copy the discord elo bot balance command.

---

## Synchronization Tip

If you launch the companion while you are already in the game lobby, the application might remain in a waiting state.
*   **Solution**: Perform a quick action in the lobby (e.g. change color, race, or modify handicap) to resend the setup and trigger immediate synchronization.

---

## Download

You can download the latest standalone portable `.exe` from the [Releases](../../releases) page.
