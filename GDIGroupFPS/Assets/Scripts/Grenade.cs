using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float delay = 3.0f;
    [SerializeField] private float blastRadius = 2.0f;
    [SerializeField] private int damage = 50;
    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material flashRedMaterial;
    private GameObject indicatorInstance;
    [SerializeField] private GameObject indicatorPrefab;


    public Renderer rend;

    private float countdown;
    private bool hasExploded = false;
    private bool soundPlayed = false;

    void Start()
    {
        countdown = delay;
        audioSource = GetComponent<AudioSource>();
        SetMaterial(defaultMaterial);
        indicatorInstance = Instantiate(indicatorPrefab, transform.position, Quaternion.identity);
        indicatorInstance.transform.localScale = new Vector3(blastRadius * 1, 0.1f, blastRadius * 1); 
        indicatorInstance.SetActive(false);
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 1f && !indicatorInstance.activeSelf)
        {
            indicatorInstance.SetActive(true);
        }
        if (indicatorInstance.activeSelf)
        {
            indicatorInstance.transform.position = new Vector3(transform.position.x, 0.001f, transform.position.z);
            indicatorInstance.transform.rotation = Quaternion.Euler(0, 0, 0); 
        }

        if (countdown <= 0.5f && !soundPlayed)
        {
            audioSource.PlayOneShot(explosionSound);
            soundPlayed = true;
        }

        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
        if (countdown <= 0.1f && indicatorInstance.activeSelf)
        {
            indicatorInstance.SetActive(false);
        }
    }

    private void Explode()
    {
        GameObject explosionInstance = null;
        if (explosionEffect != null)
        {
            explosionInstance = Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius, damageLayer);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag("Player"))
            {
                IDamage dmg = nearbyObject.GetComponent<IDamage>();
                if (dmg != null)
                {
                    dmg.takeDamage(damage);
                }
            }
        }
        float destroyDelay = Mathf.Max(0.5f + explosionSound.length, 0f);
        Destroy(gameObject, destroyDelay);
        float explosionEffectDuration = 3.0f; 
        if (explosionInstance != null)
        {
            Destroy(explosionInstance, explosionEffectDuration);
        }
    }

    private void SetMaterial(Material mat)
    {
        Material[] materials = rend.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = mat;
        }
        rend.materials = materials;
    }
    private void OnDestroy()
    {
        if (indicatorInstance != null)
        {
            Destroy(indicatorInstance);
        }
    }
}