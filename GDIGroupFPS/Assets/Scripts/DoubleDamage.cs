using UnityEngine;

public class DamageBoostPowerUp : MonoBehaviour
{
    public float boostDuration = 15f;
    public int damageMultiplier = 2; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Weapon playerWeapon = other.GetComponentInChildren<Weapon>();
            if (playerWeapon != null)
            {
                playerWeapon.ApplyDamageBoost(damageMultiplier, boostDuration);
                gameObject.SetActive(false); 
                Destroy(gameObject, 1f); 
            }
        }
    }
}