using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public AnimationCurve transitionCurve; // Curve to control the transition speed
    public float transitionDuration = 1.0f; // Duration of the transition
    public float distanceFromCenter = 5.0f; // Radius distance from the table center
    public Vector3 tableCenter; // Center point of the table
    public float angleIncrement = 45f; // Angle to rotate each time (e.g., 45 degrees)

    private bool isTransitioning = false;
    private float currentAngle = 0f;

    void Start()
    {
        // Initialize the camera to the default angle
        UpdateCameraPosition();
    }

    // Public method to rotate left, can be linked to a UI button
    public void RotateLeft()
    {
        if (!isTransitioning)
        {
            RotateCamera(-angleIncrement);
        }
    }

    // Public method to rotate right, can be linked to a UI button
    public void RotateRight()
    {
        if (!isTransitioning)
        {
            RotateCamera(angleIncrement);
        }
    }

    private void RotateCamera(float angleChange)
    {
        StartCoroutine(RotateAroundTable(angleChange));
    }

    private IEnumerator RotateAroundTable(float angleChange)
    {
        isTransitioning = true;

        float startAngle = currentAngle;
        float targetAngle = startAngle + angleChange;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            t = transitionCurve.Evaluate(t);

            // Interpolate the angle based on transition progress
            currentAngle = Mathf.Lerp(startAngle, targetAngle, t);
            UpdateCameraPosition();

            yield return null;
        }

        currentAngle = targetAngle;
        isTransitioning = false;
    }

    private void UpdateCameraPosition()
    {
        // Calculate the new position based on the current angle
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 newPosition = new Vector3(
            tableCenter.x + distanceFromCenter * Mathf.Cos(radians),
            transform.position.y, // Maintain the current height
            tableCenter.z + distanceFromCenter * Mathf.Sin(radians)
        );

        transform.position = newPosition;
        transform.LookAt(tableCenter); // Always look at the center of the table
    }
}
