using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

// Builds a level from a text map when the scene starts: paints the tilemaps,
// moves the player and the exit, spawns enemies and coins, creates the troll
// traps and fits the camera bounds to the map.
//
// The map is a text file, one character per tile. Lines starting with "//"
// are comments. See Assets/Levels/Maps/README.md for the full legend.
//
// The scene keeps its usual objects (Grid + tilemaps, player1, exit, Cameras,
// Game Session, Screen Persist); this script only arranges them. Right-click
// the component and choose "Preview map" to see the tiles in the editor.
[DefaultExecutionOrder(-100)]
public class LevelBuilder : MonoBehaviour
{
    [Header("Map")]
    [Tooltip("Text file with the level layout, one character per tile.")]
    [SerializeField] TextAsset mapFile;
    [Tooltip("Tile position of the map's bottom-left corner. Lets a converted level stay where it was.")]
    [SerializeField] Vector2Int origin;

    [Header("Tiles")]
    [SerializeField] TileBase groundTile;
    [SerializeField] TileBase spikeTile;
    [SerializeField] TileBase ladderTile;
    [SerializeField] TileBase mushroomTile;
    [Tooltip("'w' in the map: water surface (deadly, on the Hazards tilemap).")]
    [SerializeField] TileBase waterTile;
    [Tooltip("'W' in the map: deep water (deadly, on the Hazards tilemap).")]
    [SerializeField] TileBase deepWaterTile;
    [Tooltip("Background tiles, picked per cell. Repeat a tile to make it more common.")]
    [SerializeField] TileBase[] backgroundTiles;

    [Header("Prefabs")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject coinPrefab;

    [Header("Difficulty")]
    [SerializeField] float enemySpeed = 3f;
    [Tooltip("Seconds between stepping on an 'F' floor and it falling.")]
    [SerializeField] float collapseDelay = 0.15f;
    [SerializeField] float crusherGravity = 6f;
    [SerializeField] float exitRunSpeed = 9f;

    const int Margin = 12;   // background painted around the map so the camera never sees a void

    Tilemap ground, hazards, climbing, bouncing, background;
    char[,] cells;           // [x, y], y = 0 is the bottom row of the map
    int width, height;

    void Awake()
    {
        if (!Parse()) { return; }
        FindTilemaps();
        PaintTiles();
        PlaceObjects();
        FitCamera();
    }

    // ---------------- Map ----------------

    bool Parse()
    {
        if (mapFile == null)
        {
            Debug.LogWarning("LevelBuilder: no map file assigned.");
            return false;
        }
        List<string> rows = new List<string>();
        foreach (string line in mapFile.text.Replace("\r", "").Split('\n'))
        {
            if (line.StartsWith("//")) { continue; }
            rows.Add(line);
        }
        while (rows.Count > 0 && rows[rows.Count - 1].Trim().Length == 0) { rows.RemoveAt(rows.Count - 1); }
        while (rows.Count > 0 && rows[0].Trim().Length == 0) { rows.RemoveAt(0); }

        height = rows.Count;
        width = 0;
        foreach (string r in rows) { width = Mathf.Max(width, r.Length); }
        cells = new char[width, height];
        for (int row = 0; row < height; row++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[x, height - 1 - row] = x < rows[row].Length ? rows[row][x] : ' ';
            }
        }
        return width > 0 && height > 0;
    }

    char At(int x, int y)
    {
        return (x < 0 || y < 0 || x >= width || y >= height) ? '#' : cells[x, y];
    }

    static bool IsGround(char c) { return c == '#' || c == '=' || c == 'F'; }

    // Map coordinates (0,0 = bottom-left of the text map) to tilemap cells.
    Vector3Int Cell(int x, int y) { return new Vector3Int(x + origin.x, y + origin.y, 0); }

    void FindTilemaps()
    {
        foreach (Tilemap t in FindObjectsOfType<Tilemap>())
        {
            switch (t.gameObject.name)
            {
                case "platform tilemap": ground = t; break;
                case "Hazards": hazards = t; break;
                case "Climbing": climbing = t; break;
                case "bouncing objects": bouncing = t; break;
                case "background tilemap": background = t; break;
            }
        }
    }

    // ---------------- Tiles ----------------

    void PaintTiles()
    {
        foreach (Tilemap t in new[] { ground, hazards, climbing, bouncing, background })
        {
            if (t != null) { t.ClearAllTiles(); }
        }

        if (background != null && backgroundTiles != null && backgroundTiles.Length > 0)
        {
            List<Vector3Int> positions = new List<Vector3Int>();
            List<TileBase> tiles = new List<TileBase>();
            for (int x = -Margin; x < width + Margin; x++)
            {
                for (int y = -Margin; y < height + Margin; y++)
                {
                    Vector3Int c = Cell(x, y);
                    positions.Add(c);
                    tiles.Add(backgroundTiles[Hash(c.x, c.y) % backgroundTiles.Length]);
                }
            }
            background.SetTiles(positions.ToArray(), tiles.ToArray());
        }

        if (ground != null)
        {
            List<Vector3Int> positions = new List<Vector3Int>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (IsGround(cells[x, y])) { positions.Add(Cell(x, y)); }
                }
            }
            TileBase[] tiles = new TileBase[positions.Count];
            for (int i = 0; i < tiles.Length; i++) { tiles[i] = groundTile; }
            ground.SetTiles(positions.ToArray(), tiles);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int p = Cell(x, y);
                switch (cells[x, y])
                {
                    case '^': Spike(p, 0f); break;
                    case 'v': Spike(p, 180f); break;
                    case '>': Spike(p, -90f); break;
                    case '<': Spike(p, 90f); break;
                    case 'H': if (climbing != null) { climbing.SetTile(p, ladderTile); } break;
                    case 'M': if (bouncing != null) { bouncing.SetTile(p, mushroomTile); } break;
                    case 'w': if (hazards != null) { hazards.SetTile(p, waterTile); } break;
                    case 'W': if (hazards != null) { hazards.SetTile(p, deepWaterTile); } break;
                }
            }
        }

        // Fake floor: painted like ground (so the edges match) but with no collider.
        // If spikes or water sit right under it, paint fake ground over them too,
        // so the floor looks solid (grass on top, no gap showing the hazard).
        // The hazard is still there and still kills. Collider changes are done
        // last, because repainting a neighbour would reset them.
        if (ground != null)
        {
            List<Vector3Int> fake = new List<Vector3Int>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (cells[x, y] != '=') { continue; }
                    fake.Add(Cell(x, y));
                    char below = At(x, y - 1);
                    if (below == '^' || below == 'w' || below == 'W')
                    {
                        ground.SetTile(Cell(x, y - 1), groundTile);
                        fake.Add(Cell(x, y - 1));
                    }
                }
            }
            foreach (Vector3Int c in fake) { ground.SetColliderType(c, Tile.ColliderType.None); }
        }
    }

    void Spike(Vector3Int p, float angle)
    {
        if (hazards == null) { return; }
        hazards.SetTile(p, spikeTile);
        if (angle != 0f)
        {
            hazards.SetTileFlags(p, TileFlags.None);
            hazards.SetTransformMatrix(p, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, angle), Vector3.one));
        }
    }

    static int Hash(int x, int y)
    {
        unchecked
        {
            int h = x * 73856093 ^ y * 19349663;
            h ^= h >> 13;
            h *= 1274126177;
            return h & 0x7fffffff;
        }
    }

    // ---------------- Objects ----------------

    void PlaceObjects()
    {
        Vector3Int? exitTarget = null;
        Vector3Int? exitCell = null;
        List<Vector3> coins = new List<Vector3>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int p = Cell(x, y);
                Vector3 c = CellCenter(p);
                switch (cells[x, y])
                {
                    case 'P': PlacePlayer(c); break;
                    case 'X': exitCell = p; break;
                    case 'x': exitTarget = p; break;
                    case 'E': SpawnEnemy(c, enemySpeed); break;
                    case 'e': SpawnEnemy(c, -enemySpeed); break;
                    case 'c': coins.Add(c); break;
                    case 'p': MakePopSpike(p); break;
                    case 'D': MakeCrusher(p, x, y); break;
                    case 'I': MakeInvisibleBlock(p); break;
                }
            }
        }

        MakeCollapsingFloors();
        if (exitCell.HasValue) { PlaceExit(exitCell.Value, exitTarget); }
        SpawnCoins(coins);
    }

    Vector3 CellCenter(Vector3Int p)
    {
        Tilemap reference = ground != null ? ground : background;
        return reference != null ? reference.GetCellCenterWorld(p) : new Vector3(p.x + 0.5f, p.y + 0.5f, 0f);
    }

    void PlacePlayer(Vector3 center)
    {
        playerMovement player = FindObjectOfType<playerMovement>();
        if (player == null) { return; }
        // The player's feet are 0.5 below its pivot: stand on the cell's floor.
        player.transform.position = new Vector3(center.x, center.y + 0.02f, player.transform.position.z);
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = player.transform.position;
            rb.velocity = Vector2.zero;
        }
    }

    void PlaceExit(Vector3Int cell, Vector3Int? runTo)
    {
        exit door = FindObjectOfType<exit>();
        if (door == null) { return; }
        // The door is 2 tiles tall with its pivot in the middle: stand it on the floor.
        Vector3 c = CellCenter(cell);
        door.transform.position = new Vector3(c.x, c.y + 0.5f, door.transform.position.z);

        if (runTo.HasValue)
        {
            Vector3 to = CellCenter(runTo.Value);
            RunawayTrap run = door.gameObject.AddComponent<RunawayTrap>();
            run.SetZone(Vector2.zero, new Vector2(6f, 3f), 0f);
            run.SetMove(new Vector2(to.x - c.x, to.y - c.y), exitRunSpeed);
        }
    }

    void SpawnEnemy(Vector3 center, float speed)
    {
        if (enemyPrefab == null) { return; }
        // Enemies stand with their pivot 0.4 above the floor.
        GameObject e = Instantiate(enemyPrefab, new Vector3(center.x, center.y - 0.1f, 0f), Quaternion.identity);
        enemyMovement move = e.GetComponent<enemyMovement>();
        if (move != null) { move.SetSpeed(speed); }
    }

    void SpawnCoins(List<Vector3> positions)
    {
        if (coinPrefab == null || positions.Count == 0) { return; }

        // Screen Persist keeps collected coins gone after a death. After a
        // death there are two of them for a moment (the kept one and this
        // scene's copy, which is about to delete itself): the kept one already
        // has the coins, so don't add more.
        ScreenPersist[] persists = FindObjectsOfType<ScreenPersist>();
        if (persists.Length > 1) { return; }
        Transform parent = persists.Length == 1 ? persists[0].transform : null;

        foreach (Vector3 p in positions) { Instantiate(coinPrefab, p, Quaternion.identity, parent); }
    }

    // ---------------- Traps ----------------

    // 'p': a spike hidden in the floor that pops up when you get close.
    void MakePopSpike(Vector3Int cell)
    {
        Sprite sprite = (spikeTile as Tile) != null ? ((Tile)spikeTile).sprite : null;
        GameObject go = NewTrapObject("Pop-up Spike", CellCenter(cell), sprite, "Hazards", "Hazards");
        go.AddComponent<PolygonCollider2D>();
        PopOutTrap trap = go.AddComponent<PopOutTrap>();
        trap.SetZone(new Vector2(0f, 0.5f), new Vector2(3.2f, 2.5f), 0f);
    }

    // 'D': a block in the ceiling that drops when you walk under it.
    void MakeCrusher(Vector3Int cell, int mapX, int mapY)
    {
        Sprite sprite = GroundSpriteAt(cell);
        GameObject go = NewTrapObject("Crusher", CellCenter(cell), sprite, "Hazards", "platform");
        BoxCollider2D box = go.AddComponent<BoxCollider2D>();
        box.size = new Vector2(0.95f, 0.95f);
        DropTrap trap = go.AddComponent<DropTrap>();

        int drop = 0;
        while (mapY - drop - 1 >= 0 && !IsGround(At(mapX, mapY - drop - 1))) { drop++; }
        trap.SetZone(new Vector2(0f, -0.5f - drop / 2f), new Vector2(1.3f, Mathf.Max(1, drop)), 0f);
        trap.SetFall(crusherGravity, 4f);
    }

    // 'I': nothing there until you jump into the space below it.
    void MakeInvisibleBlock(Vector3Int cell)
    {
        if (ground == null) { return; }
        GameObject go = new GameObject("Invisible Block");
        go.transform.position = CellCenter(cell);
        TileAppear trap = go.AddComponent<TileAppear>();
        trap.Setup(ground, cell, groundTile);
        trap.SetZone(new Vector2(0f, -1f), new Vector2(0.9f, 1f), 0f);
    }

    // 'F': each horizontal run of F's falls away together when stepped on.
    void MakeCollapsingFloors()
    {
        if (ground == null) { return; }
        for (int y = 0; y < height; y++)
        {
            int x = 0;
            while (x < width)
            {
                if (cells[x, y] != 'F') { x++; continue; }
                int start = x;
                while (x < width && cells[x, y] == 'F') { x++; }
                int count = x - start;

                Vector3Int[] run = new Vector3Int[count];
                for (int i = 0; i < count; i++) { run[i] = Cell(start + i, y); }

                Vector3 left = CellCenter(run[0]);
                GameObject go = new GameObject("Collapsing Floor");
                go.transform.position = left;
                CollapsingTiles trap = go.AddComponent<CollapsingTiles>();
                trap.Setup(ground, run);
                // Zone: the strip just above the floor where the player stands.
                trap.SetZone(new Vector2((count - 1) / 2f, 1f), new Vector2(count, 1f), collapseDelay);
            }
        }
    }

    Sprite GroundSpriteAt(Vector3Int cell)
    {
        if (ground == null || groundTile == null) { return null; }
        // Paint for a moment so the rule tile picks the right edge pieces.
        ground.SetTile(cell, groundTile);
        Sprite s = ground.GetSprite(cell);
        ground.SetTile(cell, null);
        return s;
    }

    static GameObject NewTrapObject(string name, Vector3 position, Sprite sprite, string layer, string sortingLayer)
    {
        GameObject go = new GameObject(name);
        go.transform.position = position;
        int l = LayerMask.NameToLayer(layer);
        if (l >= 0) { go.layer = l; }
        SpriteRenderer r = go.AddComponent<SpriteRenderer>();
        r.sprite = sprite;
        r.sortingLayerName = sortingLayer;
        return go;
    }

    // ---------------- Camera ----------------

    void FitCamera()
    {
        if (background == null) { return; }
        PolygonCollider2D bounds = background.GetComponent<PolygonCollider2D>();
        if (bounds == null) { return; }

        Vector3 min = background.CellToWorld(Cell(0, 0));
        Vector3 max = background.CellToWorld(Cell(width, height));
        Transform t = bounds.transform;
        Vector2[] corners =
        {
            t.InverseTransformPoint(new Vector3(min.x, min.y, 0f)),
            t.InverseTransformPoint(new Vector3(max.x, min.y, 0f)),
            t.InverseTransformPoint(new Vector3(max.x, max.y, 0f)),
            t.InverseTransformPoint(new Vector3(min.x, max.y, 0f)),
        };
        bounds.pathCount = 1;
        bounds.SetPath(0, corners);

        foreach (CinemachineConfiner confiner in FindObjectsOfType<CinemachineConfiner>())
        {
            confiner.m_BoundingShape2D = bounds;
            confiner.InvalidatePathCache();
        }
    }

    // ---------------- Editor ----------------

    [ContextMenu("Preview map (paint tiles only)")]
    void PreviewInEditor()
    {
        if (!Parse()) { return; }
        FindTilemaps();
        PaintTiles();
    }
}
