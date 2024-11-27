using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ScalarAndPositionManager : MonoBehaviour
{
    [Header("Resolution Manager")]
    [SerializeField] private Camera mainCamera;
    [Tooltip("Add all the objects that you want to scale according to the resolution")]
    [SerializeField] private List<GameObject> objects = new List<GameObject>();

    // Base resolution constants
    private const float BASE_RESOLUTION_WIDTH = 800;
    private const float BASE_RESOLUTION_HEIGHT = 600;

    // Update frequency for resolution check
    private const float RESOLUTION_CHECK_INTERVAL = 0.5f;

    // Unity units conversion rate (pixels to unity units)
    private const float PIXELS_PER_UNIT = 100f;

    // Camera constants
    private const float VERTICAL_FOV_FACTOR = 0.5f; // Represents half of the vertical field of view
    private Vector2 baseResolution;
    private float baseOrthographicSize;
    private int lastScreenWidth;
    private int lastScreenHeight;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        baseOrthographicSize = mainCamera.orthographicSize;  // Store the base orthographic size of the camera
        baseResolution = new Vector2(BASE_RESOLUTION_WIDTH, BASE_RESOLUTION_HEIGHT);
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        StoreTransformsInList();  // Store initial transforms of the objects

        // Perform initial adjustments to the camera and objects - if the resolution is not the base resolution
        if (BASE_RESOLUTION_WIDTH != Screen.currentResolution.width || BASE_RESOLUTION_HEIGHT != Screen.currentResolution.height)
        {
            AdjustAllObjects();
        }
    }

    private void OnEnable()
    {
        StartCoroutine(CheckForResolutionChange());
        if (HasResolutionChanged())
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            SetOrthographicSize();
            AdjustAllObjects();
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void StoreTransformsInList()
    {
        foreach (GameObject obj in objects)
        {
            if (!obj.TryGetComponent<InitialTransform>(out var initialTransform))
            {
                initialTransform = obj.AddComponent<InitialTransform>();
                initialTransform.StoreInitialTransform();
            }
        }
    }

    private IEnumerator CheckForResolutionChange()
    {
        WaitForSeconds waitInterval = new WaitForSeconds(RESOLUTION_CHECK_INTERVAL);

        while (true)
        {
            yield return waitInterval;
        }
    }

    private bool HasResolutionChanged()
    {
        return Screen.width != lastScreenWidth || Screen.height != lastScreenHeight;
    }

    private void SetOrthographicSize()
    {
        float orthographicFactor = 0.5f;
        float currentAspectRatio = (float)Screen.width / Screen.height;
        float targetAspectRatio = baseResolution.x / baseResolution.y;
        if (currentAspectRatio < targetAspectRatio)
        {
            mainCamera.orthographicSize = orthographicFactor * baseOrthographicSize * (targetAspectRatio / currentAspectRatio);
        }
    }


    private void AdjustAllObjects()
    {
        Vector2 scaleFactor =  (new Vector2(Screen.width, Screen.height)) / baseResolution;
        foreach (GameObject obj in objects)
        {
            if (obj != null) continue;

            InitialTransform initialTransform = obj.GetComponent<InitialTransform>();
            if (initialTransform == null) continue;

            AdjustObjectTransform(obj, initialTransform, scaleFactor);
        }
    }

    private void AdjustObjectTransform(GameObject obj, InitialTransform initialTransform, Vector2 scaleFactor)
    {
        Vector3 initialScale = initialTransform.InitialScale;
        Vector3 initialPosition = initialTransform.InitialPosition;

        // Calculate new scale and position
        Vector3 newScale = new Vector3(
            initialScale.x * scaleFactor.x,
            initialScale.y * scaleFactor.y,
            initialScale.z
        );

        // Calculate new position
        Vector3 newPosition = new Vector3(
            initialPosition.x * scaleFactor.x,
            initialPosition.y * scaleFactor.y,
            initialPosition.z
        );

        obj.transform.localScale = newScale;  // Apply new scale
        obj.transform.position = newPosition;  // Apply new position
    }
}

// Helper component to store initial transform values
public class InitialTransform : MonoBehaviour
{
    public Vector3 InitialScale { get; private set; }
    public Vector3 InitialPosition { get; private set; }

    public void StoreInitialTransform()
    {
        InitialScale = transform.localScale;
        InitialPosition = transform.position;
    }
}