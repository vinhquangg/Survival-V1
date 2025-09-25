using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDropTable", menuName = "Enemy/Drop Table")]
public class DropTableData : ScriptableObject
{
    [System.Serializable]
    public class DropEntry
    {
        public string poolID;           
        public int spawnCount = 1;      
        public float chance = 1f;
        public Vector3 offset;
        [HideInInspector]public int quantity;        
    }

    public DropEntry[] drops;
}
