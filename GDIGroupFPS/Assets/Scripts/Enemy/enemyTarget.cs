using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyTarget : MonoBehaviour, IDamage
{
    [Header("---- Assets ----")]
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource aud;

    [Header("---- Stats ----")]
    [SerializeField] int HP;
    [SerializeField] int speed;

    [Header("---- Credits Settings ----")]
    [SerializeField] private int creditGainOnDeath;

    [Header("---- Audio ----")]
    [SerializeField] AudioClip[] audRun;
    [Range(0, 1)][SerializeField] float audRunVol;
    [SerializeField] AudioClip[] audShooting;
    [Range(0, 1)][SerializeField] float audShootingVol;
    [SerializeField] AudioClip[] audDamaged;
    [Range(0, 1)][SerializeField] float audDamagedVol;
    [SerializeField] AudioClip[] audDeath;
    [Range(0, 1)][SerializeField] float audDeathVol;


    
    Vector3 startingPos;
    public Transform damagePopupPrefab;
    public float scaleDuration = 1f;
    private bool isDying = false;


    void Start()
    {
        
        startingPos = transform.position;
        
    }

    void Update()
    {
        
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        anim.SetTrigger("Damage");
        aud.PlayOneShot(audDamaged[Random.Range(0, audDamaged.Length)], audDamagedVol);
        DamagePopup.Create(damagePopupPrefab, transform, amount);
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
        if (isDying) yield break;
        isDying = true;
        anim.SetTrigger("Death");
        aud.PlayOneShot(audDeath[Random.Range(0, audDeath.Length)], audDeathVol);
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        gameManager.instance.playerScript.credits += creditGainOnDeath;
        gameManager.instance.updateCreditsUI();
        yield return new WaitForSeconds(2f);
        StartCoroutine(ScaleToZeroCoroutine());
        yield return new WaitForSeconds(2f);

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
