# Gameplay Scripts

All core gameplay logic for Solve&Go! lives in this folder. Every script is a `MonoBehaviour` attached to a prefab or scene object.

## Script Reference

### `playerMovement.cs`
The player controller. Uses the **Input System** callbacks (`OnMove`, `OnJump`, `OnFire`) rather than the legacy input manager.

- **Run** — horizontal velocity from move input, drives the `isRunning` animator bool
- **Jump** — only allowed while the feet collider (`BoxCollider2D`) touches the `Ground` layer
- **Climb** — while touching the `Climbing` layer, gravity is zeroed and vertical input moves the player; drives the `isClimbing` animator bool
- **Fire** — instantiates the bullet prefab at the `gun` transform
- **Die** — when the body capsule touches the `enemies` or `Hazards` layers, the player gets a "death kick", the `dying` animator trigger fires, and `gameSession.ProcessPlayerDeath()` is called

Tunables exposed in the Inspector: `runSpeed`, `jumpSpeed`, `climbSpeed`, `deathkick`, bullet prefab, and gun transform.

### `gameSession.cs`
Singleton-style session manager (survives scene loads via `DontDestroyOnLoad`, self-destructs if a duplicate exists). Tracks **lives** (default 3) and **score**, and updates the TextMesh Pro HUD labels.

- `AddToScore(int)` — called by pickups
- `ProcessPlayerDeath()` — reloads the current level if lives remain, otherwise resets the whole session and returns to scene 0 (main menu)

### `ScreenPersist.cs`
Companion singleton that keeps scene-persistent objects alive between levels. `ResetScenePersist()` destroys it so a fresh copy is created when the game restarts.

### `enemyMovement.cs`
Simple patrol AI: moves horizontally at `moveSpeed`, and when its edge-detection collider exits the ground (`OnTriggerExit2D`) it reverses direction and flips its sprite.

### `coinPickUp.cs`
Trigger pickup. On contact with the `Player` tag it adds `pointsForCoinPickup` (default 100) to the session score, plays the pickup sound at the camera, and destroys itself. A `wascollected` guard prevents double-collection in a single frame.

### `exit.cs`
Level exit door. When the player enters the trigger, waits `levelLoadDelay` seconds (realtime) and loads the next scene in Build Settings, wrapping back to scene 0 after the last level. Resets the screen persist object before loading.

### `MainMenu.cs`
Menu button handlers: `PlayGame()` loads the next scene in the build order, `QuitGame()` exits the application.

### `bullet.cs` / `NewBehaviourScript.cs` — ⚠️ note the swap
The file names are swapped relative to the classes they contain:

- **`NewBehaviourScript.cs`** contains the **`bullet`** class — the actual projectile. It travels horizontally in the direction the player faces (`bulletSpeed`, default 10), destroys objects tagged `Enemies` on trigger contact, and destroys itself on any collision.
- **`bullet.cs`** contains the empty template class **`NewBehaviourScript`** — it does nothing.

Because Unity requires a MonoBehaviour's class name to match its file name to attach it in the Inspector, renaming these files to match their classes would fix the mismatch — but do it inside the Unity editor so the `.meta` GUIDs (and the bullet prefab reference) stay intact.

## Layers & Tags the Scripts Depend On

| Name | Type | Used by |
|------|------|---------|
| `Ground` | Layer | jump ground-check |
| `Climbing` | Layer | ladder climbing |
| `enemies`, `Hazards` | Layers | player death check |
| `Player` | Tag | coin pickup, exit trigger |
| `Enemies` | Tag | bullet hit detection |
