using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyScoutAI : MonoBehaviour, IDamage
{
    [Header("---- Assets ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] AudioSource aud;

    [Header("---- Stats ----")]
    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int viewCone;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] private float fleeDistance;
    [SerializeField] private float fleeSpeed;
    [SerializeField] float animSpeedTrans;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;

    [Header("---- Bullet Assets ----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [Header("---- Grenade Assets ----")]
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] Transform grenadeSpawnPoint;
    [SerializeField] float grenadeThrowRate = 5.0f;
    [SerializeField] float grenadeThrowForce = 15.0f;
    private float lastGrenadeTime = -Mathf.Infinity;
    [Range(0, 100)][SerializeField] int grenadeChance = 20;  

    //[Header("---- Waypoints ----")]
    //[SerializeField] GameObject[] waypointArray;
    //[SerializeField] int currWaypoint = 0;
    ////[SerializeField] float waypointSpeed = 1.0f;

    [Header("---- Credits Settings ----")]
    [SerializeField] private int creditGainOnDeath = 5;

    [Header("----- Drop Settings -----")]
    [SerializeField] GameObject dropObject;
    [Range(0, 100)][SerializeField] int dropChancePercentage = 25;  // 25% chance to drop

    [SerializeField] GameObject dropObject2;
    [Range(0, 100)][SerializeField] int dropChancePercentage2 = 25;

    [SerializeField] GameObject dropObject3;
    [Range(0, 100)][SerializeField] int dropChancePercentage3 = 25;

    [Header("---- Audio ----")]
    [SerializeField] AudioClip[] audRun;
    [Range(0, 1)][SerializeField] float audRunVol;
    [SerializeField] AudioClip[] audShooting;
    [Range(0, 1)][SerializeField] float audShootingVol;
    [SerializeField] AudioClip[] audDamaged;
    [Range(0, 1)][SerializeField] float audDamagedVol;
    [SerializeField] AudioClip[] audDeath;
    [Range(0, 1)][SerializeField] float audDeathVol;

    [SerializeField] private bool shouldFleeWhenDamaged = true;

    bool isShooting;
    bool playerInRange;
    float angleToPlayer;
    Vector3 playerDirection;
    float stoppingDistOrg;
    Vector3 startingPos;
    bool destinationChosen;
    public Transform damagePopupPrefab;
    public float scaleDuration = 1f;
    private bool isDying = false;
    private bool isFleeing;
    private Transform player;
    private Vector3 startPosition;


    void Start()
    {
        stoppingDistOrg = agent.stoppingDistance;
        agent.stoppingDistance = 0;
        startingPos = transform.position;
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
        float animSpeed = agent.velocity.normalized.magnitude;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= fleeDistance)
        {
            Flee();
        }

        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans));
        if (playerInRange && canSeePlayer())
        {
            TryThrowGrenade();
        }
        if (playerInRange && !canSeePlayer())
        {
            StartCoroutine(Roam()); //If the player is in range but cannot be seen, AI should roam
        }
        else if (!playerInRange)
        {
            StartCoroutine(Roam()); //AI should roam if player isn't in range
        }
    }

    IEnumerator Roam()
    {
        if (agent.remainingDistance < 0.05f && !destinationChosen)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(roamPauseTime);

            Vector3 randomPos = Random.insideUnitSphere * roamDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);

            destinationChosen = false;
        }
    }

    bool canSeePlayer()
    {
        playerDirection = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);
        
        Debug.DrawRay(headPos.position, playerDirection, Color.yellow);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.stoppingDistance = stoppingDistOrg;
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;
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

    void TryThrowGrenade()
    {
        if (Time.time > lastGrenadeTime + grenadeThrowRate && Random.Range(0, 100) < grenadeChance)
        {
            lastGrenadeTime = Time.time;
            StartCoroutine(ThrowGrenade());
        }
    }
    IEnumerator ThrowGrenade()
    {
        anim.SetTrigger("Grenade"); 
        yield return new WaitForSeconds(1.26f); 
        GameObject grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Quaternion.identity);
        Rigidbody grenadeRb = grenade.GetComponent<Rigidbody>();
        Vector3 targetPoint = gameManager.instance.player.transform.position;
        Vector3 throwDirection = targetPoint - grenadeSpawnPoint.position;
        float heightFactor = 1.0f; 
        throwDirection.y += heightFactor * (throwDirection.magnitude / 10.0f); 
        grenadeRb.AddForce(throwDirection.normalized * grenadeThrowForce, ForceMode.Impulse);
        yield return new WaitForSeconds(grenadeThrowRate);
    }
   
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        aud.PlayOneShot(audShooting[Random.Range(0, audShooting.Length)], audShootingVol);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        anim.SetTrigger("Damage");
        aud.PlayOneShot(audDamaged[Random.Range(0, audDamaged.Length)], audDamagedVol);
        DamagePopup.Create(damagePopupPrefab, transform, amount);
        agent.SetDestination(gameManager.instance.player.transform.position);
        if (shouldFleeWhenDamaged) FleeFromPlayer();
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            StartCoroutine(onDeath());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    IEnumerator onDeath()
    {
        if (isDying) yield break;
        isDying = true;
        agent.isStopped = true;
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

        //if (whereISpawned)
        //{
        //    whereISpawned.updateEnemyNumber();
        //}

        Destroy(gameObject);
        TryDropItem(dropObject, dropChancePercentage);
        TryDropItem(dropObject2, dropChancePercentage2);
        TryDropItem(dropObject3, dropChancePercentage3);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }
    private void TryDropItem(GameObject item, int chance)
    {
        if (item != null && Random.Range(0, 100) < chance)
        {
            GameObject droppedItem = Instantiate(item, transform.position, Quaternion.identity);
        }
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

        // Ensure final scale is exactly zero
        transform.localScale = targetScale;
    }
}
