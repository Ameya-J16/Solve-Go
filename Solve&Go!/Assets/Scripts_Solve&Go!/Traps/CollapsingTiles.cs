using UnityEngine;
using UnityEngine.Tilemaps;

// Ground tiles that fall away when the player steps onto them. The tiles look
// exactly like the floor around them until they go. Created by LevelBuilder
// for each run of 'F' in a map.
public class CollapsingTiles : TrollTrap
{
    Tilemap map;
    Vector3Int[] cells;

    public void Setup(Tilemap tilemap, Vector3Int[] tiles)
    {
        map = tilemap;
        cells = tiles;
    }

    protected override void Fire()
    {
        if (map == null || cells == null) { return; }

        // Grab the sprites first: removing a tile changes its neighbours' look.
        Sprite[] sprites = new Sprite[cells.Length];
        for (int i = 0; i < cells.Length; i++) { sprites[i] = map.GetSprite(cells[i]); }

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 position = map.GetCellCenterWorld(cells[i]);
            map.SetTile(cells[i], null);
            SpawnDebris(sprites[i], position, i);
        }
    }

    static void SpawnDebris(Sprite sprite, Vector3 position, int index)
    {
        if (sprite == null) { return; }
        GameObject debris = new GameObject("Falling Floor");
        debris.transform.position = position;
        SpriteRenderer r = debris.AddComponent<SpriteRenderer>();
        r.sprite = sprite;
        r.sortingLayerName = "platform";
        Rigidbody2D rb = debris.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.angularVelocity = (index % 2 == 0 ? 1f : -1f) * Random.Range(40f, 120f);
        Destroy(debris, 2.5f);
    }
}
