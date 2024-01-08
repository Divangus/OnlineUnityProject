using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectVictory : MonoBehaviour
{
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("End"))
        {
            
        }
    }
}
