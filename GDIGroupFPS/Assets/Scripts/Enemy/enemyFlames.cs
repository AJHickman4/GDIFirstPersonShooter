using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class enemyFlames : MonoBehaviour
{

    [SerializeField] float damage;


    //void start()
    //{
    //    rb.velocity = transform.forward * speed;
    //    destroy(gameobject, destroytime);

    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage((int)damage);
        }

        //Destroy(gameObject);
    }
}
