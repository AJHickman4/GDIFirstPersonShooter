using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proxSpawner : MonoBehaviour
{
    public  GameObject[] objectsToSpawn;
    public int numToSpawn;
    public int spawnTimer;
    public Transform[] spawnPos;
    public bool resetOnEnter = true;
    public int maxActiveObjects = 10;

    private List<GameObject> activeObjects = new List<GameObject>();
    private int spawnCount;
    private bool isSpawning;
    private bool startSpawning;

    void Update()
    {
        activeObjects.RemoveAll(item => item == null);

        if (startSpawning && !isSpawning && spawnCount < numToSpawn && activeObjects.Count < maxActiveObjects)
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

        GameObject spawnedObject = Instantiate(objectsToSpawn[objectIndex], spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        activeObjects.Add(spawnedObject);

        spawnCount++;
        yield return new WaitForSeconds(spawnTimer);
        isSpawning = false;
    }
}