using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Transform enemyTransform; 
    private int totalDamage; 

    private Color startColor = Color.red; 
    private Color endColor = Color.yellow; 
    private float colorTransitionTime = 0.5f; 
    public  float initialScale = 1.0f; 
    public  float endScale = 0.5f; 

    private Vector3 originalPosition;
    public float maxRiseHeight = 0.3f; 
    public float maxHorizontalMovement = 0.3f; 
    public float riseAmount = 0.1f;
    public Vector3 localPositionOffset = new Vector3(0, 2.5f, 0);

    private static Dictionary<Transform, DamagePopup> popups = new Dictionary<Transform, DamagePopup>();

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        transform.localScale = new Vector3(initialScale, initialScale, initialScale);
        textMesh.color = startColor;
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        disappearTimer -= Time.deltaTime;

        if (disappearTimer < 0)
        {
            if (enemyTransform != null && popups.ContainsKey(enemyTransform))
            {
                popups.Remove(enemyTransform);
            }
            Destroy(gameObject);
        }
        else
        {
            float t = Mathf.Clamp01(disappearTimer / colorTransitionTime);
            textMesh.color = Color.Lerp(endColor, startColor, t);
            transform.localScale = Vector3.Lerp(new Vector3(endScale, endScale, endScale), new Vector3(initialScale, initialScale, initialScale), t);
            AdjustRotation();
        }
    }

    private void AdjustRotation()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition.y = transform.position.y;
        transform.LookAt(cameraPosition);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);
    }

    public void Setup(int damageAmount, Transform enemy)
    {
        if (popups.TryGetValue(enemy, out DamagePopup existingPopup))
        {
            existingPopup.AddDamage(damageAmount);
        }
        else
        {
            InitDamagePopup(damageAmount, enemy);
            popups[enemy] = this;
        }
    }

    private void InitDamagePopup(int damageAmount, Transform enemy)
    {
        textMesh.SetText(damageAmount.ToString());
        totalDamage = damageAmount;
        disappearTimer = 0.5f;
        enemyTransform = enemy;

        transform.SetParent(enemy);
        transform.localPosition = localPositionOffset;
        originalPosition = transform.localPosition;
    }

    public void AddDamage(int additionalDamage)
    {
        totalDamage += additionalDamage;
        textMesh.SetText(totalDamage.ToString());
        disappearTimer = 0.5f; 
        BounceToNewPosition();
    }

    private void BounceToNewPosition()
    {
        float offsetX = Random.Range(-maxHorizontalMovement, maxHorizontalMovement);
        float offsetY = Mathf.Min(maxRiseHeight, riseAmount);
        Vector3 newPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);
        transform.localPosition = newPosition;
        riseAmount *= 0.9f; 
    }

    public static DamagePopup Create(Transform prefab, Transform enemy, int damageAmount)
    {
        if (popups.TryGetValue(enemy, out DamagePopup existingPopup))
        {
            existingPopup.AddDamage(damageAmount);
            return existingPopup;
        }
        else
        {
            Transform damagePopupTransform = Instantiate(prefab, enemy.position, Quaternion.identity, enemy);
            DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
            damagePopup.Setup(damageAmount, enemy);
            popups[enemy] = damagePopup;
            return damagePopup;
        }
    }
}