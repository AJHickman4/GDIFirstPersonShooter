using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb; 

    [SerializeField] private int damage = 50; 
    [SerializeField] private int speed = 1000; 
    [SerializeField] private float destroyTime = 5f; 

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>(); 

        rb.velocity = transform.forward * speed; 
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && !other.CompareTag("Player"))
        {
            return; 
        }

        IDamage damageable = other.GetComponent<IDamage>(); 

        if (damageable != null && other.CompareTag("Player"))
        {
            damageable.takeDamage(damage); 
        }
        Destroy(gameObject);
    }
}