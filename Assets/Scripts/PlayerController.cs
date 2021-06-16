using Soultia.Util;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //视线范围
    public int viewRange = 1;

    public LayerMask groundLayer;

    float maxDist = 40;


    void Update() {
        CreateChunk();
        InputCheck();
    }

    private void InputCheck() {
        bool leftClick = Input.GetMouseButtonDown( 0 );
        if ( leftClick ) {
            RaycastHit hitInfo;
            if ( Physics.Raycast( transform.position, new Vector3( 0, -1, 0 ), out hitInfo, maxDist, groundLayer ) ) {
                Vector3 pointInTargetBlock;

                pointInTargetBlock = hitInfo.point - transform.forward * .01f;

                Chunk tc = Map.instance.GetChunk( Vector3i.Floor( pointInTargetBlock ) );

                if ( tc != null ) {
                    if ( leftClick ) {
                        for ( int i = 0; i < Chunk.width; ++i ) {
                            for ( int j = 0; j < Chunk.width; ++j ) {
                                tc.blocks[i, Chunk.height - 1, j] = (byte)BlockType.grass;
                            }
                        }
                        tc.RebuildMesh();
                    }
                }
            }
        }
    }

    private void CreateChunk() {
        for ( float x = transform.position.x - viewRange; x < transform.position.x + viewRange; x += Chunk.width ) {
            //for ( float y = transform.position.y - viewRange; y < transform.position.y + viewRange; y += Chunk.height ) {
            //if ( y <= Chunk.height * 1 && y > 0 ) {
            for ( float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z += Chunk.width ) {
                int xx = Chunk.width * Mathf.CeilToInt( x / Chunk.width );
                int yy = 0;//Chunk.height * Mathf.FloorToInt( y / Chunk.height );
                int zz = Chunk.width * Mathf.CeilToInt( z / Chunk.width );

                if ( !Map.instance.ChunkExists( xx, yy, zz ) ) {
                    Map.instance.CreateChunk( new Vector3i( xx, yy, zz ) );
                }
            }
            //}
            //}
        }
    }
}
