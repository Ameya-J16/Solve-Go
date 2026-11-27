# Solve&Go!

A 2D platformer with a twist — run, jump, climb, and shoot your way through 10 levels while answering trivia questions along the way. Built with Unity.

### ▶ [Play in your browser](https://ameya-j16.github.io/Solve-Go/)

The web build is built and deployed automatically from this repo. See [Deploying the web build](#deploying-the-web-build).

## About the Game

Solve&Go! combines classic platformer action with quiz-based learning. You control a pixel-art hero who collects coins, dodges enemies and hazards, and races to the exit of each level. Between the action, quiz stages test your knowledge with multiple-choice questions (mostly computer/tech trivia).

You start with **3 lives**. Touching an enemy or a hazard costs a life and restarts the level; lose them all and the game resets back to the main menu. Coins are worth **100 points** each, and your score and lives persist across levels.

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
- **Lives system** — 3 lives, level restarts on death, full reset on game over
- **Quiz stages** — multiple-choice questions defined as ScriptableObject assets
- **10 levels + main menu**, chained together through exit doors

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
    │   ├── QuestionS/          # Quiz question ScriptableObject assets
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
- **TextMesh Pro** — UI text (score, lives, quiz)
- **2D Tilemap + Tilemap Extras** — level construction with rule tiles
- **2D Animation / PSD Importer / Sprite Shape** — character and world art

## Credits

Developed at **RAIT** (Ramrao Adik Institute of Technology).
