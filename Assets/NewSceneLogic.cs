using UnityEngine;
using UnityEngine.UI;

public class NewSceneLogic : MonoBehaviour
{
    [SerializeField] private Text topScoreText;

    void Start()
    {
        int topScore = GameData.TopScore;
        topScoreText.text = topScore.ToString();
    }
}
