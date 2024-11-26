using UnityEngine;

public class MiddleBlock : MonoBehaviour
{
    [Header("Logic Object - Drag and Drop Logic Object")]
    [Tooltip("Game logic responsible for score")]
    [SerializeField] private Logic logicScript;
    private bool gainScore = false;

    void Start()
    {
        GameObject[] logicObjects = GameObject.FindGameObjectsWithTag("Logic");
        if (logicObjects.Length > 0)
        {
            logicScript = logicObjects[0].GetComponent<Logic>();
            if (logicScript == null)
            {
                Debug.LogError("Logic component not found on the Logic GameObject");
            }
        }
        else
        {
            Debug.LogError("Logic GameObject not found");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (logicScript != null)
        {
            if (!gainScore)
            {
                logicScript.addScore();
                gainScore = true;
                Debug.Log("Score added successfully"); // Log score addition
            }
            else
            {
                Debug.Log("Score already added");
            }
        }
        else
        {
            Debug.LogError("LogicScript is null on trigger enter");
        }
    }
}
