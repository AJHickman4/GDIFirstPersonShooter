using UnityEngine;
using System.Collections;

public class UnlimitedAmmoPowerUp : MonoBehaviour
{
    public float duration = 15f; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Weapon playerWeapon = other.GetComponentInChildren<Weapon>();
            if (playerWeapon != null)
            {
                playerWeapon.EnableUnlimitedAmmo(duration);
            }
            gameObject.SetActive(false);
            Destroy(gameObject, 1f);
        }
    }
}