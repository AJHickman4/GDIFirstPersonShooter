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
            
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            shopManager.CloseShop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopManager.CloseShop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopManager.OpenShop();
        }
    }
}