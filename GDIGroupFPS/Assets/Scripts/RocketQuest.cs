using UnityEngine;

public class Rocketquest : MonoBehaviour
{
    public int rocketId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Collect();
        }
    }

    private void Collect()
    {
        FindObjectOfType<Sweepo>().CollectMissile(); 
        gameObject.SetActive(false); 
    }
}