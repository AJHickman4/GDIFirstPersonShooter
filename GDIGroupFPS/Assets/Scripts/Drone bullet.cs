using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dronebullet : MonoBehaviour
{
    // Start is called before the first frame update
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
        if (other.isTrigger && !other.CompareTag("Enemy"))
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();


        if (dmg != null && other.CompareTag("Enemy"))
        {
            dmg.takeDamage(damage);
        }

        Destroy(gameObject);
    }
}
