using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;

    private int score = 0;

    void Awake()
    {
        // Persistent singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Subscribe to scene change
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
    }

    // Called every time a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try to find the TMP_Text in the new scene
        if (scoreText == null)
        {
            scoreText = FindObjectOfType<TMP_Text>();
        }

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void ResetScore()
    {
        score = 0;
        UpdateUI();
    }

    void OnDestroy()
    {
        // Unsubscribe from scene change
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
