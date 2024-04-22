using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class FlyingRobotEnemy : MonoBehaviour, IDamage
{
    public int health = 100;
    public float detectionRange = 10.0f;
    public float meleeRange = 2.0f;
    public Transform playerTransform;
    public NavMeshAgent agent;
    private Animator animator;
    private bool isDead = false;

    public GameObject projectilePrefab;
    public Transform shootingPoint;
    public float fireRate = 2.0f;
    private float nextFireTime = 0f;

    public float meleeDamage = 30;
    public float meleeCooldown = 1.5f;
    private float nextMeleeTime = 0f;
    public AudioSource audioSource;
    public AudioClip shootingSound;
    public AudioClip meleesound;
    public AudioClip die;
    public Transform damagePopupPrefab;
    public int creditGainOnDeath = 5;

    [Header("----- Drop Settings -----")]
    [SerializeField] GameObject dropObject;
    [Range(0, 100)][SerializeField] int dropChancePercentage = 25;  // 25% chance to drop

    [SerializeField] GameObject dropObject2;
    [Range(0, 100)][SerializeField] int dropChancePercentage2 = 25;

    [SerializeField] GameObject dropObject3;
    [Range(0, 100)][SerializeField] int dropChancePercentage3 = 25;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        StartRoaming();
        audioSource = GetComponent<AudioSource>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange && HasLineOfSightToPlayer())
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
            CheckAndPerformActions(distanceToPlayer);
        }
        else
        {
            SetAnimationState(true, false, false, false);
            ContinueRoaming();
        }
    }

    public void takeDamage(int amount)
    {
        health -= amount;
        DamagePopup.Create(damagePopupPrefab, transform, amount);
        if (health <= 0)
        {
            Die();
        }
        else
        {
            FacePlayer();
            Shoot();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        SetAnimationState(false, false, true, false);
        gameManager.instance.playerScript.credits += creditGainOnDeath;
        gameManager.instance.updateCreditsUI();
        GetComponent<Collider>().enabled = false;
        PriorityManager.ReleasePriority(agent.avoidancePriority);
        agent.enabled = false;
        StartCoroutine(FallToGround());
        audioSource.PlayOneShot(die);
        TryDropItem(dropObject, dropChancePercentage);
        TryDropItem(dropObject2, dropChancePercentage2);
        TryDropItem(dropObject3, dropChancePercentage3);
    }

    IEnumerator FallToGround()
    {
        float fallTime = 1.5f;
        float startY = transform.position.y;
        float endY = 0;
        float elapsedTime = 0;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 360 * 5, 0);
        while (elapsedTime < fallTime)
        {
            float newY = Mathf.Lerp(startY, endY, (elapsedTime / fallTime));
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, (elapsedTime / fallTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, endY, transform.position.z);
        transform.rotation = endRotation;
        Destroy(gameObject, 2f);
    }

    void SetAnimationState(bool isMoving, bool isShooting, bool isDead, bool isMelee = false)
    {
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsShooting", isShooting);
        animator.SetBool("IsDead", isDead);
        animator.SetBool("IsMelee", isMelee);
    }

    void FacePlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void StartRoaming()
    {
        agent.speed = 1.5f;
        Vector3 randomDirection = Random.insideUnitSphere * detectionRange;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, detectionRange, 1);
        Vector3 finalPosition = hit.position;
        agent.SetDestination(finalPosition);
    }

    void ContinueRoaming()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartRoaming();
        }
    }

    void CheckAndPerformActions(float distanceToPlayer)
    {
        if (distanceToPlayer <= meleeRange && Time.time >= nextMeleeTime)
        {
            SetAnimationState(false, false, false, true);
            MeleeAttack();
            nextMeleeTime = Time.time + meleeCooldown;
        }
        else if (distanceToPlayer > meleeRange && Time.time >= nextFireTime && HasLineOfSightToPlayer())
        {
            FacePlayer();
            SetAnimationState(false, true, false, false);
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (projectilePrefab && shootingPoint && Time.time >= nextFireTime)
        {
            Instantiate(projectilePrefab, shootingPoint.position, Quaternion.LookRotation(playerTransform.position - shootingPoint.position));
            audioSource.PlayOneShot(shootingSound);
            nextFireTime = Time.time + fireRate;
        }
    }

    void MeleeAttack()
    {
        StartCoroutine(Delay(.5f));
        playerTransform.GetComponent<playerController>().takeDamage((int)meleeDamage);
        audioSource.PlayOneShot(meleesound);
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    bool HasLineOfSightToPlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = playerTransform.position - shootingPoint.position;
        float distanceToPlayer = Vector3.Distance(shootingPoint.position, playerTransform.position);

        if (Physics.Raycast(shootingPoint.position, directionToPlayer, out hit, distanceToPlayer))
        {
            if (hit.transform == playerTransform)
            {
                return true;
            }
        }
        return false;
    }

    private void TryDropItem(GameObject item, int chance)
    {
        if (item != null && Random.Range(0, 100) < chance)
        {
            GameObject droppedItem = Instantiate(item, transform.position, Quaternion.identity);
        }
    }
}