using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Laser : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] LayerMask ignoreMask; //for ignoring objects
    [SerializeField] float laserDistance;
    [Range(0, 100)][SerializeField] int damage;

    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;

    private RaycastHit hit;
    private Ray ray;
    private int dmgDealt = 0;
    float timeSinceLastHit = 0.0f;
    float rateOfFire = 3f;

    // Start is called before the first frame update
    void Awake()
    {
        lineRenderer.positionCount = 2;
        ray.origin = startPos.position;
        lineRenderer.useWorldSpace = true;
    }

    // Update is called once per frame
    void Update()
    {
        ray.direction = startPos.forward;

        Debug.DrawRay(transform.position, transform.forward * laserDistance, Color.red);

        if (Physics.Raycast(ray, out hit, laserDistance, ~ignoreMask))
        {
            timeSinceLastHit += Time.deltaTime;
            endPos.position = hit.point - startPos.position;

            lineRenderer.SetPosition(0, transform.position); //1 pos
            lineRenderer.SetPosition(1, hit.point); //2 pos

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null && !hit.collider.CompareTag("Enemy") && timeSinceLastHit >= rateOfFire)
            {
                timeSinceLastHit -= rateOfFire;
                dmg.takeDamage(damage);
                dmgDealt += damage;
            }
        }
        else //ray doesn't hit anything
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserDistance);
            endPos.position = ray.origin + ray.direction * laserDistance;
        }
    }
    public int TotalDamageDealt
    {
        get { return dmgDealt; }
    }
}
