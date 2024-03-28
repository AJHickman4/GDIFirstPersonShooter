using UnityEngine;

public class MysteryBox : MonoBehaviour
{
    public GameObject[] weaponPrefabs;
    [Range(1, 3)] public float spawnOffset = 1.0f;
    public int cost = 25; 

    private playerController playerController; 

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            float sphereRadius = 0.5f;
            if (Physics.SphereCast(Camera.main.transform.position, sphereRadius, Camera.main.transform.forward, out hit, 5f))
            {
                if (hit.collider.CompareTag("MysteryBox"))
                {
                    hit.collider.GetComponent<MysteryBox>().Interact();
                }
            }
        }
    }

    private void Interact()
    {
        if (playerController == null)
        {
            return;
        }
        if (playerController.credits >= cost)
        {
            playerController.credits -= cost; 
            int randomIndex = Random.Range(0, weaponPrefabs.Length);
            GameObject weaponToSpawn = weaponPrefabs[randomIndex];
            Vector3 spawnPosition = transform.position + Vector3.up * spawnOffset;
            GameObject spawnedWeapon = Instantiate(weaponToSpawn, spawnPosition, Quaternion.Euler(0, 180, 180));
        }
        else
        {
            Debug.Log("Not enough credits to interact with the Mystery Box");
        }
    }
}