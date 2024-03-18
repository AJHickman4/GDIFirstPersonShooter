using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int magazinesToAdd = 3; 

    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            
            Weapon weaponScript = other.gameObject.GetComponentInChildren<Weapon>();
            if (weaponScript != null)
            {
                
                weaponScript.totalMags += magazinesToAdd;
                Debug.Log("Added " + magazinesToAdd + " magazines.");

                Destroy(gameObject); 
            }
        }
    }
}
