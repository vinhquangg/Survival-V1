using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDropTable", menuName = "Enemy/Drop Table")]
public class DropTableData : ScriptableObject
{
    [System.Serializable]
    public class DropEntry
    {
        public string poolID;           // "RawMeat"
        public int spawnCount = 1;      // Số prefab spawn (1 hoặc nhiều object)
        public float chance = 1f;
        public Vector3 offset;
        public int quantity = 1;        // 🟢 Quantity của item nằm trong ItemEntity
    }

    public DropEntry[] drops;
}
