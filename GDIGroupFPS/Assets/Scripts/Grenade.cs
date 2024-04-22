using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float delay = 3.0f; 
    [SerializeField] private float blastRadius = 5.0f; 
    [SerializeField] private int damage = 50;  
    [SerializeField] private LayerMask damageLayer;  
    [SerializeField] private GameObject explosionEffect; 
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioSource audioSource; 

    private float countdown;
    private bool hasExploded = false;
    private bool soundPlayed = false;

    void Start()
    {
        countdown = delay;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0.5f && !soundPlayed)
        {
            audioSource.PlayOneShot(explosionSound);
            soundPlayed = true;
        }
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    private void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius, damageLayer);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag("Player"))
            {
                IDamage dmg = nearbyObject.GetComponent<IDamage>();
                if (dmg != null)
                {
                    dmg.takeDamage(damage);
                }
            }
        }
        float destroyDelay = Mathf.Max(0.5f + explosionSound.length, 0f); 
        Destroy(gameObject, destroyDelay);
    }
}

