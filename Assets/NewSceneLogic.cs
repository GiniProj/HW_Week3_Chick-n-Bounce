using UnityEngine;
using UnityEngine.UI;

public class NewSceneLogic : MonoBehaviour
{
    public Text topScoreText;

    void Start()
    {
        int topScore = GameData.TopScore;
        topScoreText.text = topScore.ToString();
    }
}
