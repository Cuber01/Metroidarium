using Godot;

namespace Metroidarium;

public class TilemapToolsComponent : Component
{
    private const int DestructibleTerrainId = 2;
    private const float NormalStrengthOffset = 0.1f;

    private const string HoleLayerPath = "../Hole";
    
    public bool CheckTryDestroy(KinematicCollision2D col, TileMapLayer layer)
    {
        Vector2I mapCoords = movingLocalToMap(col.GetPosition(), col.GetNormal(), layer);
        TileData data = layer.GetCellTileData(mapCoords);
        if (data is { Terrain: DestructibleTerrainId })
        {
            destroy(mapCoords, layer);
            return true;
        }
        // Failed to find tile or tile is not destructible
        return false;    
    }

    public int GetTileTerrain(Vector2 localPosition, TileMapLayer layer)
    {
        return layer.GetCellTileData(layer.LocalToMap(localPosition)).Terrain;
    }
    
    public bool IsDestructible(KinematicCollision2D col, TileMapLayer layer)
    {
        Vector2I mapCoords = movingLocalToMap(col.GetPosition(), col.GetNormal(), layer);
        TileData data = layer.GetCellTileData(mapCoords);
        return data.Terrain == DestructibleTerrainId;
    }
    
    public bool IsDestructible(Vector2 position, TileMapLayer layer)
    {
        Vector2I mapCoords = layer.LocalToMap(position);
        TileData data = layer.GetCellTileData(mapCoords);
        return data.Terrain == DestructibleTerrainId;
    }

    public bool IsHole(Entity actor, Vector2 position)
    {
        TileMapLayer holeLayer = actor.GetNodeOrNull<TileMapLayer>(HoleLayerPath);
        if (holeLayer == null)
        {
            return false;
        }
        
        Vector2I mapCoords = holeLayer.LocalToMap(position);
        return holeLayer.GetCellTileData(mapCoords) != null;
    }

    private void destroy(Vector2I mapCoords, TileMapLayer layer)
    {
        layer.SetCell(mapCoords, -1);
    }

    // Takes into consideration body's movement when calculating LocalToMap
    private Vector2I movingLocalToMap(Vector2 colPosition, Vector2 colNormal, TileMapLayer layer)
    {
        return layer.LocalToMap(colPosition - colNormal * NormalStrengthOffset);
    }
}