using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("----- Compenents -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 5)][SerializeField] float speed;
    [Range(1, 10)][SerializeField] float sprintSpeed; //NEW 
    [Range(0.5f, 2f)][SerializeField] float crouchSpeed; //NEW 

    [Range(1, 3)][SerializeField] int jump;
    [Range(5, 25)][SerializeField] int jumpSpeed;
    [Range(-15, -35)][SerializeField] int gravity;

    [Header("----- Gun Stats -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    private float originalHeight; //standard startingg height of our character controller
    private float crouchHeight = 1f; //adjustment to the height when crouching
    int jumpCount;
    Vector3 moveDir;
    Vector3 playerVel;
    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {
        originalHeight = controller.height; //stores original height at the start of play. 
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red); //draws the raycast in the Scene. 

        movement();

        //getButton is a hold, getButtonDown instant (once)
        if (Input.GetButton("Shoot") && !isShooting)  //if is shooting is false then 
        {
            StartCoroutine(shoot()); //start IEnum true. 
        }

        //begin the crouch 
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
           Crouch();
        }

        //end crouch
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            StandUp();
        }
    }

    void movement()
    {
        if (controller.isGrounded) //resets jump counter when it is grounded or lands. So it can jump again. 
        {
            jumpCount = 0;
            playerVel = Vector3.zero;//resets vector to 0, stops gravity build up. 
        }



        float currentSpeed = speed;
        //run speed setter
        if (controller.height < originalHeight) // when crouching, you are slower
        {
            currentSpeed = crouchSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift)) // can only sprint when not crouching^^^
        {
            currentSpeed = sprintSpeed;
        }


        //1st person camera controls //depends on camera angle. 
        moveDir = Input.GetAxis("Horizontal") * transform.right
                + Input.GetAxis("Vertical") * transform.forward;

        //topdown camera controls //depends on camera positon/angle.
        //moveDir = new Vector3(Input.GetAxis("Horizantal"), 0, Input.GetAxis("Vertical"));

        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && jumpCount < jump) //if jump is less than jump count limit
        {
            jumpCount++; //increment / keep track of jump count.
            playerVel.y = jumpSpeed; //adds positiive number.
        }
;
        playerVel.y += gravity * Time.deltaTime;   //adds mechanics to gravity. makes negative number 
        controller.Move(playerVel * Time.deltaTime);
    }

    IEnumerator shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
        {
            Debug.Log(hit.collider.name);//spits out information debug.log then what you want info in.
                                         // Instantiate(cube, hit.point, transform.rotation);

            IDamage dmg = hit.collider.GetComponent<IDamage>(); //look at the thing we hit, if it has IDamage on it, save it

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void Crouch()
    {
        controller.height = crouchHeight;
    }

    void StandUp()
    {
        controller.height = originalHeight;
    }






}
