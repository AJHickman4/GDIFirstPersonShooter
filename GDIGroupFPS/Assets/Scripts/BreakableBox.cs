using System.Collections;

using UnityEngine;

public class BreakableBox : MonoBehaviour, IDamage
{
    [SerializeField] Renderer boxRenderer;
    [SerializeField] private int health = 10;

    public void takeDamage(int damage)
    {
        if (health <= 0) 
        return;

        health -= damage;
        StartCoroutine(FlashRed());

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashRed()
    {
        boxRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        boxRenderer.material.color = Color.white;
    }
}
