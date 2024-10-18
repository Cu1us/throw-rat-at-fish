using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Dungeon tile")]
public class DungeonTile : TileBase
{
    public GameObject prefab;
    public bool Walkable = true;
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.gameObject = prefab;
    }
}
