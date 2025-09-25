
using UnityEngine;

public interface IZoneDropHandler 
{
    void OnZoneCleared(SpawnZone zone, Vector3 lastDeathPos);
}
