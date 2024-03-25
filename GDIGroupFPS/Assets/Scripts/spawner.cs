using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int spawnTimer;
    [SerializeField] Transform[] spawnPos;

    int spawnCount;
    bool isSpawning;
    bool startSpawning;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    IEnumerator spawn()
    {
        isSpawning = true;
        int arrayPos = Random.Range(0, spawnPos.Length);
        int enemySpawned = Random.Range(0, objectsToSpawn.Length);
        Instantiate(objectsToSpawn[enemySpawned], spawnPos[arrayPos].transform.position, spawnPos[arrayPos].transform.rotation);
        spawnCount++;
        yield return new WaitForSeconds(spawnTimer);
        isSpawning = false;
    }
}
