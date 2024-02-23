using System.Collections;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public GameObject enemyPrefab; //The zombie object is assigned here
    public GameObject slimePrefab; //The slime object is assigned here
    public GameObject bossPrefab; //The boss zombie object is assigned here
    public float spawnInterval = 2.0f;
    public Vector2 spawnAreaSize1 = new Vector2(10f, 10f); //Vector 2, starting scale for the first spawn area
    public Vector2 spawnAreaSize2 = new Vector2(8f, 8f); //Vector 2, starting scale for the second spawn area
    public Vector2 spawnArea2Position = Vector2.zero; //Sets the position of the spawn area to 0, 0
    public bool canSpawn = true; //If regular enemies can spawn
    private bool bossSpawned1 = false; //Is the zombie boss spawned
    private int enemiesSpawned = 0; //How many zombies have been spawned
    public int enemiesKilledThreshold = 10; //How many zombies need to be spawned
    public int slimeMax = 5; //Max amount of slimes
    public int slimesKilled = 0; //Amount of slimes killed
    private int enemiesKilled = 0; //Amount of zombies killed
    public bool bossGone; //Is the zombie boss dead
    private int slimesSpawned = 0; //Amount of slimes that have been spawned
    [SerializeField] private bool willSlimesSpawn; //Are the slimes allowed to spawn
    [SerializeField] private bool playerEnteredArea; //Is the player in area1
    [SerializeField] private bool playerEnteredArea2; //Is the player in area 2
    [SerializeField] private bool slimeBossSpawned; //Is the slime boss spawned

    private bool cameraFollowEnabled = true; //Does the camera follow the player, used for cutscenes
    private Coroutine spawnCoroutine; //Makes a coroutine, later used for loop

    void Start()
    {
        spawnCoroutine = StartCoroutine(SpawnEnemies()); //Sets the spawn coroutine to the SpawnEnemies IEnumerator
    }

    private IEnumerator SpawnEnemies() //Second part of spawn loop
    {
        WaitForSeconds spawnWait = new WaitForSeconds(spawnInterval); //How much time between spawns

        while (true)
        {
            if (CanSpawn() && playerEnteredArea) //If enemies can spawn and player is in area1, spawn enemy
            {
                SpawnEnemy();
            }

            if (CanSpawnSlime() && playerEnteredArea2 && bossGone && willSlimesSpawn)//Spawn slime if the conditions are met
            {
                SpawnSlimeWithBorder();
            }

            yield return spawnWait;

            if (ShouldSpawnBoss())
            {
                SpawnBoss();
                yield return new WaitForSeconds(5.0f); //Waits after spawning boss
            }
        }
    }

    private void Update()
    {
        if (enemiesSpawned >= enemiesKilledThreshold) //If the threshold amount of zombies have been spawned, set spawning to false
        {
            canSpawn = false;
        }
        if (slimesSpawned >= slimeMax) //Same thing as above comment but with slimes
        {
            willSlimesSpawn = false;
        }

        IsPlayerInSpawnArea();
        IsPlayerInSpawnArea2(); //These two just check if the player is in area1 /area2
    }

    public bool CanSpawn() //Can the boss spawn
    {
        return canSpawn && !bossSpawned1;
    }

    private bool CanSpawnSlime() //Slimes are allowed to spawn
    {
        return true;
    }

    private bool ShouldSpawnBoss() //Tells IEnumerator to summon the boss
    {
        return !bossSpawned1 && enemiesKilled >= enemiesKilledThreshold;
    }

    private bool ShouldSpawnSlimeBoss() //Gonna add a slime boss, so this does not do anything yet
    {
        return !slimeBossSpawned && slimesKilled >= slimeMax;
    }

    private void SpawnEnemy() //Where should the zombie should spawn
    {
        Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnAreaSize1);
        Instantiate(enemyPrefab, transform.position + randomSpawnPosition, Quaternion.identity);
        enemiesSpawned++;
    }

    private void SpawnSlimeWithBorder() //Where should the slime spawn
    {
        Vector2 randomSpawnPosition = GetRandomSpawnPosition(spawnAreaSize2);
        Instantiate(slimePrefab, spawnArea2Position + randomSpawnPosition, Quaternion.identity);

        DrawCyanBorder(spawnArea2Position + randomSpawnPosition, spawnAreaSize2);
        slimesSpawned++;
    }

    private void DrawCyanBorder(Vector3 center, Vector2 size) //Draws a border of area2, makes it easier to allign with arena
    {
        Vector3 halfSize = new Vector3(size.x / 2, size.y / 2, 0f);

        Debug.DrawLine(center + new Vector3(-halfSize.x, -halfSize.y, 0), center + new Vector3(halfSize.x, -halfSize.y, 0), Color.cyan);
        Debug.DrawLine(center + new Vector3(halfSize.x, -halfSize.y, 0), center + new Vector3(halfSize.x, halfSize.y, 0), Color.cyan);
        Debug.DrawLine(center + new Vector3(halfSize.x, halfSize.y, 0), center + new Vector3(-halfSize.x, halfSize.y, 0), Color.cyan);
        Debug.DrawLine(center + new Vector3(-halfSize.x, halfSize.y, 0), center + new Vector3(-halfSize.x, -halfSize.y, 0), Color.cyan);
    }

    private void SpawnBoss() //Instantiates the boss
    {
        bossSpawned1 = true;
        Instantiate(bossPrefab, transform.position, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition(Vector2 spawnAreaSize) //generates a random position for spawning
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

    public void SetCanSpawn(bool value) //Makes it easier to set the can spawn value for other scripts I think
    {
        canSpawn = value;
    }

    public bool IsCameraFollowEnabled() //Does the camera follow the player
    {
        return cameraFollowEnabled;
    }

    public void EnemyKilled() //If an enemy is killed, add to the enemy killed list, also was gonna use this for an endless mode
    {
        canSpawn = true;
        enemiesKilled++;
    }

    public void BossDefeated() //Is the boss zombie dead
    {
        canSpawn = false;
        bossGone = true;
    }

    private void OnDrawGizmosSelected() //Draw the square for area 1 to make it easier to allign with arena
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize1.x, spawnAreaSize1.y, 1f));
    }
}
