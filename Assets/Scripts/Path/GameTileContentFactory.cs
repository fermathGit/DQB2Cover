using UnityEngine;

[CreateAssetMenu]
public class GameTileContentFactory : GameObjectFactory
{
    [SerializeField]
    GameTileContent destinationPrefab = default;

    [SerializeField]
    GameTileContent emptyPrefab = default;

    [SerializeField]
    GameTileContent wallPrefab = default;

    [SerializeField]
    GameTileContent spawnPointPrefab = default;


    public void Reclaim( GameTileContent content ) {
        Destroy( content.gameObject );
    }

    T Get<T>( T prefab ) where T : GameTileContent {
        T instance = CreateGameObjectInstance( prefab );
        instance.OriginFactory = this;
        return instance;
    }

    public GameTileContent Get( GameTileContentType type ) {
        switch ( type ) {
            case GameTileContentType.Destination: return Get( destinationPrefab );
            case GameTileContentType.Empty: return Get( emptyPrefab );
            case GameTileContentType.Wall: return Get( wallPrefab );
            case GameTileContentType.SpawnPoint: return Get( spawnPointPrefab );
        }
        Debug.Assert( false, "Unsupported type: " + type );
        return null;
    }

}
