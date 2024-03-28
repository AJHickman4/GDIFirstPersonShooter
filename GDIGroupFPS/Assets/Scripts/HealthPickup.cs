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