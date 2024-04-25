using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CoinDrop : MonoBehaviour
{
    public playerController playerScript;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        playerController playerScript = other.gameObject.GetComponent<playerController>();
        if (other.CompareTag("Player"))
        {
            playerScript.CoinDrop();
            gameObject.SetActive(false);
        }
    }
}
