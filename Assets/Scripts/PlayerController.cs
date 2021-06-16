using Soultia.Util;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //视线范围
    public int viewRange = 1;

    public LayerMask groundLayer;

    float maxDist = 40;

   [SerializeField] GameObject _createGo;

    void Update() {
        CreateChunk();
        InputCheck();
    }

    private void InputCheck() {
        bool rightClick = Input.GetMouseButtonDown( 1 );
        if ( rightClick ) {
            RaycastHit hitInfo;
            if ( Physics.Raycast( transform.position, new Vector3( 0, -1, 0 ), out hitInfo, maxDist, groundLayer ) ) {
                Vector3 pointInTargetBlock;

                pointInTargetBlock = hitInfo.point - transform.forward * .01f;

                Chunk tc = Map.instance.GetChunk( Vector3i.Floor( pointInTargetBlock ) );

                if ( tc != null ) {
                    for ( int i = 0; i < Chunk.width; ++i ) {
                        for ( int j = 0; j < Chunk.width; ++j ) {
                            tc.blocks[i, 0, j] = (byte)BlockType.grass;
                        }
                    }
                    tc.RebuildMesh();
                }
            }
        }

        bool leftClick = Input.GetMouseButtonDown( 0 );
        if ( leftClick ) {
            Vector3i pos = GetCreatePos();

            Chunk tc = Map.instance.GetChunk( pos );
            if ( tc != null ) {
                tc.SetBlock( pos, (byte)BlockType.dirt );
                tc.RebuildMesh();
            }
        }

        _createGo.transform.position = GetCreatePos();
    }

    private Vector3i GetCreatePos() {
        Vector3i pos = Vector3i.Round( transform.position );
        Vector3i offset = Vector3i.Round( transform.forward.normalized );
        pos += new Vector3i( offset.x, 0, offset.z );
        return pos;
    }

    private void CreateChunk() {
        for ( float x = transform.position.x - viewRange; x < transform.position.x + viewRange; x += Chunk.width ) {
            //for ( float y = transform.position.y - viewRange; y < transform.position.y + viewRange; y += Chunk.height ) {
            //if ( y <= Chunk.height * 1 && y > 0 ) {
            for ( float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z += Chunk.width ) {
                int xx = Chunk.width * Mathf.FloorToInt( x / Chunk.width );
                int yy = 0;//Chunk.height * Mathf.FloorToInt( y / Chunk.height );
                int zz = Chunk.width * Mathf.FloorToInt( z / Chunk.width );

                if ( !Map.instance.ChunkExists( xx, yy, zz ) ) {
                    Map.instance.CreateChunk( new Vector3i( xx, yy, zz ) );
                }
            }
            //}
            //}
        }
    }
}
