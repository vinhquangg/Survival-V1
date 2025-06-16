using UnityEngine;

[CreateAssetMenu(menuName = "Environment/Biome")]
public class BiomeData : ScriptableObject
{
    public BiomeType biomeType;
    public Color fogColor;
    public AudioClip ambientSound;
    public GameObject[] spawnableObjects;
    public float spawnDensity;
    public TerrainLayer[] terrainTextures;
}
