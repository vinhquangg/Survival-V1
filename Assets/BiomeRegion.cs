using UnityEngine;

public class BiomeRegion : MonoBehaviour
{
    public BiomeData biomeData;

    private void OnDrawGizmos()
    {
        if (biomeData == null) return;
        Gizmos.color = biomeData.fogColor;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}
