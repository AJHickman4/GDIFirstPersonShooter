using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBeltSpawn : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToSpawn;
    [SerializeField] public float spawnInterval = 2f;
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private float lifetime = 5f;

    [SerializeField] private float timer;



    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObject();
            timer = 0;
        }
    }


    void SpawnObject()
    {
        if (objectsToSpawn.Count == 0)
        {
           
            return;
        }

        int index = Random.Range(0, objectsToSpawn.Count); //spawn one listed rando item
        GameObject selectedObject = objectsToSpawn[index];

        GameObject instance = Instantiate(selectedObject, spawnLocation.position, spawnLocation.rotation);
        Destroy(instance, lifetime);
    }
}
