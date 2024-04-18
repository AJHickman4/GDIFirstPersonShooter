using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    public float amplitude = 0.5f; 
    public float frequency = 1f; 

    private Vector3 startPos;
    public ParticleSystem floatEffect;
    private Quaternion fixedRotation;
    private BoxCollider myCollider;

    void Start()
    {
        startPos = transform.position;
        fixedRotation = transform.rotation;
        floatEffect.Play();
        myCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        Vector3 tempPos = startPos;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.rotation = fixedRotation;
        transform.position = tempPos;
        if (myCollider != null)
        {
            myCollider.transform.position = tempPos;
        }
    }
    public void StopFloating()
    {
        this.enabled = false; 
        floatEffect.Stop();
    }
}
