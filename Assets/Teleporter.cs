using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class Teleporter : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public float teleportDistance = 2f;

    [Header("Teleport Settings")]
    public KeyCode teleportKey = KeyCode.T;

    // Danh sách tất cả spawn zones trong scene
    private List<SpawnZone> spawnZones = new List<SpawnZone>();

    // Danh sách quái hợp lệ để teleport
    private List<BaseMonster> validTargets = new List<BaseMonster>();
    private int currentIndex = 0;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        spawnZones = FindObjectsOfType<SpawnZone>().ToList();

        RefreshMonsterList();
    }

    void Update()
    {
        if (Input.GetKeyDown(teleportKey))
        {
            TeleportToNextMonster();
        }
    }

    void RefreshMonsterList()
    {
        validTargets.Clear();

        foreach (var zone in spawnZones)
        {
            if (zone == null) continue;

            foreach (var enemy in zone.activeEnemies)
            {
                if (enemy == null || !enemy.activeInHierarchy) continue;

                BaseMonster monster = enemy.GetComponent<BaseMonster>();
                if (monster == null || monster.currentHeal <= 0) continue;

                // Chỉ lấy Swamp và Dragon
                if (monster is SwampMonster || monster is DragonMonster)
                {
                    validTargets.Add(monster);
                }
            }
        }

        currentIndex = 0;
    }

    void TeleportToNextMonster()
    {
        
        validTargets.RemoveAll(m => m == null || !m.gameObject.activeInHierarchy || m.currentHeal <= 0);


        if (validTargets.Count == 0)
        {
            RefreshMonsterList();
        }

       
        if (validTargets.Count == 0)
        {
            if (ZoneDropHandler.hasRecentDrop)
            {
                StartCoroutine(TeleportToDropNextFrame());
                return;
            }
            else
            {
                return;
            }
        }


        if (currentIndex >= validTargets.Count)
            currentIndex = 0;

        BaseMonster target = validTargets[currentIndex];
        currentIndex++;

        if (target == null || !target.gameObject.activeInHierarchy || target.currentHeal <= 0)
        {
            TeleportToNextMonster();
            return;
        }

        // Teleport player tới cạnh quái
        Vector3 dir = (player.position - target.transform.position).normalized;
        Vector3 targetPos = target.transform.position + dir * teleportDistance;

        player.position = targetPos;

    }

    private IEnumerator TeleportToDropNextFrame()
    {
        yield return null;

        if (ZoneDropHandler.hasRecentDrop && ZoneDropHandler.lastDropPosition != Vector3.zero)
        {
            Vector3 targetPos = ZoneDropHandler.lastDropPosition + Vector3.up * 1.2f;

            // Kiểm tra xem player đã thực sự di chuyển chưa
            if (Vector3.Distance(player.position, targetPos) > 0.5f)
            {
                player.position = targetPos;
                ZoneDropHandler.hasRecentDrop = false; // chỉ reset khi tele thành công
            }
        }
    }


}
