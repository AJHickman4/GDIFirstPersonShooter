using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin, LockVertMax; //usually 90 and -90 for scoape
    [SerializeField] bool invertY;
    [SerializeField] Camera mainCamera; //references main camera
    [SerializeField] float normalFOV = 60f;
    [SerializeField] float sprintingFOV = 75f;
    [SerializeField] float FOVTransition = 10f; //need this transition time for the lerp function below. //required 3 fields
    private bool isCameraLocked = false;

    public playerController player;

    float rotX; //how we get the rotation on the x axis. 

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  //cannot leave screen space while hidden.
        Cursor.visible = false;

        transform.forward = transform.parent.forward;//get camera to face forward right out of the gate at start. 



    }


    // Update is called once per frame
    void Update()
    {
        if (!isCameraLocked)  
        {
            // Get input
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

            // Invert look 
            if (invertY)
                rotX += mouseY;
            else
                rotX -= mouseY;

            rotX = Mathf.Clamp(rotX, lockVertMin, LockVertMax);
            transform.localRotation = Quaternion.Euler(rotX, 0, 0);
            transform.parent.Rotate(Vector3.up * mouseX);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, sprintingFOV, FOVTransition * Time.deltaTime);
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, FOVTransition * Time.deltaTime);
        }
    }

    public void LockCamera(bool shouldLock)
    {
        isCameraLocked = shouldLock;
        Cursor.lockState = shouldLock ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = shouldLock;
    }

    public void ResetFOV()
    {
        StopCoroutine("SmoothResetFOV");
        StartCoroutine(SmoothResetFOV());
    }
    private IEnumerator SmoothResetFOV()
    {
        while (Mathf.Abs(mainCamera.fieldOfView - normalFOV) > 0.1f)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, FOVTransition * Time.deltaTime);
            yield return null;  
        }
        mainCamera.fieldOfView = normalFOV;
    }
}