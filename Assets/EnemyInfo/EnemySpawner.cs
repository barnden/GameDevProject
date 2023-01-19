using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Spawn Factors
    public GameObject enemy;
    public GameObject playerLocation;
    private float spawnTimer;
    public float spawnCooldown;
    public float spawnRadius;
    public float spawnSpeed;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        // currently spawns enemies around player one at a time in a circle
        // vampire survivor spawns around the boundary of the player's screen
        if (spawnTimer <= 0.0f)
        {
            GameObject enemySpawned = Instantiate(enemy);
            enemySpawned.transform.position = playerLocation.transform.position + RandomEnemyPosition(spawnRadius, 1.0f);
            enemySpawned.GetComponent<BaseEnemyMove>().baseEnemySpeed = spawnSpeed;
            spawnTimer = spawnCooldown;
        }

    }
    
    public Vector3 RandomEnemyPosition(float radius, float offset)
    {
        float randomAngle = Random.Range(0.0f, Mathf.PI) * Mathf.Rad2Deg;
        float xPos = radius * Mathf.Cos(randomAngle) /*+ Random.Range(0.0f, offset)*/;
        float yPos = radius * Mathf.Sin(randomAngle) /*+ Random.Range(0.0f, offset)*/;
        return new Vector3(xPos,yPos, 0);
    }
}
