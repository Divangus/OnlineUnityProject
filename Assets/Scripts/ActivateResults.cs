using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateResults : MonoBehaviour
{
    bool end = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Active"))
        {
            end = true;
            Debug.Log(end);
        }
    }

    private void Update()
    {
        // Rotate the GameObject gradually while isRotating is true
        if (end)
        {
            GameObject endObject = GameObject.FindGameObjectWithTag("End");

            // Check if the GameObject is found before trying to set it active
            if (endObject != null)
            {
                endObject.SetActive(true);
            }
            else
            {
                Debug.LogError("GameObject with tag 'End' not found.");
            }

            // Optionally, set end to false to ensure this block is executed only once
            end = false;
        }
    }
}
