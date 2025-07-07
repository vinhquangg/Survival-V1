using UnityEngine;

public class WorldSeedManager : MonoBehaviour
{
    public static int Seed { get; private set; }

    [SerializeField] private string customSeed = "";

    private void Awake()
    {
        if (string.IsNullOrEmpty(customSeed))
            Seed = Random.Range(int.MinValue, int.MaxValue);
        else
            Seed = customSeed.GetHashCode();

        Debug.Log("🌱 Seed = " + Seed);
    }
}
