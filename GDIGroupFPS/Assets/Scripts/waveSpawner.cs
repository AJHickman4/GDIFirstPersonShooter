using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveSpawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int spawnTimer;
    [SerializeField] Transform[] spawnPos;

    int spawnCount;
    bool isSpawning;
    bool startSpawning;
    int numberKilled;

    public bool firstDeath;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning && !isSpawning && spawnCount < numToSpawn)
        {
            StartCoroutine(spawn());
        }
    }

    public void startWave()
    {
        startSpawning = true;
    }

    public IEnumerator spawn()
    {
        isSpawning = true;
        int arrayPos = Random.Range(0, spawnPos.Length);
        GameObject objectSpawned = Instantiate(objectToSpawn, spawnPos[arrayPos].transform.position, spawnPos[arrayPos].transform.rotation);

        if (objectSpawned.GetComponent<enemyMeleeAI>())
            objectSpawned.GetComponent<enemyMeleeAI>().whereISpawned = this;
        else if (objectSpawned.GetComponent<enemyFlamerAI>())
            objectSpawned.GetComponent<enemyFlamerAI>().whereISpawned = this;

        spawnCount++;
        yield return new WaitForSeconds(spawnTimer);
        isSpawning = false;
    }

    public void updateEnemyNumber()
    {
        if (!firstDeath)
        {
            numberKilled++;
            firstDeath = true;
        }
        if (numberKilled >= numToSpawn)
        {
            startSpawning = false;
            StartCoroutine(waveManager.instance.startWave());
        }
    }
}
