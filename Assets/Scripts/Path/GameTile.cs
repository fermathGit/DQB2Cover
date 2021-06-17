using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    Transform arrow = default;

    GameTile north, east, south, west, nextOnPath;

    int distance;

    public bool HasPath => distance != int.MaxValue;

    public GameTile GrowPathNorth() => GrowToPath( north, Direction.South );

    public GameTile GrowPathEast() => GrowToPath( east, Direction.West );

    public GameTile GrowPathSouth() => GrowToPath( south, Direction.North );

    public GameTile GrowPathWest() => GrowToPath( west, Direction.East );

    public GameTile NextOnPath => nextOnPath;

    public Vector3 ExitPoint { get; private set; }

    public bool IsAlternative { get; set; }

    public Direction PathDirection { get; private set; }

    GameTileContent content;

    public GameTileContent Content {
        get => content;
        set {
            Debug.Assert( value != null, "Null assigned to content!" );
            if ( content != null ) {
                content.Recycle();
            }
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

    static Quaternion
      northRotation = Quaternion.Euler( 90f, 0f, 0f ),
      eastRotation = Quaternion.Euler( 90f, 90f, 0f ),
      southRotation = Quaternion.Euler( 90f, 180f, 0f ),
      westRotation = Quaternion.Euler( 90f, 270f, 0f );

    public static void MakeEastWestNeighbors( GameTile east, GameTile west ) {
        Debug.Assert( west.east == null && east.west == null, "Redefined neighbors WE" );
        west.east = east;
        east.west = west;
    }

    public static void MakeNorthSouthNeighbors( GameTile north, GameTile south ) {
        Debug.Assert( north.south == null && south.north == null, "Redefined neighbors NS" );
        north.south = south;
        south.north = north;
    }

    public void ClearPath() {
        nextOnPath = null;
        distance = int.MaxValue;
    }

    public void BecomeDestination() {
        distance = 0;
        nextOnPath = null;
        ExitPoint = transform.localPosition;
    }

    GameTile GrowToPath( GameTile neighbor, Direction direction ) {
        if ( !HasPath || neighbor == null || neighbor.HasPath ) return null;
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();
        neighbor.PathDirection = direction;
        return neighbor.Content.BlocksPath ? null : neighbor;
    }

    public void ShowPath() {
        if ( distance == 0 ) {
            arrow.gameObject.SetActive( false );
            return;
        }
        arrow.gameObject.SetActive( true );
        arrow.localRotation =
          nextOnPath == north ? northRotation :
          nextOnPath == east ? eastRotation :
          nextOnPath == south ? southRotation :
          westRotation;
    }

    public void HidePath() {
        arrow.gameObject.SetActive( false );
    }
}
