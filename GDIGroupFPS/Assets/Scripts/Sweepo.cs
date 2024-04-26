using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;


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
    private float timer;
    private int interactionCount = 0;
    private bool isChasing = false;
    public float interactionCooldown = 6.0f;
    private bool canInteract = true;
    private bool hasChasedOnce = false;
    private bool deepDialogueShown = false;
    private bool finalDeepDialogueShown = false;
    public  AudioSource deepthought;

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
        if (interactionCount == 10 && !deepDialogueShown)
        {
            ShowDeepDialogue();
            deepDialogueShown = true; 
        }
        else
        {
            interactionText.enabled = true;
            StartCoroutine(HideTextAfterTime(6f));
        }
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
    IEnumerator WaitAndShowFinalThought(float time)
    {
        yield return new WaitForSeconds(time);       
            ShowFinalDeepDialogue();
            finalDeepDialogueShown = true;       
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
        StartCoroutine(HideTextAfterTime(6f));
        agent.speed = chaseSpeed;
        isChasing = true;
        //interactionCount = 0;
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
}