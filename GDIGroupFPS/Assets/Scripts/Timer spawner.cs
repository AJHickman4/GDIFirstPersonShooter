using System.Collections;
using UnityEngine;

public class Timerspawner : MonoBehaviour
{
    public GameObject objectToSpawn; 
    private GameObject currentObject; 
    public float spawnInterval = 10.0f; 

    private void Start()
    {
        if (objectToSpawn == null)
        {
            return;
        }
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true) 
        {
            yield return new WaitForSeconds(spawnInterval);
            if (currentObject == null) 
            {
                currentObject = Instantiate(objectToSpawn, transform.position, Quaternion.identity);
            }
        }
    }
}
