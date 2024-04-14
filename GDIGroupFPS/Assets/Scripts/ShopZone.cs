using UnityEngine;

public class ShopZone : MonoBehaviour
{
    public Camera mainCamera;
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
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            ToggleZoom();
        }

        if (isZoomed)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoomFOV, zoomSpeed * Time.deltaTime);
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 60f, zoomSpeed * Time.deltaTime);
        }
    }

    private void ToggleZoom()
    {
        isZoomed = !isZoomed;
        cameraControl.LockCamera(isZoomed);

        if (isZoomed)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            EquipScript.HideEquippedWeapons(); // Hide weapons
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            EquipScript.ShowEquippedWeapon(); // Show weapons
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
            if (isZoomed) // Force zoom out when player leaves the shop zone
            {
                ToggleZoom();
            }
        }
    }
}