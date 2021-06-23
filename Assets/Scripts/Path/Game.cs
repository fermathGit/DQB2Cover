using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    Vector2Int boardSize = new Vector2Int( 11, 11 );

    [SerializeField]
    GameBoard board = default;

    [SerializeField]
    GameTileContentFactory tileContentFactory = default;

    GameBehavierCollection enemies = new GameBehavierCollection();

    Ray TouchRay => Camera.main.ScreenPointToRay( Input.mousePosition );

    [SerializeField, Range( 0, 100 )]
    int startingPlayerHealth = 10;

    int playerHealth;

    const float pausedTimeScale = 0f;

    [SerializeField, Range( 1f, 10f )]
    float playSpeed = 1f;

    static Game instance;

    [SerializeField] EnemyFactory enemyFactory = default;

    [SerializeField, Range( 0.1f, 10f )] float spawnSpeed = 1f;

    float spawnProgress;

    private void Awake() {
        playerHealth = startingPlayerHealth;
        board.Initialize( boardSize, tileContentFactory );
        board.ShowGrid = true;
    }

    private void OnEnable() {
        instance = this;
    }

    private void OnValidate() {
        if ( boardSize.x < 2 )
            boardSize.x = 2;
        if ( boardSize.y < 2 )
            boardSize.y = 2;
    }

    private void Update() {
        if ( Input.GetMouseButtonDown( 0 ) ) {
            HandleTouch();
        } else if ( Input.GetMouseButtonDown( 1 ) ) {
            HandleAlternativeTouch();
        }

        if ( Input.GetKeyDown( KeyCode.V ) ) {
            board.ShowPaths = !board.ShowPaths;
        }
        if ( Input.GetKeyDown( KeyCode.G ) ) {
            board.ShowGrid = !board.ShowGrid;
        }

        if ( Input.GetKeyDown( KeyCode.Space ) ) {
            Time.timeScale = Time.timeScale > pausedTimeScale ? pausedTimeScale : 1f;
        } else if ( Time.timeScale > pausedTimeScale ) {
            Time.timeScale = playSpeed;
        }

        if ( Input.GetKeyDown( KeyCode.B ) ) {
            BeginNewGame();
        }

        enemies.GameUpdate();
        board.GameUpdate();

        spawnProgress += spawnSpeed * Time.deltaTime;
        while ( spawnProgress >= 3f ) {
            spawnProgress -= 3f;
            SpawnEnemy();
        }
    }

    void BeginNewGame() {
        playerHealth = startingPlayerHealth;
        enemies.Clear();
        board.Clear();
        //activeScenario = scenario.Begin();
    }

    void HandleTouch() {
        GameTile tile = board.GetTile( TouchRay );
        if ( tile != null ) {
            if ( Input.GetKey( KeyCode.LeftShift ) ) {

            } else {
                board.ToggleWall( tile );
            }
        }
    }

    void HandleAlternativeTouch() {
        GameTile tile = board.GetTile( TouchRay );
        if ( tile != null ) {
            if ( Input.GetKey( KeyCode.LeftShift ) )
                board.ToggleDestination( tile );
            else
                board.ToggleSpawnPoint( tile );
        }
    }

    public void SpawnEnemy( ) {
        GameTile spawnPoint = instance.board.GetSpawnPoint( Random.Range( 0, instance.board.SpawnPointCount ) );
        Enemy enemy = enemyFactory.Get( EnemyType.Large );
        enemy.SpawnOn( spawnPoint );
        instance.enemies.Add( enemy );
    }

}
