using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateResults : MonoBehaviour
{
    
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Active"))
        {
            GameObject.FindGameObjectWithTag("End").SetActive(true);
        }
    }    
}
