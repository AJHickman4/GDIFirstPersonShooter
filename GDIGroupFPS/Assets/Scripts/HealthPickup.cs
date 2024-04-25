using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public AudioClip pickupSound;
    private AudioSource audioSource;
    public int healAmount = 25; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        AdjustSpawnHeight();
    }
    private void AdjustSpawnHeight()
    {

        RaycastHit hit;
        float distanceToGround = 10.0f; 
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distanceToGround))
        {
            transform.position = hit.point + Vector3.up * 0.5f;  
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerController playerScript = other.gameObject.GetComponent<playerController>();
            if (playerScript != null && playerScript.HP < playerScript.HPOrig)
            {
                playerScript.Heal(healAmount);
                audioSource.PlayOneShot(pickupSound);
                Destroy(gameObject, pickupSound.length);
            }
        }
    }
}