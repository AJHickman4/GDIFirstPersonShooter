using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header ("---- Assets ----")]
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
    [SerializeField] float animSpeedTrans;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;

    [Header("---- Bullet Assets ----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [Header("---- Waypoints ----")]
    [SerializeField] GameObject[] waypointArray;
    [SerializeField] int currWaypoint = 0;
    //[SerializeField] float waypointSpeed = 1.0f;

    [Header("---- Audio ----")]
    [SerializeField] AudioClip[] audRun;
    [Range(0, 1)][SerializeField] float audRunVol;
    [SerializeField] AudioClip[] audShooting;
    [Range(0, 1)][SerializeField] float audShootingVol;

    bool isShooting;
    bool playerInRange;
    float angleToPlayer;
    Vector3 playerDirection;
    float stoppingDistOrg;
    Vector3 startingPos;
    bool destinationChosen;


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

            Vector3 waypoint = waypointArray[currWaypoint].transform.position;
            currWaypoint++;
            if (currWaypoint >= waypointArray.Length)
                currWaypoint = 0;
            transform.position = Vector3.MoveTowards(transform.position, waypointArray[currWaypoint].transform.position, speed * Time.deltaTime);

            NavMeshHit hit;
            NavMesh.SamplePosition(waypoint, out hit, roamDist, 1);
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

    //Try IK to move pelvis only
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, transform.position.y, playerDirection.z));
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
        agent.SetDestination(gameManager.instance.player.transform.position);
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
        playerInRange = false;
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        gameManager.instance.playerScript.credits += 2;
        gameManager.instance.updateCreditsUI();
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
}
