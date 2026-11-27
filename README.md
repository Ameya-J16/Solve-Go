# Solve&Go!

A deliberately unfair 2D platformer. Run, jump, climb, and shoot your way to the exit while the level does its best to cheat you. Built with Unity.

### ▶ [Play in your browser](https://ameya-j16.github.io/Solve-Go/)

The web build is built and deployed automatically from this repo. See [Deploying the web build](#deploying-the-web-build).

## About the Game

You control a pixel-art hero who collects coins, dodges enemies and hazards, and races to the exit of each level. The game is meant to be hard and unfair in the style of troll platformers: spikes pop out of the floor, ceilings drop on you, ground vanishes under your feet, and the exit door runs away. You learn each level by dying in it.

> The game started as a bachelor's project at RAIT, where it had to be framed as an educational quiz game. The quiz side has been dropped, and the game is now the hard platformer it was always meant to be.

There's no game over. Touching an enemy or a hazard kills you and restarts the level, and the **death counter** in the top-left corner goes up. Coins are worth **100 points** each. Score and deaths carry across levels, and when you beat the last level the main menu shows your run's totals.

## Controls

| Action | Keyboard / Mouse | Gamepad |
|--------|------------------|---------|
| Move / Climb | `WASD` or Arrow Keys | Left Stick / D-Pad |
| Jump | `Space` | Button South |
| Shoot | Left Mouse Button | Right Trigger |

Input is handled through Unity's **Input System** package, so keyboard, mouse, and gamepad all work out of the box.

## Gameplay Features

- **Run & Jump** — responsive Rigidbody2D-based movement with animated sprite flipping
- **Climbing** — ladders disable gravity and let you climb up and down
- **Shooting** — fire bullets from the player's gun to destroy enemies
- **Patrolling enemies** — enemies walk back and forth, flipping when they reach an edge
- **Coins & score** — pick up coins for points, with audio feedback
- **Death counter** — unlimited retries, quick restart on death, deaths and score shown in the HUD
- **Troll traps** — drop-in scripts for pop-out spikes, falling crushers, collapsing or vanishing floors, invisible blocks, and exits that run away (see [`Scripts_Solve&Go!/README.md`](Solve&Go!/Assets/Scripts_Solve&Go!/README.md#troll-traps))
- **8 levels + main menu**, chained together through exit doors

## Getting Started

### Requirements

- [Unity Hub](https://unity.com/download)
- **Unity 2020.3.23f1 (LTS)** — the exact editor version this project was built with

### Opening the Project

1. Clone the repository:
   ```
   git clone https://github.com/Ameya-J16/Solve-Go.git
   ```
2. Open **Unity Hub** → **Add project from disk** → select the `Solve&Go!` folder inside the repo.
3. Open the project (Unity will resolve packages on first import).
4. Open `Assets/Levels/MainMenu.unity` and press **Play**.

To build a standalone player, use **File → Build Settings** — the scene list (main menu + levels) is already configured.

### Level order

The in-game level number is the position in Build Settings, which doesn't always match the scene file name:

| In game | Scene file |
|---------|------------|
| Level 1–6 | `level 1`, `level 2`, `Level 3` … `Level 6` |
| Level 7 | `Level 9` |
| Level 8 | `Level 10` |

`Level 7` and `Level 8` exist as scene files but are empty rooms, so they're left out of the build.

## Deploying the web build

[`.github/workflows/deploy-webgl.yml`](.github/workflows/deploy-webgl.yml) uses [GameCI](https://game.ci) to build the project for **WebGL**, then publishes the result to **GitHub Pages** at <https://ameya-j16.github.io/Solve-Go/>. It runs on every push to `main` that touches `Assets/`, `Packages/` or `ProjectSettings/`. You can also start it by hand from **Actions → Build & Deploy WebGL → Run workflow**.

### One-time setup

Unity needs a license to build, even in CI, so this part has to be done once by the repo owner:

1. **Get a Unity license file (`.ulf`).**
   - Install [Unity Hub](https://unity.com/download) and sign in with your Unity account.
   - Go to **Preferences → Licenses → Add → Get a free personal license**. Do click **Add**, even if a license is already listed; that is the step that writes the file.
   - Find the file:
     - Windows: `C:\ProgramData\Unity\Unity_lic.ulf`
     - macOS: `/Library/Application Support/Unity/Unity_lic.ulf`
     - Linux: `~/.local/share/unity3d/Unity/Unity_lic.ulf`
2. **Add three repository secrets** under **Settings → Secrets and variables → Actions → New repository secret**:

   | Secret | Value |
   |--------|-------|
   | `UNITY_LICENSE` | The entire contents of `Unity_lic.ulf` (open it in a text editor, then copy and paste everything) |
   | `UNITY_EMAIL` | The email address of your Unity account |
   | `UNITY_PASSWORD` | Your Unity account password |

   GameCI warns that passwords with special characters can break activation. If activation fails, try a password made of letters and numbers only.
3. **Turn on Pages.** Go to **Settings → Pages → Build and deployment → Source** and choose **GitHub Actions**.
4. **Run it.** Push to `main`, or run the workflow by hand from the Actions tab. The first build imports the whole project and takes roughly 15–30 minutes. Later builds reuse a cached `Library` and are faster. When it finishes, the game is live at the link above.

> **Why the WebGL compression setting is Gzip + "Decompression Fallback":** GitHub Pages can't send the `Content-Encoding` headers that Unity's compressed files need. With Decompression Fallback on (`Player Settings → WebGL → Publishing Settings`), the game decompresses its own files in the browser, so it loads on any static host. If you switch it back to Brotli without the fallback, the page will get stuck while loading.

### Building the web version locally (optional)

In Unity: **File → Build Settings → WebGL → Switch Platform → Build**. You need the WebGL Build Support module, which you can add in Unity Hub under **Installs → 2020.3.23f1 → Add modules**. Opening the generated `index.html` straight from disk won't work. Serve the folder with a local web server, e.g. `python -m http.server` inside the build folder.

## Project Structure

```
Solve-Go/
├── .github/workflows/          # CI: WebGL build + GitHub Pages deploy
└── Solve&Go!/                  # Unity project root (open this in Unity Hub)
    ├── Assets/
    │   ├── Levels/             # All scenes: MainMenu + Levels 1–10
    │   ├── Scripts_Solve&Go!/  # Gameplay C# scripts (see its README)
    │   │   └── Traps/          # Troll trap components
    │   ├── Prefabs_Solve&Go/   # Player, enemy, coin, bullet, exit, session prefabs
    │   ├── Prefabs/            # UI prefabs (buttons, title text)
    │   ├── Sprites/            # Backgrounds and images
    │   ├── objects/            # Tile art, player/enemy/coin sprites, rule tiles
    │   ├── tiles/              # Tilemap assets
    │   ├── animations/         # Player/enemy animation clips & controllers
    │   ├── audio/              # Sound effects
    │   ├── components/         # Physics materials (bounce, no-friction)
    │   └── input system/       # Input System action maps and settings
    ├── Packages/               # Unity package manifest
    └── ProjectSettings/        # Project configuration
```

## Built With

- **Unity 2020.3.23f1** (2D, built-in render pipeline)
- **Input System** (`com.unity.inputsystem`) — cross-device player input
- **Cinemachine** — camera follow and confinement
- **TextMesh Pro** — UI text (menu, HUD)
- **2D Tilemap + Tilemap Extras** — level construction with rule tiles
- **2D Animation / PSD Importer / Sprite Shape** — character and world art

## Credits

Developed at **RAIT** (Ramrao Adik Institute of Technology).
