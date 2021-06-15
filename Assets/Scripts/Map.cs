using Soultia.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Map : MonoBehaviour {
    public static Map instance;

    public static GameObject chunkPrefab;

    public Dictionary<Vector3i, GameObject> chunks = new Dictionary<Vector3i, GameObject>();

    private bool spawningChunk = false;

    private void Awake() {
        instance = this;
        chunkPrefab = Resources.Load( "Prefab/Chunk" ) as GameObject;
    }

    //private void Update() {
    //    for ( int i = 0; i <= 5; ++i ) {
    //        for ( int j = 0; j <= 5; ++j ) {
    //            Vector3i pos = new Vector3i( i * Chunk.width, 0, j * Chunk.width );
    //            if ( !ChunkExists( pos ) ) {
    //                CreateChunk( pos );
    //            }
    //        }
    //    }
    //}

    public void CreateChunk( Vector3i pos ) {
        if ( spawningChunk ) return;

        StartCoroutine( SpawnChunk( pos ) );
    }

    IEnumerator SpawnChunk( Vector3i pos ) {
        spawningChunk = true;
        Instantiate( chunkPrefab, pos, Quaternion.identity );
        yield return null;
        spawningChunk = false;
    }

    //通过Chunk的坐标来判断它是否存在
    public bool ChunkExists( Vector3i worldPosition ) {
        return this.ChunkExists( worldPosition.x, worldPosition.y, worldPosition.z );
    }

    //通过Chunk的坐标来判断它是否存在
    public bool ChunkExists( int x, int y, int z ) {
        return chunks.ContainsKey( new Vector3i( x, y, z ) );
    }

    //通过世界坐标获取Chunk对象
    public Chunk GetChunk( Vector3i worldPosition ) {
        int xx = Chunk.width * Mathf.FloorToInt( 1.0f * worldPosition.x / Chunk.width );
        int yy = Chunk.height * Mathf.FloorToInt( 1.0f * worldPosition.y / Chunk.height );
        int zz = Chunk.width * Mathf.FloorToInt( 1.0f * worldPosition.z / Chunk.width );
        if ( Map.instance.chunks.ContainsKey( new Vector3i( xx, yy, zz ) ) ) {
            return Map.instance.chunks[new Vector3i( xx, yy, zz )].GetComponent<Chunk>();
        }
        return null;
    }
}
