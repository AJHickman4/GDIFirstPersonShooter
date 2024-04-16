using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShop : MonoBehaviour
{

    public ShopManager shopManager; // Reference to the ShopManager script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopManager.OpenShop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopManager.CloseShop();
        }
    }
}
