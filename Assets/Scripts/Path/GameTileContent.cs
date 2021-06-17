using UnityEngine;

[SelectionBase]
public class GameTileContent : MonoBehaviour
{
    GameTileContentFactory originFactory;

    public GameTileContentFactory OriginFactory {
        get => originFactory;
        set {
            Debug.Assert( originFactory == null, "Redefined origin factory!" );
            originFactory = value;
        }
    }

    [SerializeField]
    GameTileContentType type = default;

    public GameTileContentType Type => type;

    public bool BlocksPath => type == GameTileContentType.Wall || type == GameTileContentType.Tower;

    public virtual void GameUpdate() { }

    public void Recycle() {
        originFactory.Reclaim( this );
    }
}
