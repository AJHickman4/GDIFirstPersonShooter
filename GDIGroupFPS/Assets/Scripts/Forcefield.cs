using UnityEngine;
using System.Collections;

public class PowerUpPickup : MonoBehaviour
{

    private playerController playerHealth;

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