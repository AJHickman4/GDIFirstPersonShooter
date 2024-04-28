using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShop : MonoBehaviour
{
    public ShopManager shopManager; // Reference to the ShopManager script

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (PlayerIsNearby()) 
            {
                shopManager.OpenShop();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            shopManager.CloseShop();
        }
    }

    private bool PlayerIsNearby()
    {
       
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f); 
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}
