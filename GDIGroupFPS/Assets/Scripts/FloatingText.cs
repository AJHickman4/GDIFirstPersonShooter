using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    Transform mainCam;
    Transform unit;
    Transform WorldSpaceCanvas;
    CanvasRenderer canvasRenderer;

    public Vector3 offset;
    public float cullDistance = 10f;

    void Start()
    {
        mainCam = Camera.main.transform;
        unit = transform.parent;
        WorldSpaceCanvas = GameObject.FindObjectOfType<Canvas>().transform;

        transform.SetParent(WorldSpaceCanvas);

        canvasRenderer = GetComponent<CanvasRenderer>();

    }
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position); //looks at the camera.
        transform.position = unit.position + offset;

        float distance = Vector3.Distance(mainCam.position, unit.position);

        if (distance < cullDistance)
        {
            canvasRenderer.SetAlpha(1);
        }
        else
        {
            canvasRenderer.SetAlpha(0);
        }
    }
}

