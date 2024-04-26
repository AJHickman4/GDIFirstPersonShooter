using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform targetTransform;
    public bool isActive = true;
    public GameObject portal;
    private int entryCount = 0;  
    public int maxEntries = 20;   

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.GetComponent<playerController>() != null)
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                other.transform.position = targetTransform.position;
                gameManager.instance.StartResetTimer();
                controller.enabled = true;
                entryCount++;
                if (entryCount >= maxEntries)
                {
                    DeactivatePortal();
                }
            }
        }
    }

    public void ActivatePortal()
    {
        isActive = true;
        entryCount = 0;  
    }

    private void DeactivatePortal()
    {
        portal.SetActive(false);
        entryCount = 0;
    }
}
