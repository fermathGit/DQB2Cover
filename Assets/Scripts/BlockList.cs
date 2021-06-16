using System.Collections.Generic;
using UnityEngine;

public class BlockList :MonoBehaviour
{
    public static Dictionary<byte, Block> blocks = new Dictionary<byte, Block>();

    private void Awake() {
        Block dirt = new Block( (byte)BlockType.dirt, "Dirt", 2, 31 );
        blocks.Add( dirt.id, dirt );

        Block grass = new Block( (byte)BlockType.grass, "Grass", 3, 31, 0, 31, 2, 31 );
        blocks.Add( grass.id, grass );

        Block rock = new Block( (byte)BlockType.rock, "Rock", 6, 31, 5, 31);
        blocks.Add( rock.id, rock );
    }

    public static Block GetBlock( byte id ) {
        return blocks.ContainsKey( id ) ? blocks[id] : null;
    }

    public static Block GetBlock( BlockType type ) {
        return GetBlock( (byte)type );
    }
    }

