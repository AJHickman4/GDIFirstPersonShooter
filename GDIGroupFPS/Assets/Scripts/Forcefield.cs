using UnityEngine;
using System.Collections;

public class PowerUpPickup : MonoBehaviour
{
    public GameObject forceFieldEffect; 
    private playerController playerHealth;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<playerController>();
            if (playerHealth != null)
            {
                ActivateForceField();
                playerHealth.SetInvincibility(true);
                gameManager.instance.ShowShieldIcon();
                gameObject.SetActive(false);
            }
        }
    }

    void ActivateForceField()
    {
        if (forceFieldEffect != null)
        {
            forceFieldEffect.SetActive(true);
            playerHealth.StartCoroutine(DeactivateForceFieldAfterDelay(30)); // Deactivate after 30 seconds
        }
    }

    public IEnumerator DeactivateForceFieldAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (forceFieldEffect != null)
        {
            forceFieldEffect.SetActive(false);
        }
        if (playerHealth != null)
        {
            playerHealth.SetInvincibility(false);
            gameManager.instance.HideShieldIcon();
            Destroy(gameObject);
        }
    }
}