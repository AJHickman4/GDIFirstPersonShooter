using System.Collections;

using UnityEngine;

public class BreakableBox : MonoBehaviour, IDamage
{
    [Header("----- Compenents -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] Renderer boxRenderer;
    [SerializeField] private int health = 10;
    [SerializeField] GameObject explosion;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] BoxCollider boxCollider;


    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] audExplosion;
    [Range(0, 1)][SerializeField] float audExplosionVol;

    [Header("----- Drop Settings -----")]
    [SerializeField] GameObject dropObject;
    [Range(0, 100)][SerializeField] int dropChancePercentage = 25;  // 25% chance to drop

    private void Awake()
    {
        
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        if (meshCollider == null)
        {
            meshCollider = GetComponent<MeshCollider>();
        }
        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider>();
        }
    }
    public void takeDamage(int damage)
    {
        if (health <= 0) 
        return;

        health -= damage;
        StartCoroutine(FlashRed());

        if (health <= 0)
        {
            if (explosion != null)
            {
                Instantiate(explosion, transform.position, transform.rotation);
                aud.PlayOneShot(audExplosion[Random.Range(0, audExplosion.Length)], audExplosionVol);
            }


            StartCoroutine(DestroyAfterDelay());

        }
    }

    private IEnumerator FlashRed()
    {
        boxRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        boxRenderer.material.color = Color.white;
    }

    private IEnumerator DestroyAfterDelay()
    {
        meshRenderer.enabled = false;
        
        if (meshCollider != null)
        {
            meshCollider.enabled = false;
        }
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
        yield return new WaitForSeconds(1f); // Wait for 1 second before destroying the object

        if (dropObject != null && Random.Range(0, 100) < dropChancePercentage)
        {
            Instantiate(dropObject, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

}
