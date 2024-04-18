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

            playerController playerController = other.gameObject.GetComponent<playerController>();
            if (playerController != null && playerController.currentWeapon != null)
            {
                Weapon activeWeapon = playerController.currentWeapon;
                if (!activeWeapon.isReloading)
                {
                    bool magAdded = activeWeapon.AddOneMagIfNeeded();
                    if (magAdded)
                    {
                        audioSource.PlayOneShot(pickupSound);
                        Destroy(gameObject, pickupSound.length);
                    }
                }
            }
        }
    }
}