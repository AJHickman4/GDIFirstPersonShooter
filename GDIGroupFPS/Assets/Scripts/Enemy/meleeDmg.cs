using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeDmg : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int speed;



    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(damage);
        }
    }
}
