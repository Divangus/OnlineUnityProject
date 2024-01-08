using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Set this to the speed at which you want the rotation to occur
    public float rotationSpeed = 720f;
    private float startTime;

    private bool isRotating = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rot"))
        {
            // Start rotating when the GameObject enters the trigger
            isRotating = true;
            startTime = Time.time; // Record the start time
        }
    }

    private void Update()
    {
        // Rotate the GameObject gradually while isRotating is true
        if (isRotating)
        {
            // Calculate the rotation amount based on the rotationSpeed and time
            float rotationAmount = rotationSpeed * Time.deltaTime;

            // Rotate the GameObject around its up (Y) axis
            transform.Rotate(Vector3.up, rotationAmount);

            // Check if 1.5 seconds have passed
            if (Time.time - startTime >= 3f)
            {
                // Stop rotating after 1.5 seconds
                isRotating = false;
            }
        }
    }
}