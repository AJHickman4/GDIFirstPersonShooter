using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CoinDrop : MonoBehaviour
{
    public playerController playerScript;

    void Start()
    {
        AdjustSpawnHeight();
    }

    private void AdjustSpawnHeight()
    {
        RaycastHit hit;
        float distanceToGround = 10.0f;  
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distanceToGround))
        {
            transform.position = hit.point + Vector3.up * 0.5f;  
        }
    }
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
