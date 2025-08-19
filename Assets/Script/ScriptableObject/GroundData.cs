using UnityEngine;
[CreateAssetMenu(menuName = "Environment/GroundData")]
public class GroundData : ScriptableObject
{
    public TerrainLayer terrainLayer;
    public GroundType groundType;
    public AudioClip[] footstepClips;
}
