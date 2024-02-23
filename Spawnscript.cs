using System.Collections;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject slimePrefab;
    public GameObject bossPrefab;
    public float spawnInterval = 2.0f;
    public Vector2 spawnAreaSize1 = new Vector2(10f, 10f);
    public Vector2 spawnAreaSize2 = new Vector2(8f, 8f);
    public Vector2 spawnArea2Position = Vector2.zero;
    public bool canSpawn = true;
    private bool bossSpawned1 = false;
    private int enemiesSpawned = 0;
    public int enemiesKilledThreshold = 10;
    public int slimeMax = 5;
    public int slimesKilled = 0;
    private int enemiesKilled = 0;
    public bool bossGone;
    private int slimesSpawned = 0;
    [SerializeField] private bool willSlimesSpawn;
    [SerializeField] private bool playerEnteredArea;
    [SerializeField] private bool playerEnteredArea2;
    [SerializeField] private bool slimeBossSpawned;

    private bool cameraFollowEnabled = true;
    private Coroutine spawnCoroutine;

    void Start()
    {
        spawnCoroutine = StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds spawnWait = new WaitForSeconds(spawnInterval);

        while (true)
        {
            if (CanSpawn() && playerEnteredArea)
            {
                SpawnEnemy();
            }

            if (CanSpawnSlime() && playerEnteredArea2 && bossGone && willSlimesSpawn)
            {
                SpawnSlimeWithBorder();
            }

            yield return spawnWait;

            if (ShouldSpawnBoss())
            {
                SpawnBoss();
                yield return new WaitForSeconds(5.0f);
            }
        }
    }

    private void Update()
    {
        if (enemiesSpawned >= enemiesKilledThreshold)
        {
            canSpawn = false;
        }
        if (slimesSpawned >= slimeMax)
        {
            willSlimesSpawn = false;
        }

        IsPlayerInSpawnArea();
        IsPlayerInSpawnArea2();
    }

    public bool CanSpawn()
    {
        return canSpawn && !bossSpawned1;
    }

    private bool CanSpawnSlime()
    {
        return true;
    }

    private bool ShouldSpawnBoss()
    {
        return !bossSpawned1 && enemiesKilled >= enemiesKilledThreshold;
    }

    private bool ShouldSpawnSlimeBoss()
    {
        return !slimeBossSpawned && slimesKilled >= slimeMax;
    }

    private void SpawnEnemy()
    {
        Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnAreaSize1);
        Instantiate(enemyPrefab, transform.position + randomSpawnPosition, Quaternion.identity);
        enemiesSpawned++;
    }

    private void SpawnSlimeWithBorder()
    {
        Vector2 randomSpawnPosition = GetRandomSpawnPosition(spawnAreaSize2);
        Instantiate(slimePrefab, spawnArea2Position + randomSpawnPosition, Quaternion.identity);

        DrawCyanBorder(spawnArea2Position + randomSpawnPosition, spawnAreaSize2);
        slimesSpawned++;
    }

    private void DrawCyanBorder(Vector3 center, Vector2 size)
    {
        Vector3 halfSize = new Vector3(size.x / 2, size.y / 2, 0f);

        Debug.DrawLine(center + new Vector3(-halfSize.x, -halfSize.y, 0), center + new Vector3(halfSize.x, -halfSize.y, 0), Color.cyan);
        Debug.DrawLine(center + new Vector3(halfSize.x, -halfSize.y, 0), center + new Vector3(halfSize.x, halfSize.y, 0), Color.cyan);
        Debug.DrawLine(center + new Vector3(halfSize.x, halfSize.y, 0), center + new Vector3(-halfSize.x, halfSize.y, 0), Color.cyan);
        Debug.DrawLine(center + new Vector3(-halfSize.x, halfSize.y, 0), center + new Vector3(-halfSize.x, -halfSize.y, 0), Color.cyan);
    }

    private void SpawnBoss()
    {
        bossSpawned1 = true;
        Instantiate(bossPrefab, transform.position, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition(Vector2 spawnAreaSize)
    {
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        return new Vector3(randomX, randomY, 0f);
    }

    private bool IsPlayerInSpawnArea() //Same Script but with the original area, works perfectly fine
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(spawnAreaSize1.x, spawnAreaSize1.y), 0f);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return playerEnteredArea = true;
            }
        }

        return false;
    }

    private bool IsPlayerInSpawnArea2() //This is where the player should be detected, but the player is not
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(spawnAreaSize2.x, spawnAreaSize2.y), 0f);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return playerEnteredArea2 = true;
            }
        }

        return false;
    }

    public void SetCanSpawn(bool value)
    {
        canSpawn = value;
    }

    public bool IsCameraFollowEnabled()
    {
        return cameraFollowEnabled;
    }

    public void EnemyKilled()
    {
        canSpawn = true;
        enemiesKilled++;
    }

    public void BossDefeated()
    {
        canSpawn = false;
        bossGone = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize1.x, spawnAreaSize1.y, 1f));
    }
}
