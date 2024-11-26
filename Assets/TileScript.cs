using UnityEngine;

public class TileScript : MonoBehaviour
{
    [Tooltip("Range to set active the tile")]
    [SerializeField] private float rangeSetActive = 45f;
    [Tooltip("Time until the tile reappear in the screen")]
    [SerializeField] private float time = 2f;

    private bool exitCollision = false;
    private bool isRunning = true;  // To control the async loop

    private void Start()
    {
        ChangeActivationLoop();
    }

    private void OnDestroy()
    {
        isRunning = false;  // Stop the async loop when object is destroyed
    }

    async void ChangeActivationLoop()
    {
        while (isRunning)
        {
            await Awaitable.WaitForSecondsAsync(time);
            if(!isRunning || !gameObject) return;

            if (exitCollision)
            {
                float randomX = Random.Range(-rangeSetActive, rangeSetActive);
                transform.position = new Vector3(randomX, transform.position.y, transform.position.z);
                exitCollision = false;
                gameObject.SetActive(true);
            }
            else if (!gameObject.activeSelf)  // Simplified condition
            {
                gameObject.SetActive(true);
            }
        }
    }

    private void OnCollisionExit2D()
    {
        gameObject.SetActive(false);
        exitCollision = true;
    }
}