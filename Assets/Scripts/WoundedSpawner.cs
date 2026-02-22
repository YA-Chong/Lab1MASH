using System.Collections.Generic;
using UnityEngine;

public class WoundedSpawner : MonoBehaviour
{
    public GameObject soldierPrefab;
    public int spawnCount = 8;
    public float avoidRadius = 1f;
    public LayerMask obstacleLayer;

    private List<GameObject> spawnedSoldiers = new List<GameObject>();

    [Header("Generate area")]
    public float edgeMargin = 0.5f;

    public void SpawnAll()
    {
        ClearSoldiers();

        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10));

        int currentSpawned = 0;
        int attempts = 0;

        while (currentSpawned < spawnCount && attempts < 200)
        {
            attempts++;

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

        if (currentSpawned < spawnCount)
        {
            Debug.LogWarning(
                $"Not enough soldiers to generate! Only {currentSpawned} have been created. please check Avoid Radius or Obstacle Layer settings."
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
}
