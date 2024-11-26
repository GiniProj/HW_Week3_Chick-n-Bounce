using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Logic : MonoBehaviour
{
    [Header("Logic Basic Settings")]
    [Tooltip("Player Score - Default 0")]
    public int score = 0;
    public Text scoreText;
    [Tooltip("Top Score - Default 0")]
    public int topScore = 0;
    public Text topScoreText;

    public GameObject gameOverScreen;


    [ContextMenu("Add Score")]
    public void addScore()
    {
        score++;
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
            Debug.Log("Score updated to: " + score); // Log updated score
        }
        else
        {
            Debug.LogError("Cannot update scoreText. It is not assigned.");
        }
    }

    [ContextMenu("Top Score")]
    public void updateTopScore()
    {
        if (score > topScore)
        {
            topScore = score;
            if (topScoreText != null)
            {
                topScoreText.text = topScore.ToString();
                Debug.Log("Top Score updated to: " + topScore);
            }
            else
            {
                Debug.LogError("Cannot update topScoreText. It is not assigned.");
            }
        }
    }

    public void gameOver()
    {
        gameOverScreen.SetActive(true);
        updateTopScore();
    }

    public void restartGame()
    {
        GameData.TopScore = topScore > GameData.TopScore ? topScore : GameData.TopScore;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    void Start()
    {
        // Ensure scoreText is assigned in the Inspector
        if (scoreText == null)
        {
            Debug.LogError("ScoreText is not assigned in the Inspector");
        }

        if (topScoreText == null)
        {
            Debug.LogError("TopScoreText is not assigned.");
        }
    }



}
