using UnityEngine;

public class DamageBoostPowerUp : MonoBehaviour
{
    public float boostDuration = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PowerUpManager.Instance.ActivateDoubleDamage(boostDuration);
            gameObject.SetActive(false);
            Destroy(gameObject, 1f);
        }
    }
}