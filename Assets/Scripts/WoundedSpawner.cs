using System.Collections.Generic;
using UnityEngine;

public class WoundedSpawner : MonoBehaviour
{
    public GameObject soldierPrefab; // 拖入你的士兵预制体
    public int spawnCount = 8; // 刷新数量
    public float avoidRadius = 1f; // 避开障碍物的半径
    public LayerMask obstacleLayer; // 设置哪些层级属于“障碍物”

    private List<GameObject> spawnedSoldiers = new List<GameObject>();

    [Header("生成区域调节")]
    public float edgeMargin = 0.5f; // 边距减小到 0.5，让士兵能更靠边

    // 供 GameManager 调用的生成方法
    public void SpawnAll()
    {
        // 1. 清理旧士兵（如果是重开游戏）
        ClearSoldiers();

        // 关键修正：ViewportToWorldPoint 的 Z 轴必须设为摄像机与物体的距离（2D通常是10）
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10));

        int currentSpawned = 0;
        int attempts = 0; // 防止陷入死循环的计数器

        while (currentSpawned < spawnCount && attempts < 200) // 增加尝试次数
        {
            attempts++;

            // 使用变量控制边距
            float randomX = Random.Range(bottomLeft.x + edgeMargin, topRight.x - edgeMargin);
            float randomY = Random.Range(bottomLeft.y + edgeMargin, topRight.y - edgeMargin);
            Vector2 spawnPos = new Vector2(randomX, randomY);

            Collider2D hit = Physics2D.OverlapCircle(spawnPos, avoidRadius, obstacleLayer);

            if (hit == null)
            {
                GameObject s = Instantiate(soldierPrefab, spawnPos, Quaternion.identity);
                spawnedSoldiers.Add(s);
                currentSpawned++;
            }
        }
        // 在 while 循环结束后加入
        if (currentSpawned < spawnCount)
        {
            Debug.LogWarning(
                $"士兵生成不足！只生成了 {currentSpawned} 个。请检查 Avoid Radius 或 Obstacle Layer 设置。"
            );
        }
    }

    public void ClearSoldiers()
    {
        foreach (var s in spawnedSoldiers)
        {
            if (s != null)
                Destroy(s);
        }
        spawnedSoldiers.Clear();
    }

    /* private void OnDrawGizmosSelected()
    {
        // 在 Scene 窗口画出刷新范围，方便你调整
        Gizmos.color = Color.red;
        Vector3 bl = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10));
        Vector3 tr = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10));

        // 画一个矩形框
        Vector3 center = (bl + tr) / 2;
        Vector3 size = new Vector3(tr.x - bl.x - 2f, tr.y - bl.y - 2f, 0); // 这里的 2f 对应代码里的左右/上下边距
        Gizmos.DrawWireCube(center, size);
    } */
}
