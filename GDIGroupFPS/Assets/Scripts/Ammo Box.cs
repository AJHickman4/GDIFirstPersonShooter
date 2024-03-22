using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public AudioClip pickupSound;
    private AudioSource audioSource;

    void Start()
    {       
        audioSource = GetComponent<AudioSource>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Weapon weaponScript = other.gameObject.GetComponentInChildren<Weapon>();
            if (weaponScript != null)
            {
                weaponScript.AddOneMagIfNeeded();
                audioSource.PlayOneShot(pickupSound);
                Destroy(gameObject, pickupSound.length);
            }
        }
    }
}