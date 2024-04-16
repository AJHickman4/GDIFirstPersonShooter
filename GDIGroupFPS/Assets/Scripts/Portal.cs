using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform targetTransform; 
    public bool isActive = true; 
    
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
            }
        }
    }
        public void ActivatePortal()
        {
            isActive = true; // use this method to activate the portal for shop
        }
    }

