using UnityEngine;

public class PartsMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Maximum movement speed")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("X position where object will be destroyed")]
    [SerializeField] private float deadZone = -65f;

    private Vector2 lastPosition;
    private const float POSITION_CHANGE_THRESHOLD = 10f;

    void OnEnable()
    {
        lastPosition = transform.position;
        Debug.Log($"[PartsMovement] Initialized {gameObject.name} at position: {transform.position}");
    }

    void FixedUpdate()
    {
        moveObject();
        checkBoundaries();
        monitorMovement();
    }

    private void moveObject()
    {
        transform.position += Vector3.left * moveSpeed * Time.fixedDeltaTime;  // Move the object to the left at a constant speed
    }

    private void checkBoundaries()
    {
        Vector2 currentPosition = transform.position;

        // Check for deadzone
        if (currentPosition.x < deadZone)
        {
            Debug.Log($"[PartsMovement] {gameObject.name} reached deadzone at {currentPosition}. Destroying.");
            destroyObject();
            return;
        }
    }

    private void monitorMovement()
    {
        Vector2 currentPosition = transform.position;
        float distance = Vector2.Distance(currentPosition, lastPosition);

        // Check for unusual movement
        if (distance > POSITION_CHANGE_THRESHOLD)
        {
            Debug.LogWarning($"[PartsMovement] Unusual movement detected on {gameObject.name}! " +
                           $"Delta: {distance}, Previous: {lastPosition}, Current: {currentPosition}");
        }

        lastPosition = currentPosition;
    }

    private void destroyObject()
    {
        enabled = false; // Disable this component
        Destroy(gameObject, 10f);
    }
}