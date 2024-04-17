using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class enemyMeleeAI : MonoBehaviour, IDamage
{
    [Header("----- Enemy Items -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;
    [SerializeField] Collider meleeCol;
    [SerializeField] AudioSource aud;

    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int viewCone;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] float animSpeedTrans;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [Range(0, 15)][SerializeField] int meleeDmg;
    [SerializeField] float slashRate;

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
    [SerializeField] AudioClip[] audSlashHit;
    [Range(0, 1)][SerializeField] float audSlashHitVol;
    [SerializeField] AudioClip[] audDamaged;
    [Range(0, 1)][SerializeField] float audDamagedVol;
    [SerializeField] AudioClip[] audDeath;
    [Range(0, 1)][SerializeField] float audDeathVol;

    public Transform damagePopupPrefab;

    bool isSlashing;
    bool playerInRange;
    float angleToPlayer;
    Vector3 playerDirection;
    float stoppingDistOrg;
    Vector3 startingPos;
    bool destinationChosen;
    public float scaleDuration = 1f;

    public waveSpawner whereISpawned;


    void Start()
    {
        stoppingDistOrg = agent.stoppingDistance;
        agent.stoppingDistance = 0;
        startingPos = transform.position;
    }

    void Update()
    {
        float animSpeed = agent.velocity.normalized.magnitude;

        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans));

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
        Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDirection, Color.yellow);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.stoppingDistance = stoppingDistOrg;
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (playerInRange && !isSlashing)
                {
                    StartCoroutine(slash());
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

    //Try IK to move pelvis only
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, transform.position.y, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);

    }

    IEnumerator slash()
    {
        isSlashing = true;
        anim.SetTrigger("Slash");

        yield return new WaitForSeconds(slashRate);
        isSlashing = false;
    }


    public void takeDamage(int amount)
    {
        HP -= amount;
        anim.SetTrigger("Damage");
        aud.PlayOneShot(audDamaged[Random.Range(0, audDamaged.Length)], audDamagedVol);
        DamagePopup.Create(damagePopupPrefab, transform, amount);
        agent.SetDestination(gameManager.instance.player.transform.position);
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            StartCoroutine(onDeath());
            if (whereISpawned)
                whereISpawned.firstDeath = false;
            
        }
    }

    IEnumerator onDeath()
    {
        agent.isStopped = true;
        StopCoroutine(Roam());
        playerInRange = false;
        anim.SetTrigger("Death");
        aud.PlayOneShot(audDeath[Random.Range(0, audDeath.Length)], audDeathVol);
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        yield return new WaitForSeconds(2f);
        StartCoroutine(ScaleToZeroCoroutine());
        yield return new WaitForSeconds(2f);

        if (whereISpawned)
        {
            whereISpawned.updateEnemyNumber();
        }
        Destroy(gameObject);
        gameManager.instance.playerScript.credits += creditGainOnDeath;
        gameManager.instance.updateCreditsUI();
        TryDropItem(dropObject, dropChancePercentage);
        TryDropItem(dropObject2, dropChancePercentage2);
        TryDropItem(dropObject3, dropChancePercentage3);

    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
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

    public void meleeColOn()
    {
        meleeCol.enabled = true;
        aud.PlayOneShot(audSlashHit[Random.Range(0, audSlashHit.Length)], audSlashHitVol);
    }

    public void meleeColOff()
    {
        meleeCol.enabled = false;
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
