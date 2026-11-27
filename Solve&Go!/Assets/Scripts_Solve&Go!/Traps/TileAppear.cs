using UnityEngine;
using UnityEngine.Tilemaps;

// An invisible block: nothing is there until the player jumps into the space
// under it, then a solid ground tile appears and they bonk their head on it.
// Created by LevelBuilder for each 'I' in a map.
public class TileAppear : TrollTrap
{
    Tilemap map;
    Vector3Int cell;
    TileBase tile;

    public void Setup(Tilemap tilemap, Vector3Int position, TileBase groundTile)
    {
        map = tilemap;
        cell = position;
        tile = groundTile;
    }

    protected override void Fire()
    {
        if (map != null && tile != null) { map.SetTile(cell, tile); }
    }
}
