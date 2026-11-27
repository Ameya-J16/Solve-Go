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

### `bullet.cs`
The projectile, attached to the bullet prefab. It travels horizontally in the direction the player faces (`bulletSpeed`, default 10), destroys objects tagged `Enemies` on trigger contact, and destroys itself on any collision.

> The contents of `bullet.cs` and `NewBehaviourScript.cs` used to be swapped. Because Unity needs a MonoBehaviour's class name to match its file name, the bullet prefab (which references `bullet.cs`) showed a missing script and bullets did nothing. The contents are now back in the right files. The `.meta` GUIDs were left alone, so the prefab reference still works.

### `NewBehaviourScript.cs`
An empty Unity template script. Nothing uses it, so it can be deleted.

## Troll Traps

The scripts in `Traps/` turn ordinary objects into traps. Each one watches a **trigger zone**, drawn as a red box in the Scene view, and springs the moment the player's body touches it. You don't need extra trigger colliders. Move and resize the box with **Zone Offset** and **Zone Size** in the Inspector. When the player dies, the level reloads and every trap resets.

| Script | What it does | Layer to use | Where to put the zone |
|--------|--------------|--------------|-----------------------|
| `PopOutTrap` | Starts hidden (e.g. inside the floor) and shoots out when you get close. **Hidden Offset** is where it hides, relative to where you placed it. | `Hazards` | Where the player will be standing when it pops |
| `DropTrap` | Hangs still, then falls. Use it as a ceiling crusher or a collapsing floor. Adds a `Rigidbody2D` automatically. | Crusher: `Hazards`<br>Floor: `Ground` | Crusher: under it (default)<br>Floor: just on top of it |
| `ToggleBlock` | **Vanish**: floor that disappears when you step on it.<br>**Appear**: invisible block that shows up when you jump, so you bonk your head on it. | `Ground` | Vanish: on top of it (default)<br>Appear: under it |
| `RunawayTrap` | Slides by **Move By** when you get close. Put it on the exit door so the door runs away, or on a platform so it slides out from under you. The yellow line shows where it goes. | Whatever the object already uses | In front of it, on the side the player comes from |

All traps also have:
- **Delay**: seconds between entering the zone and the trap firing. Leave it at 0 for maximum unfairness.
- **`Activate()`**: a public method, so you can also fire a trap from a UnityEvent, e.g. a button or another trap.

### Adding a trap to a level

Traps work on **individual GameObjects**, not on tiles painted into a Tilemap, because a whole Tilemap is a single object.

1. Drag a sprite into the scene, e.g. a spike or ground tile from `Assets/objects/`.
2. Add a **BoxCollider2D**. Leave **Is Trigger** off.
3. Set the object's **Layer** using the table above. `Hazards` kills on touch, and `Ground` can be stood on and jumped from.
4. Add the trap component and drag the red zone box to where it should fire.
5. Press Play and die a lot.

Don't put traps under the **Screen Persist** object. Anything under it survives level reloads, so a trap there would stay sprung after you die.

Some troll ideas don't need a script:
- **Fake floor**: a ground sprite with no collider. You fall right through it.
- **Fake exit**: a door sprite on the `Hazards` layer. It kills you.
- **Fake coin**: a coin sprite on the `Hazards` layer.

## Layers & Tags the Scripts Depend On

| Name | Type | Used by |
|------|------|---------|
| `Ground` | Layer | jump ground-check |
| `Climbing` | Layer | ladder climbing |
| `enemies`, `Hazards` | Layers | player death check |
| `Player` | Tag | coin pickup, exit trigger |
| `Enemies` | Tag | bullet hit detection |
