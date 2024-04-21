using UnityEngine.AI;
using UnityEngine;
using System.Collections;

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

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        StartRoaming();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        FacePlayer();

        if (distanceToPlayer <= meleeRange)
        {
            SetAnimationState(false, false, false, true);
            if (Time.time >= nextMeleeTime)
            {
                MeleeAttack();
                nextMeleeTime = Time.time + meleeCooldown;
            }
        }
        else if (distanceToPlayer <= detectionRange)
        {
            SetAnimationState(false, true, false, false);
            if (Time.time >= nextFireTime && HasLineOfSightToPlayer())
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
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
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        SetAnimationState(false, false, true, false);
        agent.enabled = false;
        Destroy(gameObject, 3f);
        audioSource.PlayOneShot(die);
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

    void Shoot()
    {
        if (projectilePrefab && shootingPoint && HasLineOfSightToPlayer())
        {
            Instantiate(projectilePrefab, shootingPoint.position, Quaternion.LookRotation(playerTransform.position - shootingPoint.position));
            audioSource.PlayOneShot(shootingSound);
        }
    }

    void MeleeAttack()
    {
        StartCoroutine(delay(.5f));
        playerTransform.GetComponent<playerController>().takeDamage((int)meleeDamage);
        audioSource.PlayOneShot(meleesound);
    }

    IEnumerator delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    bool HasLineOfSightToPlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = playerTransform.position - shootingPoint.position;

        if (Physics.Raycast(shootingPoint.position, directionToPlayer, out hit, detectionRange))
        {
            if (hit.transform == playerTransform)
            {
                return true;
            }
        }
        return false;
    }
}