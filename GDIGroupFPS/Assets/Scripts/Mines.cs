using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mines : MonoBehaviour, IDamage
{
    public Transform player;
    public float moveSpeed = 2.0f;
    public float explosionRange = 1.0f;
    public int damage = 50;
    public float detectionRange = 5.0f;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 0.5f;
    public int hp = 100;

    private playerController playerHealth;
    private bool isExploded = false;
    private Vector3 originalPosition;
    private bool isPlayerInRange = false;
    public ParticleSystem explosionEffect;
    public Renderer mineRenderer;
    private Color originalColor;
    public AudioSource explosion;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Renderer mineRenderer = GetComponent<Renderer>();
        originalPosition = transform.position;
        originalColor = mineRenderer.material.color;
        if (player != null)
        {
            playerHealth = player.GetComponent<playerController>();
        }
    }

    void Update()
    {
        if (player == null || gameObject == null || isExploded || hp <= 0) return;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= explosionRange)
        {
            Explode();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            ReturnToPositionAndFloat();
        }
    }
    void ReturnToPositionAndFloat()
    {
        Vector3 targetPosition = new Vector3(originalPosition.x, transform.position.y, originalPosition.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        Float();
    }

    void MoveTowardsPlayer()
    {
        Vector3 step = moveSpeed * Time.deltaTime * (player.position - transform.position).normalized;
        transform.position += step;
    }

    void Float()
    {
        float newY = Mathf.Lerp(transform.position.y, originalPosition.y + Mathf.Sin(Time.time * Mathf.PI * floatFrequency) * floatAmplitude, Time.deltaTime * moveSpeed);
        Vector3 newPosition = new Vector3(transform.position.x, newY, transform.position.z);
        transform.position = newPosition;
    }

    public void takeDamage(int amount)
    {
        hp -= amount;
        StartCoroutine(FlashRed());

        if (hp <= 0)
        {
            die();
        }
    }

    IEnumerator FlashRed()
    {
        if (mineRenderer != null)
        {
            mineRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            mineRenderer.material.color = originalColor;
        }
    }
    public void die()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
        ParticleSystem effectInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        effectInstance.Play();
        explosion.Play();
        Destroy(effectInstance.gameObject, effectInstance.main.duration);
        StartCoroutine(DelayedDestroy(4f));

    }

    void Explode()
    {
        if (!isExploded)
        {
            Debug.Log("Boom! Damage: " + damage);
            if (playerHealth != null)
            {
                playerHealth.takeDamage(damage);
            }

            if (explosionEffect != null)
            {
                ParticleSystem effectInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                effectInstance.Play();
                Destroy(effectInstance.gameObject, effectInstance.main.duration);
            }
            explosion.Play();
            isExploded = true;
            SphereCollider collider = GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
            StartCoroutine(DelayedDestroy(4f));
        }
    }
    IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}