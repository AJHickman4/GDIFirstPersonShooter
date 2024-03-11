using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class waypoints : MonoBehaviour
{
    [SerializeField] GameObject[] waypointArray;
    [SerializeField] int currWaypoint = 0;

    [SerializeField] float speed = 1.0f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, waypointArray[currWaypoint].transform.position) < .1f)
        {
            currWaypoint++;
            if (currWaypoint >= waypointArray.Length)
                currWaypoint = 0;
        }

        transform.position = Vector3.MoveTowards(transform.position, waypointArray[currWaypoint].transform.position, speed * Time.deltaTime);
    }

}
