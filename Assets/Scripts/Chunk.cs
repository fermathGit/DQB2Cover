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
    public static int height = 16;

    public byte[,,] blocks;
    public Vector3i position;

    private Mesh _mesh;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uv = new List<Vector2>();
    public static float textureOffset = 1 / 32f;
    public static float shrinkSize = 0.001f;

    private bool _isWorking = false;

    // Start is called before the first frame update
    void Start() {
        position = new Vector3i( this.transform.position );
        if ( Map.instance.ChunkExists( position ) ) {
            Debug.Log( "�˷����Ѵ���" + position );
            Destroy( this );
        } else {
            Map.instance.chunks.Add( position, this.gameObject );
            this.name = "(" + position.x + "," + position.y + "," + position.z + ")";
            StartFuncion();
        }
    }

    private void StartFuncion() {
        _mesh = new Mesh();
        _mesh.name = "Chunk";

        StartCoroutine( CreateMap() );
    }

    IEnumerator CreateMap() {
        while ( _isWorking ) {
            yield return null;
        }
        _isWorking = true;
        blocks = new byte[width, height, width];
        for ( int x = 0; x < Chunk.width; x++ ) {
            for ( int y = 0; y < Chunk.height; y++ ) {
                for ( int z = 0; z < Chunk.width; z++ ) {
                    if ( y == Chunk.height - 1 ) {
                        if ( UnityEngine.Random.Range( 1, 5 ) == 1 ) {
                            blocks[x, y, z] = 2;
                        }
                    } else if ( x == Chunk.width - 1 || x == 0 || z == Chunk.width - 1 || z == 0 ) {
                        if ( UnityEngine.Random.Range( 1, 5 ) == 1 ) {
                            blocks[x, y, z] = 2;
                        }
                    } else {
                        blocks[x, y, z] = 1;
                    }
                }
            }
        }

        StartCoroutine( CreateMesh() );
    }

    IEnumerator CreateMesh() {
        vertices.Clear();
        triangles.Clear();
        //��������ĵ�����������ӽ�ȥ
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
        _isWorking = false;
    }

    public bool IsBlockTransparent( int x, int y, int z ) {
        if ( x >= width || y >= height || z >= width || x < 0 || y < 0 || z < 0 ) {
            return true;
        } else {
            return blocks[x, y, z] == 0;
        }
        return false;
    }

    private void AddFace_Front( int x, int y, int z, Block block ) {
        //��һ��������
        triangles.Add( 0 + vertices.Count );
        triangles.Add( 3 + vertices.Count );
        triangles.Add( 2 + vertices.Count );

        //�ڶ���������
        triangles.Add( 2 + vertices.Count );
        triangles.Add( 1 + vertices.Count );
        triangles.Add( 0 + vertices.Count );


        //���4����
        vertices.Add( new Vector3( 0 + x, 0 + y, 0 + z ) );
        vertices.Add( new Vector3( 0 + x, 0 + y, 1 + z ) );
        vertices.Add( new Vector3( 0 + x, 1 + y, 1 + z ) );
        vertices.Add( new Vector3( 0 + x, 1 + y, 0 + z ) );

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add( new Vector2( block.textureFrontX * textureOffset, block.textureFrontY * textureOffset ) + new Vector2( shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset ) + new Vector2( -shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset + textureOffset ) + new Vector2( -shrinkSize, -shrinkSize ) );
        uv.Add( new Vector2( block.textureFrontX * textureOffset, block.textureFrontY * textureOffset + textureOffset ) + new Vector2( shrinkSize, -shrinkSize ) );
    }

    private void AddFace_Back( int x, int y, int z, Block block ) {
        //��һ��������
        triangles.Add( 0 + vertices.Count );
        triangles.Add( 3 + vertices.Count );
        triangles.Add( 2 + vertices.Count );

        //�ڶ���������
        triangles.Add( 2 + vertices.Count );
        triangles.Add( 1 + vertices.Count );
        triangles.Add( 0 + vertices.Count );


        //���4����
        vertices.Add( new Vector3( -1 + x, 0 + y, 1 + z ) );
        vertices.Add( new Vector3( -1 + x, 0 + y, 0 + z ) );
        vertices.Add( new Vector3( -1 + x, 1 + y, 0 + z ) );
        vertices.Add( new Vector3( -1 + x, 1 + y, 1 + z ) );

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add( new Vector2( block.textureBackX * textureOffset, block.textureBackY * textureOffset ) + new Vector2( shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset ) + new Vector2( -shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset + textureOffset ) + new Vector2( -shrinkSize, -shrinkSize ) );
        uv.Add( new Vector2( block.textureBackX * textureOffset, block.textureBackY * textureOffset + textureOffset ) + new Vector2( shrinkSize, -shrinkSize ) );
    }

    private void AddFace_Right( int x, int y, int z, Block block ) {
        //��һ��������
        triangles.Add( 0 + vertices.Count );
        triangles.Add( 3 + vertices.Count );
        triangles.Add( 2 + vertices.Count );

        //�ڶ���������
        triangles.Add( 2 + vertices.Count );
        triangles.Add( 1 + vertices.Count );
        triangles.Add( 0 + vertices.Count );


        //���4����
        vertices.Add( new Vector3( 0 + x, 0 + y, 1 + z ) );
        vertices.Add( new Vector3( -1 + x, 0 + y, 1 + z ) );
        vertices.Add( new Vector3( -1 + x, 1 + y, 1 + z ) );
        vertices.Add( new Vector3( 0 + x, 1 + y, 1 + z ) );

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add( new Vector2( block.textureRightX * textureOffset, block.textureRightY * textureOffset ) + new Vector2( shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset ) + new Vector2( -shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset + textureOffset ) + new Vector2( -shrinkSize, -shrinkSize ) );
        uv.Add( new Vector2( block.textureRightX * textureOffset, block.textureRightY * textureOffset + textureOffset ) + new Vector2( shrinkSize, -shrinkSize ) );
    }

    private void AddFace_Left( int x, int y, int z, Block block ) {
        //��һ��������
        triangles.Add( 0 + vertices.Count );
        triangles.Add( 3 + vertices.Count );
        triangles.Add( 2 + vertices.Count );

        //�ڶ���������
        triangles.Add( 2 + vertices.Count );
        triangles.Add( 1 + vertices.Count );
        triangles.Add( 0 + vertices.Count );


        //���4����
        vertices.Add( new Vector3( -1 + x, 0 + y, 0 + z ) );
        vertices.Add( new Vector3( 0 + x, 0 + y, 0 + z ) );
        vertices.Add( new Vector3( 0 + x, 1 + y, 0 + z ) );
        vertices.Add( new Vector3( -1 + x, 1 + y, 0 + z ) );

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add( new Vector2( block.textureLeftX * textureOffset, block.textureLeftY * textureOffset ) + new Vector2( shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset ) + new Vector2( -shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset + textureOffset ) + new Vector2( -shrinkSize, -shrinkSize ) );
        uv.Add( new Vector2( block.textureLeftX * textureOffset, block.textureLeftY * textureOffset + textureOffset ) + new Vector2( shrinkSize, -shrinkSize ) );
    }

    private void AddFace_Top( int x, int y, int z, Block block ) {
        //��һ��������
        triangles.Add( 1 + vertices.Count );
        triangles.Add( 0 + vertices.Count );
        triangles.Add( 3 + vertices.Count );

        //�ڶ���������
        triangles.Add( 3 + vertices.Count );
        triangles.Add( 2 + vertices.Count );
        triangles.Add( 1 + vertices.Count );


        //���4����
        vertices.Add( new Vector3( 0 + x, 1 + y, 0 + z ) );
        vertices.Add( new Vector3( 0 + x, 1 + y, 1 + z ) );
        vertices.Add( new Vector3( -1 + x, 1 + y, 1 + z ) );
        vertices.Add( new Vector3( -1 + x, 1 + y, 0 + z ) );

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add( new Vector2( block.textureTopX * textureOffset, block.textureTopY * textureOffset ) + new Vector2( shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset ) + new Vector2( -shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset + textureOffset ) + new Vector2( -shrinkSize, -shrinkSize ) );
        uv.Add( new Vector2( block.textureTopX * textureOffset, block.textureTopY * textureOffset + textureOffset ) + new Vector2( shrinkSize, -shrinkSize ) );
    }

    private void AddFace_Bottom( int x, int y, int z, Block block ) {
        //��һ��������
        triangles.Add( 1 + vertices.Count );
        triangles.Add( 0 + vertices.Count );
        triangles.Add( 3 + vertices.Count );

        //�ڶ���������
        triangles.Add( 3 + vertices.Count );
        triangles.Add( 2 + vertices.Count );
        triangles.Add( 1 + vertices.Count );


        //���4����
        vertices.Add( new Vector3( -1 + x, 0 + y, 0 + z ) );
        vertices.Add( new Vector3( -1 + x, 0 + y, 1 + z ) );
        vertices.Add( new Vector3( 0 + x, 0 + y, 1 + z ) );
        vertices.Add( new Vector3( 0 + x, 0 + y, 0 + z ) );

        //���UV����㣬������4����ѭ����˳��һ��
        uv.Add( new Vector2( block.textureBottomX * textureOffset, block.textureBottomY * textureOffset ) + new Vector2( shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset ) + new Vector2( -shrinkSize, shrinkSize ) );
        uv.Add( new Vector2( block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset + textureOffset ) + new Vector2( -shrinkSize, -shrinkSize ) );
        uv.Add( new Vector2( block.textureBottomX * textureOffset, block.textureBottomY * textureOffset + textureOffset ) + new Vector2( shrinkSize, -shrinkSize ) );
    }
}
