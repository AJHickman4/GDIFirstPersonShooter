using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    [SerializeField] private List<GameObject> powerUpPrefabs; // List of power-up prefabs to spawn
    [SerializeField] private Transform spawnPoint; // Point where power-ups will be spawned

    // Function to be called by the player to spawn a power-up
    public void DispensePowerUp()
    {
        if (powerUpPrefabs.Count == 0)
            return;

        // Randomly select a power-up to spawn
        int index = Random.Range(0, powerUpPrefabs.Count);
        GameObject powerUp = Instantiate(powerUpPrefabs[index], spawnPoint.position, Quaternion.identity);

        // some physics force to "spit out" the powerupss
        Rigidbody rb = powerUp.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(spawnPoint.forward * 5f, ForceMode.VelocityChange);
        }
    }
}
