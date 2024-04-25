using UnityEngine;
using System.Collections;

public class PowerUpPickup : MonoBehaviour
{

    private playerController playerHealth;

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
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<playerController>();
            if (playerHealth != null)
            {
                playerHealth.ActivateForceField(15);
                playerHealth.SetInvincibility(true);
                gameManager.instance.ShowShieldIcon();
                gameObject.SetActive(false);
            }
        }
    }

}