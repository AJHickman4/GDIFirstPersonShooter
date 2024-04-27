using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform targetTransform;
    public bool isActive = true;
    public GameObject portal;
    private int entryCount = 0;  
    public int maxEntries = 20;  
    public bool clearguns = true;
    public EquipScript equipScript;

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.GetComponent<playerController>() != null)
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null && !clearguns)
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
            else if (controller != null && clearguns)
            {
                controller.enabled = false;
                other.transform.position = targetTransform.position;
                controller.enabled = true;
                ClearGuns(other);
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
    public void ClearGuns(Collider other)
    {
        if (clearguns)
        {
            EquipScript equipScript = other.GetComponent<EquipScript>();
            equipScript.DestroyCurrentGun();
            equipScript.guns.Clear();
            gameManager.instance.playerScript.credits = 0;
            gameManager.instance.updateCreditsUI();
            gameManager.instance.playerScript.speed = 4;
            gameManager.instance.playerScript.HP = gameManager.instance.playerScript.HPOrig;
        }
        else if (!clearguns)
        {
            return;
        }
    }
}

