using UnityEngine;

public class UnlimitedAmmoPowerUp : MonoBehaviour
{
    public float duration = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PowerUpManager.Instance.ActivateUnlimitedAmmo(duration);
            gameObject.SetActive(false);
            Destroy(gameObject, 1f);
        }
    }
}