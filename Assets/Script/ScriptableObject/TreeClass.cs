using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewTree", menuName = "Environment/Tree")]
public class TreeClass : ScriptableObject
{
    public string treeName;
    public GameObject stumpPrefab;
    public string stumpPoolTag;
}
