using System; // contains [Serializable]
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // for interfacing with the system's subobjects (timer, wave number display, ect)

public class WaveUISystem : MonoBehaviour
{
    [Serializable]
    private class WaveInfo
    {
        public int waveTime;
        public string waveName;
        public string waveDescription;
        public Sprite[] waveSprites;
    }

    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] TextMeshProUGUI waveCounter;
    [SerializeField] WaveInfo[] waveList;

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

            }

            // display waveSprites
        }
    }

    void getNextWave()
    {
        waveCounter.GetComponent<WaveCounter>().getWave();
    }
}