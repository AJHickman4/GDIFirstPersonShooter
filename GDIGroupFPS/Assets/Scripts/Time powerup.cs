using UnityEngine;

public class TimePowerUp : MonoBehaviour
{
    public float additionalTime = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            gameManager.instance.IncreaseResetTimer(additionalTime);
            Destroy(gameObject); 
        }
    }
}
