using Soultia.Util;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //视线范围
    public int viewRange = 30;


    void Update() {
        for ( float x = transform.position.x - viewRange; x < transform.position.x + viewRange; x += Chunk.width ) {
            for ( float y = transform.position.y - viewRange; y < transform.position.y + viewRange; y += Chunk.height ) {
                if ( y <= Chunk.height * 1 && y > 0 ) {
                    for ( float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z += Chunk.width ) {
                        int xx = Chunk.width * Mathf.FloorToInt( x / Chunk.width );
                        int yy = Chunk.height * Mathf.FloorToInt( y / Chunk.height );
                        int zz = Chunk.width * Mathf.FloorToInt( z / Chunk.width );
                        if ( !Map.instance.ChunkExists( xx, yy, zz ) ) {
                            Map.instance.CreateChunk( new Vector3i( xx, yy, zz ) );
                        }
                    }
                }
            }
        }
    }
}
