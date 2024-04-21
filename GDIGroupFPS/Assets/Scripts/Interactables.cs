using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    public Animator targetAnimator;

    public void TriggerFloorRise()
    {
        if (targetAnimator != null)
        {
            targetAnimator.SetTrigger("FloorRise");
        }
    }
}
