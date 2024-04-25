using UnityEngine;

public class DamageBoostPowerUp : MonoBehaviour
{
    public float boostDuration = 15f;

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