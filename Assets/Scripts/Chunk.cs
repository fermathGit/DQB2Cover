using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soultia.Util;

[RequireComponent( typeof( MeshRenderer ) )]
[RequireComponent( typeof( MeshFilter ) )]
[RequireComponent( typeof( MeshCollider ) )]
public class Chunk : MonoBehaviour
{
    public static int width = 16;
    public static int height = 3;

    public byte[,,] blocks;
    public Vector3i position;

    private Mesh _mesh;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uv = new List<Vector2>();
    public static float textureOffset = 1 / 32f;
    public static float shrinkSize = 0.001f;

    public bool IsWorking = false;
    private bool _isFinished = false;

    // Start is called before the first frame update
    void Start() {
        position = new Vector3i( this.transform.position );
        if ( Map.instance.ChunkExists( position ) ) {
            Debug.Log( "此方块已存在" + position );
            Destroy( this );
        } else {
            Map.instance.chunks.Add( position, this.gameObject );
            this.name = "(" + position.x + "," + position.y + "," + position.z + ")";
            //StartFuncion();
        }
    }

    private void Update() {
        if ( IsWorking == false && _isFinished == false ) {
            _isFinished = true;
            StartFuncion();
        }
    }

    private void StartFuncion() {
        IsWorking = true;
        _mesh = new Mesh();
        _mesh.name = "Chunk";

        StartCoroutine( CreateMap() );
    }

    IEnumerator CreateMap() {
        blocks = new byte[width, height, width];
        for ( int x = 0; x < Chunk.width; x++ ) {
            for ( int y = 0; y < Chunk.height; y++ ) {
                for ( int z = 0; z < Chunk.width; z++ ) {
                    if ( y == Chunk.height - 1 ) {
                        if ( UnityEngine.Random.Range( 1, 1 ) == 1 ) {
                            blocks[x, y, z] = (byte)BlockType.rock;
                        }
                    } else if ( x == Chunk.width - 1 || x == 0 || z == Chunk.width - 1 || z == 0 ) {
                        if ( UnityEngine.Random.Range( 1, 1 ) == 1 ) {
                            blocks[x, y, z] = (byte)BlockType.rock;
                        }
                    } else {
                        blocks[x, y, z] = (byte)BlockType.dirt;
                    }
                    //byte blockId = Terrain.GetTerrainBlock( new Vector3i( x, y, z ) + position );
                    //byte blockId_up = Terrain.GetTerrainBlock( new Vector3i( x, y + 1, z ) + position );
                    //if ( blockId == 1 && blockId_up == 0 ) {
                    //    blocks[x, y, z] = 3;
                    //} else {
                    //    blocks[x, y, z] = blockId;
                    //}
                }
            }
        }

        yield return null;
        StartCoroutine( CreateMesh() );
    }

    public void RebuildMesh() {
        StartCoroutine( CreateMesh() );
    }

    IEnumerator CreateMesh() {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();
        //把所有面的点和面的索引添加进去
        for ( int x = 0; x < Chunk.width; x++ ) {
            for ( int y = 0; y < Chunk.height; y++ ) {
                for ( int z = 0; z < Chunk.width; z++ ) {
                    Block block = BlockList.GetBlock( this.blocks[x, y, z] );
                    if ( block == null ) continue;

                    if ( IsBlockTransparent( x + 1, y, z ) ) {
                        AddFace_Front( x, y, z, block );
                    }
                    if ( IsBlockTransparent( x - 1, y, z ) ) {
                        AddFace_Back( x, y, z, block );
                    }
                    if ( IsBlockTransparent( x, y, z + 1 ) ) {
                        AddFace_Right( x, y, z, block );
                    }
                    if ( IsBlockTransparent( x, y, z - 1 ) ) {
                        AddFace_Left( x, y, z, block );
                    }
                    if ( IsBlockTransparent( x, y + 1, z ) ) {
                        AddFace_Top( x, y, z, block );
                    }
                    if ( IsBlockTransparent( x, y - 1, z ) ) {
                        AddFace_Bottom( x, y, z, block );
                    }
                }
            }
        }


        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
        _mesh.uv = uv.ToArray();

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = _mesh;
        GetComponent<MeshCollider>().sharedMesh = _mesh;

        yield return null;
        IsWorking = false;
    }

    public bool IsBlockTransparent( int x, int y, int z ) {
        if ( x >= width || y >= height || z >= width || x < 0 || y < 0 || z < 0 ) {
            return true;
        } else {
            return blocks[x, y, z] == (byte)BlockType.air;
        }
        return false;
    }

    private void AddFace_Front( int x, int y, int z, Block block ) {
        //第一个三角面
        triangles.Add( 0 + vertices.Count );
        triangles.Add( 3 + vertices.Count );
        triangles.Add( 2 + vertices.Count );

        //第二个三角面
        triangles.Add( 2 + vertices.Count );
        triangles.Add( 1 + vertices.Count );
        triangles.Add( 0 + vertices.Count );


        //添加4个点
        vertices.Add( new Vector3( 0 + x, 0 + y, 0 + z ) );
        vertices.Add( new Vector3( 0 + x, 0 + y, 1 + z ) );
        vertices.Add( new Vector3( 0 + x, 1 + y, 1 + z ) );
        vertices.Add( new Vector3( 0 + x, 1 + y, 0 + z ) );

        //添加UV坐标点，跟上面4个点循环的顺序一致
        uv.Add( new Vector2( block.textureFrontX * textureOffset, block.textureFrontY * textureOffset ) + new Vector2( shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset ) + new Vector2( -shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset + textureOffset ) + new Vector2( -shrinkSize, -shrinkSize ) );
        uv.Add( new Vector2( block.textureFrontX * textureOffset, block.textureFrontY * textureOffset + textureOffset ) + new Vector2( shrinkSize, -shrinkSize ) );
    }

    private void AddFace_Back( int x, int y, int z, Block block ) {
        //第一个三角面
        triangles.Add( 0 + vertices.Count );
        triangles.Add( 3 + vertices.Count );
        triangles.Add( 2 + vertices.Count );

        //第二个三角面
        triangles.Add( 2 + vertices.Count );
        triangles.Add( 1 + vertices.Count );
        triangles.Add( 0 + vertices.Count );


        //添加4个点
        vertices.Add( new Vector3( -1 + x, 0 + y, 1 + z ) );
        vertices.Add( new Vector3( -1 + x, 0 + y, 0 + z ) );
        vertices.Add( new Vector3( -1 + x, 1 + y, 0 + z ) );
        vertices.Add( new Vector3( -1 + x, 1 + y, 1 + z ) );

        //添加UV坐标点，跟上面4个点循环的顺序一致
        uv.Add( new Vector2( block.textureBackX * textureOffset, block.textureBackY * textureOffset ) + new Vector2( shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset ) + new Vector2( -shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset + textureOffset ) + new Vector2( -shrinkSize, -shrinkSize ) );
        uv.Add( new Vector2( block.textureBackX * textureOffset, block.textureBackY * textureOffset + textureOffset ) + new Vector2( shrinkSize, -shrinkSize ) );
    }

    private void AddFace_Right( int x, int y, int z, Block block ) {
        //第一个三角面
        triangles.Add( 0 + vertices.Count );
        triangles.Add( 3 + vertices.Count );
        triangles.Add( 2 + vertices.Count );

        //第二个三角面
        triangles.Add( 2 + vertices.Count );
        triangles.Add( 1 + vertices.Count );
        triangles.Add( 0 + vertices.Count );


        //添加4个点
        vertices.Add( new Vector3( 0 + x, 0 + y, 1 + z ) );
        vertices.Add( new Vector3( -1 + x, 0 + y, 1 + z ) );
        vertices.Add( new Vector3( -1 + x, 1 + y, 1 + z ) );
        vertices.Add( new Vector3( 0 + x, 1 + y, 1 + z ) );

        //添加UV坐标点，跟上面4个点循环的顺序一致
        uv.Add( new Vector2( block.textureRightX * textureOffset, block.textureRightY * textureOffset ) + new Vector2( shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset ) + new Vector2( -shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset + textureOffset ) + new Vector2( -shrinkSize, -shrinkSize ) );
        uv.Add( new Vector2( block.textureRightX * textureOffset, block.textureRightY * textureOffset + textureOffset ) + new Vector2( shrinkSize, -shrinkSize ) );
    }

    private void AddFace_Left( int x, int y, int z, Block block ) {
        //第一个三角面
        triangles.Add( 0 + vertices.Count );
        triangles.Add( 3 + vertices.Count );
        triangles.Add( 2 + vertices.Count );

        //第二个三角面
        triangles.Add( 2 + vertices.Count );
        triangles.Add( 1 + vertices.Count );
        triangles.Add( 0 + vertices.Count );


        //添加4个点
        vertices.Add( new Vector3( -1 + x, 0 + y, 0 + z ) );
        vertices.Add( new Vector3( 0 + x, 0 + y, 0 + z ) );
        vertices.Add( new Vector3( 0 + x, 1 + y, 0 + z ) );
        vertices.Add( new Vector3( -1 + x, 1 + y, 0 + z ) );

        //添加UV坐标点，跟上面4个点循环的顺序一致
        uv.Add( new Vector2( block.textureLeftX * textureOffset, block.textureLeftY * textureOffset ) + new Vector2( shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset ) + new Vector2( -shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset + textureOffset ) + new Vector2( -shrinkSize, -shrinkSize ) );
        uv.Add( new Vector2( block.textureLeftX * textureOffset, block.textureLeftY * textureOffset + textureOffset ) + new Vector2( shrinkSize, -shrinkSize ) );
    }

    private void AddFace_Top( int x, int y, int z, Block block ) {
        //第一个三角面
        triangles.Add( 1 + vertices.Count );
        triangles.Add( 0 + vertices.Count );
        triangles.Add( 3 + vertices.Count );

        //第二个三角面
        triangles.Add( 3 + vertices.Count );
        triangles.Add( 2 + vertices.Count );
        triangles.Add( 1 + vertices.Count );


        //添加4个点
        vertices.Add( new Vector3( 0 + x, 1 + y, 0 + z ) );
        vertices.Add( new Vector3( 0 + x, 1 + y, 1 + z ) );
        vertices.Add( new Vector3( -1 + x, 1 + y, 1 + z ) );
        vertices.Add( new Vector3( -1 + x, 1 + y, 0 + z ) );

        //添加UV坐标点，跟上面4个点循环的顺序一致
        uv.Add( new Vector2( block.textureTopX * textureOffset, block.textureTopY * textureOffset ) + new Vector2( shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset ) + new Vector2( -shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset + textureOffset ) + new Vector2( -shrinkSize, -shrinkSize ) );
        uv.Add( new Vector2( block.textureTopX * textureOffset, block.textureTopY * textureOffset + textureOffset ) + new Vector2( shrinkSize, -shrinkSize ) );
    }

    private void AddFace_Bottom( int x, int y, int z, Block block ) {
        //第一个三角面
        triangles.Add( 1 + vertices.Count );
        triangles.Add( 0 + vertices.Count );
        triangles.Add( 3 + vertices.Count );

        //第二个三角面
        triangles.Add( 3 + vertices.Count );
        triangles.Add( 2 + vertices.Count );
        triangles.Add( 1 + vertices.Count );


        //添加4个点
        vertices.Add( new Vector3( -1 + x, 0 + y, 0 + z ) );
        vertices.Add( new Vector3( -1 + x, 0 + y, 1 + z ) );
        vertices.Add( new Vector3( 0 + x, 0 + y, 1 + z ) );
        vertices.Add( new Vector3( 0 + x, 0 + y, 0 + z ) );

        //添加UV坐标点，跟上面4个点循环的顺序一致
        uv.Add( new Vector2( block.textureBottomX * textureOffset, block.textureBottomY * textureOffset ) + new Vector2( shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset ) + new Vector2( -shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset + textureOffset ) + new Vector2( -shrinkSize, -shrinkSize ) );
        uv.Add( new Vector2( block.textureBottomX * textureOffset, block.textureBottomY * textureOffset + textureOffset ) + new Vector2( shrinkSize, -shrinkSize ) );
    }
}
