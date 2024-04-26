using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public float speed = 100f;
    public float rotationSpeed = 100f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Fly()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.velocity = transform.forward * moveVertical * speed;

        transform.Rotate(Vector3.up, moveHorizontal * rotationSpeed * Time.deltaTime);
    }
}
