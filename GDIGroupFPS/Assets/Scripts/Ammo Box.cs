using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{  
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Weapon weaponScript = other.gameObject.GetComponentInChildren<Weapon>();
            if (weaponScript != null)
            {
                weaponScript.AddOneMagIfNeeded(); 
                Destroy(gameObject);
            }
        }
    }
}