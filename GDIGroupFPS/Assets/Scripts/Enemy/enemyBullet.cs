using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;


    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, destroyTime);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && !other.CompareTag("Player"))
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        
        if (dmg != null && other.CompareTag("Player"))
        {
            dmg.takeDamage(damage);
        }

        Destroy(gameObject);
    }
}