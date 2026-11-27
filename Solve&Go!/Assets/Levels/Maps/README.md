# Level maps

These text files are the layouts of in-game levels 7 to 10:

| File | In-game level |
|------|---------------|
| `Level 9.txt` | Level 7. Converted from the hand-made scene, with the first troll traps added. |
| `Level 10.txt` | Level 8. Converted the same way, with more traps. The water pool under the low ceiling is fixed. |
| `Level 11.txt` | Level 9, **OBSCENE** |
| `Level 12.txt` | Level 10, **ABSOLUTELY OBSCENE** |

When the scene starts, the scene's **Level Builder** object (`Scripts_Solve&Go!/LevelBuilder.cs`) reads its map and:
- paints the tilemaps
- moves the player and the exit
- spawns enemies and coins
- creates the troll traps
- fits the camera to the map

To change a level, edit its `.txt` file and press Play. Nothing in the scene needs touching.

## Legend

One character per tile. The top line of the file is the top of the level. Lines starting with `//` are comments.

| Char | Tile |
|------|------|
| `#` | ground |
| `=` | **fake ground**: looks solid, but you fall through. Spikes or water directly under it are hidden behind more fake ground, so the floor looks seamless. |
| `F` | **collapsing floor**: falls away a moment after you step on it (each horizontal run of `F` falls together) |
| `I` | **invisible block**: appears when you jump into the space under it |
| `^` | spikes |
| `v` | ceiling spikes |
| `<` `>` | wall spikes |
| `p` | **hidden spike**: pops out of the floor when you get close |
| `D` | **crusher**: drops from the ceiling when you walk under it |
| `M` | mushroom (bounce) |
| `w` / `W` | water / deep water (deadly) |
| `H` | ladder |
| `P` | player start |
| `X` | exit door (stands on the floor below it) |
| `x` | where the exit runs to when you get close (optional) |
| `E` / `e` | enemy walking right / left |
| `c` | coin |
| space | empty |

The Level Builder's **Origin** is the tile where the map's bottom-left corner goes. The converted levels use (−22, −8), so they stay exactly where the hand-made versions were.

The Level Builder component also has per-level difficulty settings:
- enemy speed
- how fast collapsing floors drop
- crusher gravity
- how fast the exit runs away

## Keeping it beatable

These are the player's limits, from `player1.prefab` and the physics settings:
- **Jump height:** a jump rises about **3.4 tiles**, so a step up of 3 is the most you can climb.
- **Jump distance:** a flat jump covers about **4.7 tiles**, so a gap of 4 is the longest you can clear.
- **Height:** the player is a bit under 1 tile tall and needs about 4.3 tiles of headroom for a full jump, so ceiling spikes 5 above the floor can be jumped under.
- **No short hops:** every jump is full height.
- **Mushrooms:** each bounce is 35% higher than the last.

Both maps were checked with a physics simulation of the player that assumes the worst. It treated fake floors and collapsing floors as already gone, pop spikes as already up and crushers as deadly blocks, and it tried invisible blocks both hidden and revealed. The exit was still reachable in every case. So every trap can be learned and avoided.

Avoid putting a collapsing floor (`F`) right next to fake ground (`=`): when the `F` falls, the fake ground next to it can become solid again.

To see the tiles in the editor without pressing Play, right-click the Level Builder component and choose **Preview map**. Play mode rebuilds everything from the text file anyway.
