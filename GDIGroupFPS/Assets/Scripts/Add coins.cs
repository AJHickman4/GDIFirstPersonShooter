using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Addcoins : MonoBehaviour
{
    public playerController playerScript;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerScript.AddCoins();
            gameObject.SetActive(false); 
        }
    }
}
