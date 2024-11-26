using UnityEditor.EditorTools;
using UnityEngine;

public class CloudProperty : MonoBehaviour
{
    [Header("Cloud Properties")]
    [Tooltip("Approve to change the scale of the cloud - Override Transform Scale")]
    public bool changeScale = true;
    [Tooltip("Scale of the cloud")]    
    public float mainScale = 1.5f;
    [Tooltip("Approve to change the scale of the cloud randomly - Override Transform Scale")]
    public bool randomMainScale = true;
    [Tooltip("Range to Random scale from main scale cloud")]
    public float mainScaleRangeRandomness = 0.5f;

    // Add minimum scale threshold
    private const float MIN_SCALE_THRESHOLD = 0.1f;

    void Start()
    {
        if (changeScale)
        {
            float targetScale = mainScale;
            
            if (randomMainScale)
            {
                targetScale = Random.Range(mainScale - mainScaleRangeRandomness, mainScale + mainScaleRangeRandomness);
                
                // Ensure scale doesn't go below minimum threshold
                if (targetScale < MIN_SCALE_THRESHOLD)
                {
                    Debug.LogWarning($"[CloudProperty] Random scale ({targetScale}) below minimum threshold. Adjusting to {MIN_SCALE_THRESHOLD}");
                    targetScale = MIN_SCALE_THRESHOLD;
                }
                
                Debug.Log($"[CloudProperty] Applying random scale: {targetScale}");
            }
            
            transform.localScale = new Vector3(targetScale, targetScale, targetScale);
        }        
    }
}
