using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraDirectionHitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<PlayerMovement>().ChangeBoardDirection(1);
        }
    }
}
