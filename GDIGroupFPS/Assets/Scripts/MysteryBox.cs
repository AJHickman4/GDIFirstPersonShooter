using UnityEngine;
using System.Collections;
using TMPro;

public class MysteryBox : MonoBehaviour
{
    public GameObject[] weaponPrefabs;
    [Range(1, 3)] public float spawnOffset = 1.0f;
    public int cost = 25;

    private playerController playerController;
    private bool isAvailable = true;
    private float cooldown = 2.0f;  
    private float floatAmplitude = 0.2f;
    public float zOffset = 0.5f;
    public TextMeshProUGUI notEnoughCreditsText;
    public AudioClip interactSound;
    public AudioSource audioSource;
    public bool isDispensing = false;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
        if (notEnoughCreditsText != null)
            notEnoughCreditsText.gameObject.SetActive(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            float lineDistance = 5f; 
            Vector3 rayOrigin = Camera.main.transform.position; 
            Vector3 rayDirection = Camera.main.transform.forward;

            if (Physics.Linecast(rayOrigin, rayOrigin + rayDirection * lineDistance, out hit))
            {
                if (hit.collider.CompareTag("MysteryBox") && isAvailable)
                {
                    Interact();
                    
                }
            }
        }
    }
    IEnumerator HideCreditText()
    {
        yield return new WaitForSeconds(3);  
        if (notEnoughCreditsText != null)
            notEnoughCreditsText.gameObject.SetActive(false);
    }
    
    private void Interact()
    {
        if (playerController == null)
            return;

        if (playerController.credits >= cost)
        {
            playerController.credits -= cost;
            gameManager.instance.updateCreditsUI();
            StartCoroutine(DispenseWeapon());
            audioSource.PlayOneShot(interactSound);
        }
        else
        {
            int neededCredits = cost - playerController.credits;
            if (notEnoughCreditsText != null)
            {
                notEnoughCreditsText.text = $"You need {neededCredits} more credits!";
                notEnoughCreditsText.gameObject.SetActive(true);
            }
            StartCoroutine(HideCreditText());
        }
    }

    private IEnumerator DispenseWeapon()
    {
        isAvailable = false;
        isDispensing = true;
        float displayDuration = 0.9f;
        float floatFrequency = 0.5f;

        for (int i = 0; i < 5; i++)
        {
            GameObject tempWeapon = weaponPrefabs[Random.Range(0, weaponPrefabs.Length)];
            Vector3 tempPosition = transform.position + Vector3.up * (spawnOffset + 1.0f) + Vector3.forward * zOffset;
            GameObject displayedWeapon = Instantiate(tempWeapon, tempPosition, Quaternion.Euler(180, 90, 0));

            Collider weaponCollider = displayedWeapon.GetComponent<Collider>();
            if (weaponCollider != null)
                weaponCollider.enabled = false;

            float elapsedTime = 0;
            while (elapsedTime < displayDuration)
            {
                elapsedTime += Time.deltaTime;
                float newY = Mathf.Sin(floatFrequency * elapsedTime * Mathf.PI) * floatAmplitude;
                displayedWeapon.transform.position = new Vector3(displayedWeapon.transform.position.x, transform.position.y + spawnOffset + 1.0f + newY, displayedWeapon.transform.position.z);
                yield return null;
            }

            Destroy(displayedWeapon);
        }

        int randomIndex = Random.Range(0, weaponPrefabs.Length);
        GameObject weaponToSpawn = weaponPrefabs[randomIndex];
        Vector3 spawnPosition = transform.position + Vector3.up * (spawnOffset + 1.0f) + Vector3.forward * zOffset;
        GameObject finalWeapon = Instantiate(weaponToSpawn, spawnPosition, Quaternion.Euler(180, 90, 0));

        Collider finalCollider = finalWeapon.GetComponent<Collider>();
        if (finalCollider != null)
            finalCollider.enabled = true;

        isDispensing = false;
        yield return new WaitForSeconds(cooldown);

        isAvailable = true;
    }
}