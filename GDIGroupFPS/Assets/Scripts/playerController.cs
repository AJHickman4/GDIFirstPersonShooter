using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Compenents -----")]
    public CharacterController controller;
    [SerializeField] Animator anim;

    [Header("----- Player Stats -----")]
    [Range(1, 20)] public float speed;
    [Range(1, 25)] public float sprintSpeed;
    [Range(0.5f, 2f)][SerializeField] float crouchSpeed;

    [Range(1, 3)] public float jump;
    [Range(5, 25)][SerializeField] int jumpSpeed;
    [Range(-15, -35)][SerializeField] int gravity;

    [Header("----- Health -----")]
    [Range(1, 300)] public int HP;
    public int HPOrig;
    private bool isAlive = true;

    [Header("----- Stamina -----")]
    [SerializeField] public float maxStamina;
    [SerializeField] public float currentStamina;
    [SerializeField] private float staminaDrain = 10f; // Stamina per second used
    [SerializeField] private float staminaRechargeRate = 5f; // regen per second
    [SerializeField] bool canSprint = true;


    [Header("----- Inventory -----")]
    public List<int> keys; // Inventory of keys
    public int credits; // Currency Balance of the Player

    [Header("----- Sliding -----")]
    [SerializeField] private float maxSlideTime = 2f;
    [SerializeField] private float slideSpeed = 10f;
    [SerializeField] private KeyCode slideKey = KeyCode.LeftAlt;
    [SerializeField] private KeyCode slideMouseButton = KeyCode.Mouse3;
    private float slideTimer;
    private bool isSliding = false;
    [SerializeField] private float slideYScale = 0.5f;

    [Header("----- Melee Attack Parameters -----")]
    public GameObject meleeWeapon; 
    public Animator meleeAnimator;
    public float meleeRange = 2.0f;
    public float meleeDamage = 25.0f; 
    public float meleeCooldown = 1.0f; 
    public bool isMeleeReady = true;


    [Header("----- Other -----")]

    private float originalHeight; //standard startingg height of our character controller
    private float crouchHeight = 1f; //adjustment to the height when crouching
    int jumpCount;
    Vector3 moveDir;
    Vector3 playerVel;
    bool isShooting;
    public bool isInvincible = false;
    public GameObject forceFieldEffect;
    public bool isTakingDamage;
    public float lastDamageTime = -1f;
    public float interactionRange = 5f;
    public Camera playerCamera;
    private DoorOpenClose currentlyAimedDoor = null;
    public EquipScript equipScript;
    private Weapon currentWeapon;

    private float speedMultiplier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        credits = 0;
        gameManager.instance.updateCreditsUI();
        originalHeight = controller.height; //stores original height at the start of play. 
        HPOrig = HP;

        currentStamina = maxStamina;
        spawnPlayer();
    }

    public void spawnPlayer()
    {
        isAlive = true;
        HP = HPOrig;
        updatePlayerUI();
        controller.enabled = false;
        transform.position = gameManager.instance.startingSpawn.transform.position;
        controller.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        /*Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);*/ //draws the raycast in the Scene. 
        float actualSpeed = speed * speedMultiplier;
        movement();
        CheckForDoorAiming();
        currentWeapon = equipScript.GetCurrentWeapon();

        //begin the crouch 
        if (Input.GetKeyDown(KeyCode.C))
        {
            Crouch();
        }

        //end crouch
        if (Input.GetKeyUp(KeyCode.C))
        {
            StandUp();
        }
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRechargeRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            if (currentStamina >= maxStamina) //can sprint only once stamina has fully recharged
            {
                canSprint = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.T)) // Press T key to apply test damage
        {
            takeDamage(1); // Apply 10 damage for testing
        }
        if ((Input.GetKeyDown(slideKey) || Input.GetKeyDown(slideMouseButton)) && !isSliding && controller.isGrounded)
        {
            StartSlide();
        }
        else if ((Input.GetKeyUp(slideKey) || Input.GetKeyUp(slideMouseButton)) && isSliding)
        {
            StopSlide();
        }

        if (isSliding)
        {
            SlideMovement();
        }
        
        if (Input.GetKeyDown(KeyCode.V) && isMeleeReady) 
        {
            StartCoroutine(PerformMeleeAttack()); 
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
        if (controller.height < originalHeight) // when crouching, you are slower, AND turn into a short king
        {
            currentSpeed = crouchSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            currentSpeed = sprintSpeed;
            currentStamina -= staminaDrain * Time.deltaTime;

            if (currentStamina < 0)
            {
                currentSpeed = 0;
                canSprint = false; //not allowed to sprint when stamina is depleated. 
            }

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

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            currentStamina += staminaRechargeRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        }

        updatePlayerUI();


    }


    void Crouch()
    {
        controller.height = crouchHeight;
    }

    void StandUp()
    {
        controller.height = originalHeight;
    }

    public void takeDamage(int amount)
    {

        if (isInvincible || gameManager.instance.isResetting)
        {
            return;
        }
        if (!isAlive) return;
        isTakingDamage = true;
        StartCoroutine(ShowDamageIndicator());
        HP -= amount;
        HP = Mathf.Clamp(HPOrig, 0, HP);
        updatePlayerUI();


        if (HP <= 0)
        {
            Die();
            gameManager.instance.youHaveLost();
        }

    }
    void Die()
    {
        if (gameManager.instance.isResetting)
        {
            return;
        }

        isAlive = false;

    }
    public IEnumerator ShowDamageIndicator()
    {

        gameManager.instance.damageIndicator.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.damageIndicator.SetActive(false);
        isTakingDamage = false;
    }
    public void updatePlayerUI()
    {
        gameManager.instance.healthBar.fillAmount = (float)HP / HPOrig;
        gameManager.instance.staminaBar.fillAmount = (float)currentStamina / maxStamina;
    }

    public void SetInvincibility(bool invincible)
    {
        isInvincible = invincible;

    }

    public void Heal(int amount)
    {
        HP += amount;
        HP = Mathf.Clamp(HP, 0, HPOrig);
        updatePlayerUI();
    }

    public void ActivateForceField(float duration)
    {
        forceFieldEffect.SetActive(true);
        isInvincible = true;
        PowerUpManager.Instance.SetForceFieldActive(true);
        StartCoroutine(DeactivateForceFieldAfterDelay(duration));
    }

    private IEnumerator DeactivateForceFieldAfterDelay(float delay)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < delay)
        {
            yield return null;
        }
        gameManager.instance.HideShieldIcon();
        forceFieldEffect.SetActive(false);
        isInvincible = false;
        PowerUpManager.Instance.SetForceFieldActive(false);
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = maxSlideTime;
        controller.height = slideYScale;
    }

    private void SlideMovement()
    {
        if (slideTimer > 0)
        {
            Vector3 slideDirection = moveDir.normalized * slideSpeed;
            controller.Move(slideDirection * Time.deltaTime);
            slideTimer -= Time.deltaTime;
        }
        else
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        isSliding = false;
        controller.height = originalHeight;
    }

    void CheckForDoorAiming()
    {
        RaycastHit hit;
        float sphereRadius = 0.5f;
        //Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionRange, Color.red);
        if (Physics.SphereCast(playerCamera.transform.position, sphereRadius, playerCamera.transform.forward, out hit, interactionRange))
        {
            DoorOpenClose door = hit.collider.GetComponent<DoorOpenClose>();
            if (door != null && currentlyAimedDoor != door)
            {
                if (currentlyAimedDoor != null)
                {
                    currentlyAimedDoor.SetPlayerAimingAtDoor(false);
                }
                currentlyAimedDoor = door;
                currentlyAimedDoor.SetPlayerAimingAtDoor(true);
            }
        }
        else if (currentlyAimedDoor != null)
        {
            currentlyAimedDoor.SetPlayerAimingAtDoor(false);
            currentlyAimedDoor = null;
        }
    }

    IEnumerator PerformMeleeAttack()
    {
        if (!isMeleeReady || currentWeapon == null || currentWeapon.isReloading || currentWeapon.isRecoiling)
        {
            yield break;  
        }
        isMeleeReady = false;
        currentWeapon.readyToShoot = false; 
        equipScript.HideEquippedWeapons();
        meleeWeapon.SetActive(true);
        meleeAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);  
        ApplyMeleeDamage();
        yield return new WaitForSeconds(0.4f);
        meleeWeapon.SetActive(false);
        equipScript.ShowEquippedWeapon();
        currentWeapon.readyToShoot = true;
        isMeleeReady = true;
    }
    
    private void ApplyMeleeDamage()
    {
        Vector3 attackPosition = transform.position + transform.forward * 1.0f;
        float attackRange = meleeRange;
        Collider[] hitColliders = Physics.OverlapSphere(attackPosition, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                continue; 
            }
            IDamage target = hitCollider.GetComponent<IDamage>();
            if (target != null)
            {
                target.takeDamage((int)meleeDamage); 
            }
        }
    }

    public void AdjustSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }


}




