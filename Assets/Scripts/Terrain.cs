using LibNoise;
using LibNoise.Generator;
using Soultia.Util;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    public static byte GetTerrainBlock( Vector3i worldPosition ) {
        int seed = 20210615;
        Perlin noise = new Perlin( 1f, 1f, 1f, 8, seed, QualityMode.High );

        Random.InitState( seed );

        Vector3 offset = new Vector3( Random.value * 100000, Random.value * 100000, Random.value * 100000 );

        float noiseX = Mathf.Abs( (worldPosition.x+offset.x) / 20 );
        float noiseY = Mathf.Abs( ( worldPosition.y + offset.y ) / 20 );
        float noiseZ = Mathf.Abs( ( worldPosition.z + offset.z ) / 20 );
        double noiseValue = noise.GetValue( noiseX, noiseY, noiseZ );

        noiseValue += ( 20 - worldPosition.y ) / 15f;
        noiseValue /= worldPosition.y / 5f;

        if ( noiseValue > 0.5f ) {
            return 1;
        }

        return 0;
    }
}

