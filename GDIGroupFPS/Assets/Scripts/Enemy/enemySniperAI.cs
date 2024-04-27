using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class enemySniperAI : MonoBehaviour, IDamage
{
    [Header("---- Assets ----")]
    [SerializeField] private Renderer model;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform shootPos;
    [SerializeField] private AudioSource aud;
    [SerializeField] Transform headPos;


    [Header("---- Stats ----")]
    [SerializeField] private int HP;
    [SerializeField] private float attackRange;
    [SerializeField] private float chargeTime;
    [SerializeField] int viewCone;
    [SerializeField] private float shootRate;
    [SerializeField] private float fleeDistance;
    [SerializeField] private float fleeSpeed;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] float animSpeedTrans;
    [SerializeField] int faceTargetSpeed;

    [Header("---- Bullet Assets ----")]
    [SerializeField] private GameObject bulletPrefab;
    public float bulletSpeed = 10f;

    [Header("---- Audio ----")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] AudioClip[] audRun;
    [Range(0, 1)][SerializeField] float audRunVol;
    [SerializeField] AudioClip[] audShooting;
    [Range(0, 1)][SerializeField] float audShootingVol;
    [SerializeField] AudioClip[] audDamaged;
    [Range(0, 1)][SerializeField] float audDamagedVol;
    [SerializeField] AudioClip[] audDeath;
    [Range(0, 1)][SerializeField] float audDeathVol;
    
    [Header("---- Credits Settings ----")]
    [SerializeField] private int creditGainOnDeath = 5;

    [Header("----- Drop Settings -----")]
    [SerializeField] GameObject dropObject;
    [Range(0, 100)][SerializeField] int dropChancePercentage = 25;  // 25% chance to drop

    [SerializeField] GameObject dropObject2;
    [Range(0, 100)][SerializeField] int dropChancePercentage2 = 25;

    [SerializeField] GameObject dropObject3;
    [Range(0, 100)][SerializeField] int dropChancePercentage3 = 25;

    [SerializeField] private bool shouldFleeWhenDamaged = true;

    public Transform damagePopupPrefab;

    public float scaleDuration = 1f;
    private bool isDying = false;

    bool playerInRange;
    Vector3 startingPos;
    bool destinationChosen;
    private bool isCharging;
    private bool isFleeing;
    private float chargeTimer;
    private float shootTimer;
    private Vector3 startPosition;
    Vector3 playerDirection;
    private Transform player;
    float angleToPlayer;



    void Start()
    {
        agent.updateRotation = false;
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = fleeSpeed;
        StartCoroutine(Roam());
        int uniquePriority = PriorityManager.GetUniquePriority();
        if (uniquePriority != -1)
        {
            agent.avoidancePriority = uniquePriority;
        }
        else
        {
            //Debug
        }
    }

    void Update()
    {
        //Debug.DrawLine(transform.position, transform.position + (player.position - transform.position).normalized * 10, Color.green);
        if (player == null) return;  
        playerController playerMovement = player.GetComponent<playerController>();
        Vector3 playerVelocity = playerMovement.velocity;
        float bulletTravelTime = Vector3.Distance(transform.position, player.position) / bulletSpeed;  
        Vector3 futurePosition = player.position + playerVelocity * bulletTravelTime; 

        float animSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans));
        float distanceToPlayer = Vector3.Distance(transform.position, player.position); 
        if (distanceToPlayer <= fleeDistance)
        {
            Flee();
        }
        else if (distanceToPlayer <= attackRange && shootTimer <= 0) 
        {
            if (playerInRange && canSeePlayer())
            {
                faceTarget(); 
            }
            if (!isCharging)  
            {
                StartCharge(); 
            }
            else if (canSeePlayer())  
            {
                ContinueCharge();
            }
        }
        else  
        {
            isCharging = false;
            isFleeing = false;
        }

        // Decrease shoot timer
        if (shootTimer > 0)
            shootTimer -= Time.deltaTime;
    }
    void PredictiveAim(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * faceTargetSpeed);
    }
    private Vector3 PredictPlayerPosition()
    {
        playerController playerMovement = player.GetComponent<playerController>(); 
        Vector3 playerVelocity = playerMovement.velocity;
        float bulletTravelTime = Vector3.Distance(transform.position, player.position) / bulletSpeed;
        Vector3 futurePosition = player.position + playerVelocity * bulletTravelTime;
        return futurePosition;
    }
    private void StartCharge()
    {
        isCharging = true;
        chargeTimer = chargeTime;
    }

    private void ContinueCharge()
    {
        Vector3 futurePosition = PredictPlayerPosition(); 
        PredictiveAim(futurePosition);
        chargeTimer -= Time.deltaTime;
        if (chargeTimer <= 0)
        {
            StartCoroutine(Shoot());
            agent.enabled = false;
        }
    }

    IEnumerator Shoot()
    {
        if (isDying) yield break;
        faceTarget(); 
        isCharging = false;
        shootTimer = shootRate;
        anim.SetTrigger("Shoot");
        aud.PlayOneShot(shootSound);
        yield return new WaitForSeconds(shootRate);
        agent.enabled = true; 
    }
    public void createBullet()
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, shootPos.position, Quaternion.LookRotation(shootPos.forward));
        bulletInstance.GetComponent<Rigidbody>().velocity = shootPos.forward * bulletSpeed;
    }

    private void FleeFromPlayer()
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            Vector3 fleeDirection = (transform.position - player.position).normalized;
            Vector3 fleePosition = transform.position + fleeDirection * fleeDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleePosition, out hit, fleeDistance, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
        else
        {
            return;
        }
    }

    private void Flee()
    {
        if (isFleeing || agent == null || !agent.isActiveAndEnabled) return;

        isFleeing = true;
        Vector3 fleeDirection = (transform.position - player.position).normalized;
        Vector3 fleePosition = startPosition + fleeDirection * fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePosition, out hit, fleeDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            return;
        }
    }
    bool canSeePlayer()
    {
        Vector3 directionToPlayer = player.position - headPos.position;
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, directionToPlayer.normalized, out hit, attackRange))
        {
            return hit.collider.gameObject == player.gameObject && Vector3.Angle(directionToPlayer, transform.forward) <= viewCone;
        }
        return false;
    }
    void faceTarget()
    {
        Vector3 targetHeightOffset = new Vector3(0, player.GetComponent<Collider>().bounds.extents.y, 0); 
        Vector3 playerCenter = player.position + targetHeightOffset;
        Vector3 directionToPlayerCenter = (playerCenter - headPos.position).normalized;
        Quaternion targetRotation = isCharging ? Quaternion.LookRotation(directionToPlayerCenter) :
                                                 Quaternion.Euler(0, Quaternion.LookRotation(directionToPlayerCenter).eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * faceTargetSpeed);
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        faceTarget();
        anim.SetTrigger("Damage");
        aud.PlayOneShot(audDamaged[Random.Range(0, audDamaged.Length)], audDamagedVol);
        DamagePopup.Create(damagePopupPrefab, transform, amount);
        if (shouldFleeWhenDamaged) FleeFromPlayer();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            StartCoroutine(onDeath());
        }
    }
    IEnumerator onDeath()
    {
        if (isDying) yield break;
        StopCoroutine(Shoot());
        isDying = true;
        StopCoroutine(Roam());
        playerInRange = false;
        anim.SetTrigger("Death");
        PriorityManager.ReleasePriority(agent.avoidancePriority);
        aud.PlayOneShot(audDeath[Random.Range(0, audDeath.Length)], audDeathVol);
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        gameManager.instance.playerScript.credits += creditGainOnDeath;
        gameManager.instance.updateCreditsUI();
        yield return new WaitForSeconds(2f);
        StartCoroutine(ScaleToZeroCoroutine());
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        TryDropItem(dropObject, dropChancePercentage);
        TryDropItem(dropObject2, dropChancePercentage2);
        TryDropItem(dropObject3, dropChancePercentage3);

    }
    IEnumerator ScaleToZeroCoroutine()
    {
        float timer = 0f;
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;

        while (timer < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / scaleDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }
    private void TryDropItem(GameObject item, int chance)
    {
        if (item != null && Random.Range(0, 100) < chance)
        {
            GameObject droppedItem = Instantiate(item, transform.position, Quaternion.identity);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
    IEnumerator Roam()
    {
        while (!isDying)
        {
            if (!destinationChosen || agent.remainingDistance < 0.05f)
            {
                destinationChosen = true;
                agent.stoppingDistance = 0;
                yield return new WaitForSeconds(roamPauseTime);
                Vector3 randomDirection = Random.insideUnitSphere * roamDist;
                randomDirection += startingPos;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, roamDist, 1 << NavMesh.GetAreaFromName("Walkable")))
                {
                    agent.SetDestination(hit.position);
                }
                destinationChosen = false;
            }
            yield return null;
        }
    }
}

