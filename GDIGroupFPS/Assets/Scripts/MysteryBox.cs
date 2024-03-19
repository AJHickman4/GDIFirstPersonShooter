using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : MonoBehaviour
{
    public GameObject[] weaponPrefabs; 
    public float spawnOffset = 1.0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            float sphereRadius = 0.5f;
            if (Physics.SphereCast(Camera.main.transform.position, sphereRadius, Camera.main.transform.forward, out hit, 5f))
            {
                if (hit.collider.CompareTag("MysteryBox"))
                {
                    hit.collider.GetComponent<MysteryBox>().Interact();
                }
            }
        }
    }

    private void Interact()
    {
        Debug.Log("Spawning weapon above Mystery Box");
        int randomIndex = Random.Range(0, weaponPrefabs.Length);
        GameObject weaponToSpawn = weaponPrefabs[randomIndex];
        Vector3 spawnPosition = transform.position + Vector3.up * spawnOffset;
        GameObject spawnedWeapon = Instantiate(weaponToSpawn, spawnPosition, Quaternion.Euler(0, 90, 0));
    }
}
