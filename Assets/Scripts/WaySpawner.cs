using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    public int enemiesPerWave = 10;
    public float spawnDelay = 1f;   
    public float waveCooldown = 5f;    
    private int currentWave = 0;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            currentWave++;
            Debug.Log("Wave " + currentWave);

            for (int i = 0; i < enemiesPerWave; i++)
            {
                ZombiesPooling.instance.Get_Zombie_01();
                yield return new WaitForSeconds(spawnDelay);
            }

            yield return new WaitForSeconds(waveCooldown);
        }
    }
}