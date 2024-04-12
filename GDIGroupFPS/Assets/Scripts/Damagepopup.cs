using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Damagepopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private static float heightOffset = 1f;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }
    private void Update()
    {
        float moveYSpeed = .5f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        // Make the popup always face the main camera
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0); // Adjust for correct orientation

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 1f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Setup(int damageAmount)
    {
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;
        disappearTimer = 1f;
    }

    public static Damagepopup Create(Transform prefab, Vector3 position, int damageAmount)
    {
        Debug.Log("Creating damage popup at position: " + position);
        Vector3 adjustedPosition = position + Vector3.up * heightOffset;
        Transform damagePopupTransform = Instantiate(prefab, adjustedPosition, Quaternion.Euler(0, 180, 0));
        Damagepopup damagePopup = damagePopupTransform.GetComponent<Damagepopup>();
        damagePopup.Setup(damageAmount);

        return damagePopup;
    }
}
