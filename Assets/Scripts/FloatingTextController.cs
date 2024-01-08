using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextController : MonoBehaviour
{
    public Text floatingText;
    public string textToDisplay = "";
    public float floatSpeed = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Active"))
        {
            textToDisplay = "Win";
            Text textInstance = Instantiate(floatingText, transform.position, Quaternion.identity);
            textInstance.transform.SetParent(GameObject.Find("CanvasHolder").transform); // Set the canvas as the parent
            textInstance.text = textToDisplay;
            Destroy(textInstance.gameObject, 2.0f); // Destroy the floating text after 2 seconds

            // Add a floating animation (optional)
            StartCoroutine(FloatText(textInstance));
        }
    }

    IEnumerator FloatText(Text textInstance)
    {
        Vector3 originalPosition = textInstance.transform.position;
        Vector3 targetPosition = originalPosition + Vector3.up * 2; // Adjust the floating height

        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            textInstance.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * floatSpeed;
            yield return null;
        }
    }
}
