using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] Text livesText;
    [SerializeField] Text scoreText;
    [SerializeField] Text blocksText;
    [SerializeField] Text highScoreText;

    int currentScore = 0;
    int currentLives = 0;
    int currentHighScore = 0;
    private void Awake()
    {
        int numGameSessions = FindObjectsOfType<UIManager>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        Block.OnBlockDestruction += OnBlockDestruction;
        BlocksManager.OnLevelLoaded += OnLevelLoaded;
        GameManager.OnLifeLost += OnLifeLost;
    }

    private void Start()
    {
        OnLifeLost(GameManager.Instance.AvailableLives);
    }

    private void OnBlockDestruction(Block block)
    {
        UpdateRemainingBlocksText();
        UpdateScoreText(block.PointsPerBlock);
    }

    private void UpdateRemainingBlocksText()
    {
        blocksText.text = $"TARGET:{Environment.NewLine}{BlocksManager.Instance.RemainingBlocks.Count}/{BlocksManager.Instance.InitialBlocksCount}";
    }
    private void OnLevelLoaded()
    {
        UpdateRemainingBlocksText();
        UpdateScoreText(0);
    }

    private void UpdateScoreText(int points)
    {
        currentScore += points;
        string scoreString = currentScore.ToString().PadLeft(5, '0');
        scoreText.text = $"SCORE:{Environment.NewLine}{scoreString}";
        if (currentScore > currentHighScore)
        {
            currentHighScore = currentScore;
            string highScoreString = currentHighScore.ToString().PadLeft(5, '0');
            highScoreText.text = $"HIGH SCORE:{Environment.NewLine}{highScoreString}";
        }
    }

    private void OnDisable()
    {
        Block.OnBlockDestruction -= OnBlockDestruction;
        BlocksManager.OnLevelLoaded -= OnLevelLoaded;
        GameManager.OnLifeLost -= OnLifeLost;
    }

    private void OnLifeLost(int lives)
    {
        currentLives = lives;
        livesText.text = $"LIVES: {currentLives}";
    }
}
