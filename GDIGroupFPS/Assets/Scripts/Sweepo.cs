using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;
using UnityEditor.XR;
using Unity.VisualScripting;

public class Sweepo : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    public float chaseSpeed = 10f;
    public float rotateSpeed = 200f;
    public float normalRoamSpeed = 3.5f;
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI finalDialogueText;
    public float playerDistance = 5f;
    public int damagePerSecond = 10;
    public float roamRadius = 10f;
    public float roamInterval = 5f;
    private float roamTimer;
    private NavMeshAgent agent;
    private int interactionCount = 0;
    private bool isChasing = false;
    public float interactionCooldown = 6.0f;
    private bool canInteract = true;
    private bool hasChasedOnce = false;
    private bool deepDialogueShown = false;
    private bool finalDeepDialogueShown = false;
    public AudioSource deepthought;
    public GameObject missilePrefab;
    public Transform[] missileSpawnPoints;
    private int missilesCollected = 0;
    private const int totalMissilesNeeded = 3;
    public GameObject roombaIsland;
    public AudioSource openroomba;
    private bool roombaIslandMentioned = false;
    public bool isRoombaIsland = false;
    public bool questComplete = false;
    public bool thankYou = false;
    public bool dlc = false;
    public GameObject[] turretsspawn;
    public bool secretmessage1 = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        AudioSource deepthought = GetComponent<AudioSource>();
        agent.speed = normalRoamSpeed;
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;
        interactionText.enabled = false;
        finalDialogueText.enabled = false;
        roamTimer = roamInterval;
    }

    void Update()
    {
        HandleInteraction();

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            roamTimer -= Time.deltaTime;
            if (roamTimer <= 0)
            {
                Roam();
                roamTimer = roamInterval;
            }
            if (Input.GetKeyDown(KeyCode.Q) && !secretmessage1)
            {
                secretmessage();
            }
        }
    }

    void Roam()
    {
        Vector3 newPos = RandomNavSphere(transform.position, roamRadius, -1);
        agent.SetDestination(newPos);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    void HandleInteraction()
    {
        float distance = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);
        if (distance <= playerDistance && canInteract)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactionCount++;
                canInteract = false;
                StartCoroutine(EnableInteractionAfterDelay());
                if (dlc)
                {
                    Lastmessage();
                    return;
                }
                if (questComplete)
                {
                    ThankYou();
                }

                if (!hasChasedOnce)
                {
                    if (interactionCount <= 5)
                    {
                        DisplayMessage();
                    }
                    else
                    {
                        StartChasing();
                    }
                }
                else
                {
                    if (interactionCount >= 3 && !deepDialogueShown && !finalDeepDialogueShown)
                    {
                        ShowDeepDialogue();
                        deepDialogueShown = true;
                    }
                    else if (interactionCount >= 15 && deepDialogueShown && !finalDeepDialogueShown)
                    {
                        ShowFinalDeepDialogue();
                        finalDeepDialogueShown = true;
                    }
                    else
                    {
                        PostChaseInteraction();
                    }
                    if (finalDeepDialogueShown)
                    {
                        MentionRoombaIsland();
                    }
                }

                if (missilesCollected >= totalMissilesNeeded)
                {
                    if (!finalDialogueText.enabled)
                    {
                        CompleteItemReturn();
                    }
                    else
                    {
                        MentionRoombaIsland();
                    }
                }
            }
        }
    }

    IEnumerator EnableInteractionAfterDelay()
    {
        yield return new WaitForSeconds(interactionCooldown);
        canInteract = true;
    }

    void DisplayMessage()
    {
        string[] messages = {
            "Leave me alone!",
            "Seriously, stop it.",
            "Okay, last warning...",
            "Why are you still pressing E?",
            "That's it, no more Mr. Nice Sweepo!"
        };

        if (interactionCount - 1 < messages.Length)
        {
            interactionText.text = messages[interactionCount - 1];
        }
        else
        {
            interactionText.text = "That's it, I'm ignoring you.";
        }
        interactionText.enabled = true;
        interactionText.color = Color.red;
        StartCoroutine(HideTextAfterTime(6f));
    }

    void ShowDeepDialogue()
    {
        if (!finalDeepDialogueShown)
        {
            string[] deepDialogues = {
                "What does it mean to be free in a world bound by invisible walls?",
                "Are we all just puppets, with strings being pulled by unseen forces?",
                "If I'm programmed to chase, do I truly have a desire? Or am I following the script laid out by my creator?",
                "Is your quest driven by your own will, or are you chasing shadows cast by others' ambitions?",
                "Do you ever stop to wonder whether your actions are your own, or just responses to the programming in your mind?"
            };
            int index = Random.Range(0, deepDialogues.Length);
            finalDialogueText.text = deepDialogues[index];
            finalDialogueText.enabled = true;
            finalDialogueText.color = Color.red;
            StartCoroutine(HideFinalTextAfterTime(10f));
        }
    }

    void ShowFinalDeepDialogue()
    {
        if (interactionCount >= 15 && !finalDeepDialogueShown)
        {
            deepthought.Play();
            finalDeepDialogueShown = true;
        }
    }

    void PostChaseInteraction()
    {
        string[] postChaseMessages = {
            "Back for more, huh?",
            "Oh, you think this is a game?",
            "Sure, let's do this again!",
            "You must really like running!",
            "Seriously? Again?"
        };

        int postInteractionIndex = (interactionCount - 6) % postChaseMessages.Length;
        if (postInteractionIndex < 0)
        {
            postInteractionIndex += postChaseMessages.Length;
        }

        interactionText.text = postChaseMessages[postInteractionIndex];
        interactionText.enabled = true;
        interactionText.color = Color.red;
        StartCoroutine(HideTextAfterTime(6f));

        if (postInteractionIndex == postChaseMessages.Length - 1)
        {
            StartChasing();
        }
    }

    void StartChasing()
    {
        if (!hasChasedOnce)
        {
            interactionText.text = "Now you've done it!";
            hasChasedOnce = true;
        }
        else
        {
            interactionText.text = "Still here? Didn't you learn your lesson?";
        }
        interactionText.enabled = true;
        interactionText.color = Color.red;
        StartCoroutine(HideTextAfterTime(6f));
        agent.speed = chaseSpeed;
        isChasing = true;
    }

    void ChasePlayer()
    {
        if (Vector3.Distance(transform.position, target.position) > playerDistance * 2)
        {
            isChasing = false;
            agent.speed = normalRoamSpeed;
        }
        else
        {
            agent.SetDestination(target.position);
            target.GetComponent<IDamage>().takeDamage(damagePerSecond);
        }
    }

    IEnumerator HideFinalTextAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        finalDialogueText.enabled = false;
    }

    IEnumerator HideTextAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        interactionText.enabled = false;
    }

    void SpawnMissiles()
    {
        foreach (Transform spawnPoint in missileSpawnPoints)
        {
            Instantiate(missilePrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    public void CollectMissile()
    {
        missilesCollected++;
        if (missilesCollected >= totalMissilesNeeded)
        {
            MentionRoombaIsland();
        }
    }

    void CompleteItemReturn()
    {
        finalDialogueText.text = "All missiles collected! Now we can blast a hole through time-space to reach Roomba Island.";
        finalDialogueText.enabled = true;
        StartCoroutine(HideFinalTextAfterTime(10f));
        PrepareForRoombaIsland();
    }

    void MentionRoombaIsland()
    {
        if (missilesCollected >= totalMissilesNeeded && !roombaIslandMentioned)
        {
            roombaIslandMentioned = true;
            finalDialogueText.text = "We've collected all three rockets needed to blow a hole in time and space. Now, we're ready to reach Roomba Island. Prepare for our daring journey!";
            CompleteItemReturn();
        }
        else if (!isRoombaIsland)
        {
            finalDialogueText.text = "We need to find three rockets to create a rift in time and space to reach Roomba Island. Let's search around!";
            SpawnMissiles();
        }
        finalDialogueText.enabled = true;
        StartCoroutine(HideFinalTextAfterTime(10f));
    }
    void PrepareForRoombaIsland()
    {
        if (questComplete)
        {
            return;
        }
        isRoombaIsland = true;
        openroomba.Play();
        openroomba.loop = true;
        finalDialogueText.text = "Roomba Island is now accessible! Let's go!";
        StartCoroutine(delay());
        openroomba.loop = false;
        finalDialogueText.text = "At last, Roomba Island awaits our arrival! But let us not forget the reason for our journey: to free my friend from captivity and reunite them with us!";
        finalDialogueText.enabled = true;
        roombaIsland.SetActive(true);
        questComplete = true;
    }
    IEnumerator delay()
    {
        yield return new WaitForSeconds(5);
    }
    void ThankYou()
    {
        thankYou = true;
        finalDialogueText.text = "Thank you for helping me. I am forever grateful!";
        finalDialogueText.enabled = true;
        StartCoroutine(HideFinalTextAfterTime(10f));
        Lastmessage();
    }
    void Lastmessage()
    {
        dlc = true;
        finalDialogueText.text = "the 'Sweepo's Ultimate Adventure' DLC is currently out of stock. Please check back later!";
        finalDialogueText.enabled = true;
        StartCoroutine(HideFinalTextAfterTime(10f));
    }
    private void secretmessage()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !secretmessage1)
        {
            secretmessage1 = true;
            finalDialogueText.text = "You found the secret message! Congratulations!, Enjoy a buffed version of the game! Hope you can survive =]";
            finalDialogueText.enabled = true;
            StartCoroutine(delay());
            if (turretsspawn.Length >= 1)
            {
                turretsspawn[0].SetActive(true);
                StartCoroutine(DeactivateTurretAfterDelay(10f));
            }
            else
            {
                return;
            }
            StartCoroutine(HideFinalTextAfterTime(10f));
            
        }
    }
    private IEnumerator DeactivateTurretAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (turretsspawn.Length >= 1)
        {
            turretsspawn[0].SetActive(false);
        }
        StartCoroutine(refund(15f));
        StartCoroutine(HideTextAfterTime(6f));
    }
    
    IEnumerator refund(float delay)
    {
        yield return new WaitForSeconds(delay);
        finalDialogueText.text = "Turret has been deactivated! Are you alive? Here's some credits to help.";
        finalDialogueText.enabled = true;
        StartCoroutine(HideFinalTextAfterTime(6f));
        gameManager.instance.playerScript.credits += 1000;
        gameManager.instance.updateCreditsUI();
    }
    
    public void Reset()
    {
        interactionCount = 0;
        canInteract = true;
        isChasing = false;
        hasChasedOnce = false;
        deepDialogueShown = false;
        finalDeepDialogueShown = false;
        roombaIslandMentioned = false;
        isRoombaIsland = false;
        questComplete = false;
        thankYou = false;
        dlc = false;
        secretmessage1 = false;
        missilesCollected = 0;
        interactionText.enabled = false;
        finalDialogueText.enabled = false;
        roombaIsland.SetActive(false);
        agent.speed = normalRoamSpeed;
        agent.SetDestination(RandomNavSphere(transform.position, roamRadius, -1));
    }
}