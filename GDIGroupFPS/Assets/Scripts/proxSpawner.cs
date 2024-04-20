using System.Collections;
using UnityEngine;

public class proxSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int spawnTimer;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] bool resetOnEnter = true;  

    int spawnCount;
    bool isSpawning;
    bool startSpawning;

    void Start()
    {
       
    }

    void Update()
    {
        if (startSpawning && !isSpawning && spawnCount < numToSpawn)
        {
            StartCoroutine(Spawn());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (resetOnEnter)
            {
                spawnCount = 0;  
                isSpawning = false;  
            }
            startSpawning = true;
        }
    }

    IEnumerator Spawn()
    {
        isSpawning = true;
        int arrayPos = Random.Range(0, spawnPos.Length);
        int objectIndex = Random.Range(0, objectsToSpawn.Length);
        Instantiate(objectsToSpawn[objectIndex], spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        spawnCount++;
        yield return new WaitForSeconds(spawnTimer);
        isSpawning = false;
    }
}
