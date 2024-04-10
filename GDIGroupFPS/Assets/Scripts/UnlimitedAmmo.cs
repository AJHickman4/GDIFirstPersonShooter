using UnityEngine;
using System.Collections;

public class UnlimitedAmmoPowerUp : MonoBehaviour
{
    public float duration = 15f; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Weapon[] weapons = FindObjectsOfType<Weapon>();
            foreach (var weapon in weapons)
            {
                weapon.EnableUnlimitedAmmo(duration);
            }
            gameObject.SetActive(false);
            Destroy(gameObject, 1f);
        }
    }
}