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
    [SerializeField] float FOVTransition = 10f; //need this transition time for the lerp function below. 


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
        //get input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        //invert look 
        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        //clamping the rot ont he x-axis.
        rotX = Mathf.Clamp(rotX, lockVertMin, LockVertMax);

        //rotate the camera on the x-Axis. 
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //rotate the player on the y-axis
        transform.parent.Rotate(Vector3.up * mouseX);


    //adjust FOV while sprinting..
    if (Input.GetKey(KeyCode.LeftShift))
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, sprintingFOV, FOVTransition * Time.deltaTime); //if shift is pressed make FOV become number
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, FOVTransition * Time.deltaTime); //reverts FOV back after sprinting / sets norn speed 
        }
        
    }
}
