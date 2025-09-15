using UnityEngine;

[System.Serializable]
public class AnimMapping
{
    public MonsterAnimState state;
    public string animName;
}

[CreateAssetMenu(menuName = "Monster/AnimationData", fileName = "NewMonsterAnimData")]
public class MonsterAnimationData : ScriptableObject
{
    public AnimMapping[] mappings;
}
