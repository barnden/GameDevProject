using System; // contains [Serializable]
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // for interfacing with the system's subobjects (timer, wave number display, ect)

public class WaveUISystem : MonoBehaviour
{
    [Serializable]
    private class WaveEnemyParams
    {
        public GameObject enemyToSpawn;
        public int numToSpawn;
        public float delayForEachSpawn;
        public float spawnRadius;
    }

    [Serializable]
    private class WaveInfo
    {
        public int waveTime;
        public string waveName;
        public string waveDescription;
        public WaveEnemyParams[] waveEnemies;
    }

    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] TextMeshProUGUI waveCounter;
    [SerializeField] WaveInfo[] waveList;
    public GameObject playerLocation; // TO BE DELETED

    // Start is called before the first frame update
    void Start()
    {
        timer.GetComponent<Stopwatch>().startStopwatch();
        waveCounter.GetComponent<WaveCounter>().setWave(0);
    }

    void Update()
    {
        // Check next time and see if it's the same as the next wave event
        // get current wave
        int currentWaveNumber = waveCounter.GetComponent<WaveCounter>().getWave();

        // make sure we don't go out of scope
        if (currentWaveNumber < waveList.Length)
        {
            WaveInfo currentWave = waveList[currentWaveNumber];
            if (currentWave.waveTime <= timer.GetComponent<Stopwatch>().getCurrentSecond())
            {
                Debug.Log("Start wave!: " + currentWave.waveName);
                waveCounter.GetComponent<WaveCounter>().setWave(currentWaveNumber + 1);

                // Spawn the wave
                StartCoroutine(SpawnWave(currentWave));
                
            }
            // display waveSprites
        }
    }

    IEnumerator SpawnWave(WaveInfo currentWave)
    {
        // Instantiate each enemy within the current wave
        foreach (WaveEnemyParams currWaveEnemy in currentWave.waveEnemies)
        {
            for (int enemyCounter = 0; enemyCounter < currWaveEnemy.numToSpawn; enemyCounter++)
            {
                GameObject enemySpawned = Instantiate(currWaveEnemy.enemyToSpawn);

                // find random enemy position
                Vector3 spawnPos = RandomEnemyPosition(currWaveEnemy.spawnRadius, 1.0f);
                enemySpawned.transform.position = playerLocation.transform.position + spawnPos;
                yield return new WaitForSeconds(currWaveEnemy.delayForEachSpawn);
            }
        }
    }

    void getNextWave()
    {
        waveCounter.GetComponent<WaveCounter>().getWave();
    }
    public Vector3 RandomEnemyPosition(float radius, float offset)
    {
        float randomAngle = UnityEngine.Random.Range(0.0f, Mathf.PI) * Mathf.Rad2Deg;
        float xPos = radius * Mathf.Cos(randomAngle);
        float yPos = radius * Mathf.Sin(randomAngle);
        return new Vector3(xPos,yPos, 0);
    }

    // Checks that all params are filled and valid

}