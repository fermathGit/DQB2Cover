using UnityEngine;

public class Enemy : GameBehavier
{
    EnemyFactory originFactory;

    GameTile tileFrom, tileTo;

    Vector3 positionFrom, positionTo;

    Direction direction;

    DirectionChange directionChange;

    float directionAngleFrom, directionAngleTo;

    float progress, progressFactor;

    [SerializeField]
    Transform model = default;

    public float Scale { get; private set; }

    float pathOffset;

    float speed;

    float Health { get; set; }

    public EnemyFactory OriginFactory {
        get => originFactory;
        set {
            Debug.Assert( originFactory == null, "Redefined origin factory!" );
            originFactory = value;
        }
    }

    private void Awake() {

    }

    public void Initialize( float scale, float speed, float pathOffset, float health ) {
        Scale = scale;
        model.localScale = Vector3.one * scale;
        this.speed = speed;
        this.pathOffset = pathOffset;
        Health = health;

    }

    public void SpawnOn( GameTile tile ) {
        Debug.Assert( tile.NextOnPath != null, "Nowhere to go!", this );
        tileFrom = tile;
        tileTo = tile.NextOnPath;
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileTo.transform.localPosition;
        progress = 0f;
        //PrepareIntro();
    }

    public override void Recycle() {
        OriginFactory.Reclaim( this );
    }

    public override bool GameUpdate() {
        progress += Time.deltaTime ;
        while ( progress >= 1f ) {
            tileFrom = tileTo;
            tileTo = tileTo.NextOnPath;
            if ( tileTo == null ) {
                OriginFactory.Reclaim( this );
                return false;
            }
            positionFrom = positionTo;
            positionTo = tileTo.transform.localPosition;
            progress -= 1f;
        }
        transform.localPosition = Vector3.LerpUnclamped( positionFrom, positionTo, progress );
        return true;
    }

    private void OnDestroy() {
    }

    void PrepareIntro() {
        positionFrom = tileFrom.transform.localPosition;
        transform.localPosition = positionFrom;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3( pathOffset, 0f );
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }

    void PrepareNextState() {
        tileFrom = tileTo;
        tileTo = tileTo.NextOnPath;
        positionFrom = positionTo;
        if ( tileTo == null ) {
            PrepareOutro();
            return;
        }
        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChangeTo( tileFrom.PathDirection );
        direction = tileFrom.PathDirection;
        directionAngleFrom = directionAngleTo;
        switch ( directionChange ) {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
    }

    void PrepareForward() {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3( pathOffset, 0f );
        progressFactor = speed;
    }

    void PrepareTurnRight() {
        directionAngleTo = directionAngleFrom + 90f;
        model.localPosition = new Vector3( pathOffset - 0.5f, 0f );
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = speed / ( Mathf.PI * 0.5f * ( 0.5f - pathOffset ) );
    }

    void PrepareTurnLeft() {
        directionAngleTo = directionAngleFrom - 90f;
        model.localPosition = new Vector3( pathOffset + 0.5f, 0f );
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = speed / ( Mathf.PI * 0.5f * ( 0.5f + pathOffset ) );
    }

    void PrepareTurnAround() {
        directionAngleTo = directionAngleFrom + ( pathOffset < 0 ? 180f : -180f );
        model.localPosition = new Vector3( pathOffset, 0f );
        transform.localPosition = positionFrom;
        progressFactor = speed / ( Mathf.PI * Mathf.Max( Mathf.Abs( pathOffset ), 0.2f ) );
    }

    void PrepareOutro() {
        positionTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3( pathOffset, 0f );
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }

    public void ApplyDemage( float demage ) {
        Health -= demage;
    }

}
