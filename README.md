# Solve&Go!

A 2D platformer with a twist — run, jump, climb, and shoot your way through 10 levels while answering trivia questions along the way. Built with Unity.

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

## Project Structure

```
Solve-Go/
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
