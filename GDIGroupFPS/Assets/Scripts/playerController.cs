using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Compenents -----")]
    public CharacterController controller;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource aud;
    [SerializeField] private cameraController myCameraController;
    
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
    [SerializeField] private float slideStaminaCost = 20f;
    [SerializeField] private KeyCode slideKey = KeyCode.LeftAlt;
    [SerializeField] private KeyCode slideMouseButton = KeyCode.Mouse3;
    private float slideTimer;
    private bool isSliding = false;
    [SerializeField] private float slideYScale = 0.5f;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audLose;
    [Range(0, 1)][SerializeField] float audLoseVol;

    [Header("----- Dash Parameters -----")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.3f;
    private bool isDashing = false;
    public Vector3 dashDirection;
    public float dashCooldown = 2f; 
    private float lastDashTime = -10f;
    public float verticalThreshold = 0.5f;

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

    public bool isInvincible = false;
    public GameObject forceFieldEffect;
    public bool isTakingDamage;
    public float lastDamageTime = -1f;
    public float interactionRange = 5f;
    public Camera playerCamera;
    private DoorOpenClose currentlyAimedDoor = null;
    private DoorOpenClosewithcost currentlyAimedDoorWithCost = null;
    private Openclosedoorwithcostnotimer currentlyAimedDoorWithCostNoTimer = null;
    public EquipScript equipScript;
    public Weapon currentWeapon;
    public bool canMove = true; // remove if you get animations to work
    private float speedMultiplier = 1f;

    private bool playingSteps;
    private bool isSprinting;
    private float lastHurtTime = 0;
    public float hurtSoundCooldown = 0.5f;  

    private float wantAlpha;
    private float startAlpha;
    private Vector3 lastPosition;
    public Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        credits = 0;
        gameManager.instance.updateCreditsUI();
        originalHeight = controller.height; //stores original height at the start of play. 
        HPOrig = HP;
        playerVel = Vector3.zero;
        currentStamina = maxStamina;
        spawnPlayer();
        lastPosition = transform.position;
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
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
        movement();
        CheckForDoorAiming();
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionRange, Color.green);
        CheckForButtonPress(); //For all interactables int he future.(But also this button) :)
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
        if (Input.GetKeyDown(KeyCode.T)) // Press T key to apply test damage
        {
            takeDamage(50); // Apply 10 damage for testing
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

        if (Input.GetKeyDown(KeyCode.Mouse1) && isMeleeReady)
        {
            StartCoroutine(PerformMeleeAttack());
        }


        if (HP <= HPOrig * 0.5)
        {
            StartCoroutine(FadeIn());
        }
        else if (HP >= HPOrig * 0.5)
        {
            StartCoroutine(FadeOut());
        }
        
        if (HP == HPOrig)
        {
            StartCoroutine(FadeOut());
        }
    }

    void movement()
    {
        if (!canMove) return; // Skip movement if movement is disabled.

        // Check if the player is grounded.
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero; // Reset the player's vertical velocity to stop gravity accumulation.
        }

        float currentSpeed = speed;
        // Handle crouching speed reduction.
        if (controller.height < originalHeight)
        {
            currentSpeed = crouchSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && canSprint && currentStamina > 0)
        {
            // Sprint only if Left Shift is held, stamina is positive, and canSprint is true.
            isSprinting = true;
            currentSpeed = sprintSpeed;
            currentStamina -= staminaDrain * Time.deltaTime;
        }

        // Handle stamina depletion and recovery.
        if (currentStamina <= 0)
        {
            currentStamina = 0;
            canSprint = false; // Disable sprinting when stamina is depleted.
            isSprinting = false;
        }

        // Recharge stamina when not sprinting and Left Shift is not held down.
        if (!Input.GetKey(KeyCode.LeftShift) || !isSprinting)
        {
            currentStamina += staminaRechargeRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            
            if (myCameraController != null)
            {
                myCameraController.ResetFOV();  
            } 
            if (currentStamina >= 20)
            {
                canSprint = true;
            }
        }

        // Move the player based on input and current speed.
        Vector3 moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * currentSpeed * Time.deltaTime);

        // Handle jumping.
        if (Input.GetButtonDown("Jump") && jumpCount < jump)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }

        // Apply gravity.
        playerVel.y += gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        // Play footsteps if moving on the ground.
        if (controller.isGrounded && moveDir.normalized.magnitude > 0.3f && !playingSteps)
        {
            StartCoroutine(playSteps());
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
        if (isInvincible || !isAlive) return;

        isTakingDamage = true;
        StartCoroutine(ShowDamageIndicator());
        HP -= amount;
        HP = Mathf.Clamp(HP, 0, HPOrig);
        updatePlayerUI();

        float currentTime = Time.time;
        if (currentTime - lastHurtTime > hurtSoundCooldown)
        {
            aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
            lastHurtTime = currentTime;
        }

        if (HP <= 0)
        {
            aud.PlayOneShot(audLose[Random.Range(0, audLose.Length)], audLoseVol);
            Die();
        }
    }
    public void Die()
    {
        //StartCoroutine(PlayerDeathAnim());
        StartCoroutine(RotateTowardsGround());
        StartCoroutine(DeathDelay());
        GlobalWeaponsStatsManager.Instance.CanShoot = false;
        isInvincible = true;
    }
    IEnumerator RotateTowardsGround() //remove if you get animations to work
    {
        canMove = false;  
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 0, 90); 

        float duration = 1.0f; 
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null; 
        }
        transform.rotation = endRotation; 
    }

    public IEnumerator ShowDamageIndicator()
    {

        gameManager.instance.damageIndicator.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        gameManager.instance.damageIndicator.SetActive(false);
        isTakingDamage = false;
    }
    public IEnumerator DeathDelay() // put back in die if you get animations to work
    {
        yield return new WaitForSeconds(1.5f);
        int creditsToDeduct = credits / 2;
        credits -= creditsToDeduct;
        gameManager.instance.updateCreditsUI();
        gameManager.instance.CancelAndResetTimer();
        gameManager.instance.StopAllCoroutines();
        gameManager.instance.isResetting = false;
        gameManager.instance.teleportEffect.Clear();
        gameManager.instance.teleportEffect.Stop();
        gameManager.instance.DisplayCreditsLostOnDeath(creditsToDeduct);
        TeleportToSpawn();
        isInvincible = false;
        updatePlayerUI();
        HP = HPOrig;
    }
    IEnumerator playSteps()
    {
        playingSteps = true;

        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        if (!isSliding && !isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);

        playingSteps = false;
    }

    public void updatePlayerUI()
    {
        gameManager.instance.healthBar.fillAmount = (float)HP / HPOrig;
        gameManager.instance.staminaBar.fillAmount = (float)currentStamina / maxStamina;
        gameManager.instance.damageBar.fillAmount = Mathf.Lerp(gameManager.instance.damageBar.fillAmount, gameManager.instance.healthBar.fillAmount, 0.01f);
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
        if (!canMove) return; // remove if you get animations to work
        if (currentStamina >= slideStaminaCost)
        {
            isSliding = true;
            slideTimer = maxSlideTime;
            controller.height = slideYScale; 
            currentStamina -= slideStaminaCost;
        }
        
    }

    private void SlideMovement()
    {
        if (slideTimer > 0)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;
            Vector3 slideDirection = (transform.forward * inputDirection.z + transform.right * inputDirection.x) * slideSpeed;
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
        if (Physics.SphereCast(playerCamera.transform.position, sphereRadius, playerCamera.transform.forward, out hit, interactionRange))
        {
            DoorOpenClose door = hit.collider.GetComponent<DoorOpenClose>();
            DoorOpenClosewithcost doorWithCost = hit.collider.GetComponent<DoorOpenClosewithcost>();
            Openclosedoorwithcostnotimer doorWithCostNoTimer = hit.collider.GetComponent<Openclosedoorwithcostnotimer>();

            if (door != null && currentlyAimedDoor != door)
            {
                if (currentlyAimedDoor != null)
                {
                    currentlyAimedDoor.SetPlayerAimingAtDoor(false);
                }
                currentlyAimedDoor = door;
                currentlyAimedDoor.SetPlayerAimingAtDoor(true);
            }
            else if (doorWithCost != null && currentlyAimedDoorWithCost != doorWithCost)
            {
                if (currentlyAimedDoorWithCost != null)
                {
                    currentlyAimedDoorWithCost.SetPlayerAimingAtDoor(false);
                }
                currentlyAimedDoorWithCost = doorWithCost;
                currentlyAimedDoorWithCost.SetPlayerAimingAtDoor(true);
            }
            else if (doorWithCostNoTimer != null && currentlyAimedDoorWithCostNoTimer != doorWithCostNoTimer)
            {
                if (currentlyAimedDoorWithCostNoTimer != null)
                {
                    currentlyAimedDoorWithCostNoTimer.SetPlayerAimingAtDoor(false);
                }
                currentlyAimedDoorWithCostNoTimer = doorWithCostNoTimer;
                currentlyAimedDoorWithCostNoTimer.SetPlayerAimingAtDoor(true);
            }
        }
        else
        {
            if (currentlyAimedDoor != null)
            {
                currentlyAimedDoor.SetPlayerAimingAtDoor(false);
                currentlyAimedDoor = null;
            }
            if (currentlyAimedDoorWithCost != null)
            {
                currentlyAimedDoorWithCost.SetPlayerAimingAtDoor(false);
                currentlyAimedDoorWithCost = null;
            }
            if (currentlyAimedDoorWithCostNoTimer != null)
            {
                currentlyAimedDoorWithCostNoTimer.SetPlayerAimingAtDoor(false);
                currentlyAimedDoorWithCostNoTimer = null;
            }
        }
    }

    IEnumerator PerformMeleeAttack()
    {
        if (!canMove) yield break;  
        if (!isMeleeReady || (currentWeapon != null && (currentWeapon.isReloading || currentWeapon.isRecoiling)))
        {
            yield break;  
        }        
        isMeleeReady = false;  
        equipScript.DeactivateAllGuns();
        meleeWeapon.SetActive(true);
        meleeAnimator.SetTrigger("Attack");
        if (Time.time >= lastDashTime + dashCooldown)
            StartCoroutine(PerformDash());
        yield return new WaitForSeconds(0.2f);  
        ApplyMeleeDamage();
        yield return new WaitForSeconds(0.4f);  
        meleeWeapon.SetActive(false);
        equipScript.ReactivateAllGuns();
        isMeleeReady = true;  
    }
    IEnumerator PerformDash()
    {
        if (Time.time < lastDashTime + dashCooldown) 
            yield break;
        isDashing = true;
        lastDashTime = Time.time; 
        Vector3 horizontalInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        horizontalInput = transform.TransformDirection(horizontalInput.normalized);
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;  
        forward.Normalize();
        float vertical = playerCamera.transform.forward.y;
        vertical = (vertical > verticalThreshold) ? vertical : 0;  
        dashDirection = horizontalInput + new Vector3(0, vertical, 0);
        dashDirection = dashDirection.normalized * dashSpeed;
        float dashEndTime = Time.time + dashDuration;
        while (Time.time < dashEndTime)
        {
            controller.Move(dashDirection * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
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

    private void TeleportToSpawn()
    {           
        if (controller.enabled)
        {
            controller.enabled = false;
            transform.position = gameManager.instance.startingSpawn.transform.position;
            StartCoroutine(ResetRotationAfterDelay(0f)); //remove if you get animations to work
            controller.enabled = true;
        }
        else
        {
            transform.position = gameManager.instance.startingSpawn.transform.position;
            StartCoroutine(ResetRotationAfterDelay(0f)); //remove if you get animations to work
            controller.enabled = true;
        }
    }
    IEnumerator ResetRotationAfterDelay(float delay) //remove if you get animations to work
    {
        yield return new WaitForSeconds(delay);

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.identity; 
        float duration = 1.0f; 
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null; 
        }
        canMove = true;
        GlobalWeaponsStatsManager.Instance.CanShoot = true;
        transform.rotation = endRotation; 

    }

    void CheckForButtonPress()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionRange))
            {
                if (hit.collider.CompareTag("Button")) 
                {
                    TriggerButtonAction(hit.collider.gameObject);
                }
                VendingMachine vendingMachine = hit.collider.GetComponent<VendingMachine>();
                if (vendingMachine != null)
                {
                   if (credits >= 50)
                    {
                        credits -= 50;
                        gameManager.instance.updateCreditsUI();
                        vendingMachine.DispensePowerUp();
                    }
                }
            }
        }
    }

    void TriggerButtonAction(GameObject button)
    {
        Animator anim = button.GetComponent<Animator>();
        if (anim != null)
        {
            Debug.Log("Attempting to trigger FloorRise on Animator: " + anim.gameObject.name);
            anim.SetTrigger("FloorRise");
        }
        else
        {
            Debug.Log("No Animator found on the button GameObject.");
        }
    }


    public void AddCoins() // used beacuse im tired of having to add coins manually
    {
        credits += 1000;
        gameManager.instance.updateCreditsUI();
    }
    public void CoinDrop() 
    {
        int randomCoins = UnityEngine.Random.Range(10, 50);
        credits += randomCoins;
        gameManager.instance.updateCreditsUI();
    }

    //private IEnumerator PlayerDeathAnim()
    //{
    //    GetComponent<CharacterController>().enabled = false;
    //    anim.SetTrigger("Death");
    //    aud.PlayOneShot(audDead[Random.Range(0, audDead.Length)], audDeadVol);
    //    GetComponentInChildren<Animator>().enabled = true;
    //    yield return new WaitForSeconds(1f);
    //    TeleportToSpawn();

    //    GetComponent<CharacterController>().enabled = true;
    //}


    private IEnumerator FadeOut()
    {
        float alphaVal = gameManager.instance.lowHP.color.a;
        Color current = gameManager.instance.lowHP.color;

        while (gameManager.instance.lowHP.color.a > 0)
        {
            alphaVal -= 0.09f;
            current.a = alphaVal;
            gameManager.instance.lowHP.color = current;

            yield return new WaitForSeconds(2f);
            gameManager.instance.lowHP.enabled = false;
        }
    }

    private IEnumerator FadeIn()
    {
        float alphaVal = gameManager.instance.lowHP.color.a;
        Color current = gameManager.instance.lowHP.color;

        while (gameManager.instance.lowHP.color.a < 1)
        {
            gameManager.instance.lowHP.enabled = true;
            alphaVal += 0.09f;
            current.a = alphaVal;
            gameManager.instance.lowHP.color = current;

            yield return new WaitForSeconds(0.08f);
        }
    }

    
}




