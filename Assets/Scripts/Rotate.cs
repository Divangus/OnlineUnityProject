using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Set this to the speed at which you want the rotation to occur
    public float rotationSpeed = 180f;

    private bool isRotating = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rot"))
        {
            // Start rotating when the car enters the trigger
            isRotating = true;
        }
    }

    private void Update()
    {
        // Rotate the car gradually while isRotating is true
        if (isRotating)
        {
            // Calculate the rotation amount based on the rotationSpeed and time
            float rotationAmount = rotationSpeed * Time.deltaTime;

            // Rotate the car around its up (y) axis
            transform.Rotate(Vector3.up, rotationAmount);

            // Check if we have rotated 360 degrees
            if (Mathf.Abs(transform.rotation.eulerAngles.y) >= 360f)
            {
                // Stop rotating when we reach 360 degrees
                isRotating = false;
            }
        }
    }
}

