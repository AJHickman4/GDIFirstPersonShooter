using UnityEngine;

public class ShopZone : MonoBehaviour
{
    public Camera mainCamera; 
    public Camera zoomCamera;
    public float zoomFOV = 10f;
    public float zoomSpeed = 5f; 
    private bool isPlayerNear = false;
    private bool isZoomed = false;

    public EquipScript EquipScript;
    public cameraController cameraControl;

    void Start()
    {
        if (!mainCamera)
            mainCamera = Camera.main;

        if (zoomCamera)
            zoomCamera.enabled = false;
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            ToggleZoom();
        }
    }

    private void ToggleZoom()
    {
        isZoomed = !isZoomed;
        cameraControl.LockCamera(isZoomed); 
        mainCamera.enabled = !isZoomed;
        if (zoomCamera)
            zoomCamera.enabled = isZoomed;
        Cursor.lockState = isZoomed ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isZoomed;
        if (isZoomed)
        {
            EquipScript.HideEquippedWeapons(); 
        }
        else
        {
            EquipScript.ShowEquippedWeapon(); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (isZoomed) 
            {
                ToggleZoom();
            }
        }
    }
}