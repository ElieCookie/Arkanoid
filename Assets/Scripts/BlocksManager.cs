using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BlocksManager : MonoBehaviour
{
    public static BlocksManager Instance { get; private set; }

    public List<int[,]> LevelsData { get; set; }
    public Sprite[] BlockSprites;
    public Color[] BlockColors;
    public List<Block> RemainingBlocks { get; set; }

    [SerializeField] public int CurrentLevel;
    [SerializeField] public Block BlockPrefab;
    public int InitialBlocksCount { get; set; }
    private int maxRows = 17;
    private int maxCols = 12;
    private GameObject blocksContainer;
    private float initialBlockSpawnPositionX = -1.96f;
    private float initialBlockSpawnPositionY = 3.325f;
    private float shiftAmount = 0.365f;

    public static event Action OnLevelLoaded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        blocksContainer = new GameObject("BlocksContainer");
        LevelsData = LoadLevelsData();
        GenerateBlocks();
    }

    private void GenerateBlocks()
    {
        RemainingBlocks = new List<Block>();
        int[,] currentLevelData = LevelsData[CurrentLevel];
        float currentSpawnX = initialBlockSpawnPositionX;
        float currentSpawnY = initialBlockSpawnPositionY;
        float zShift = 0.2f;

        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                int blockType = currentLevelData[row, col];
                if (blockType > 0)
                {
                    Block newBlock = Instantiate(BlockPrefab, new Vector3(currentSpawnX, currentSpawnY, 0 - zShift), Quaternion.identity) as Block;
                    newBlock.Init(blocksContainer.transform, BlockSprites[blockType - 1], BlockColors[blockType], blockType);

                    RemainingBlocks.Add(newBlock);
                    zShift += 0.0001f;
                }
                currentSpawnX += shiftAmount;
                if (col + 1 == maxCols)
                {
                    currentSpawnX = initialBlockSpawnPositionX;
                }
            }
            currentSpawnY -= shiftAmount;
        }
        InitialBlocksCount = RemainingBlocks.Count;
        OnLevelLoaded?.Invoke();
    }

    private List<int[,]> LoadLevelsData()
    {
        TextAsset levelsFile = Resources.Load("Levels") as TextAsset;
        string[] rows = levelsFile.text.Split(new string[] { Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
        List<int[,]> levelsData = new List<int[,]>();
        int[,] currentLevel = new int[maxRows, maxCols];
        int currentRow = 0;

        for (int row = 0; row < rows.Length; row++)
        {
            string line = rows[row];
            if (line.IndexOf("--") == -1)
            {
                string[] blocks = line.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
                for (int col = 0; col < blocks.Length; col++)
                {
                    currentLevel[currentRow, col] = int.Parse(blocks[col]);
                }
                currentRow++;
            }
            else
            {
                currentRow = 0;
                levelsData.Add(currentLevel);
                currentLevel = new int[maxRows, maxCols];
            }

        }
        return levelsData;
    }

    public void LoadLevel(int level)
    {
        CurrentLevel = level;
        ClearRemainingBlocks();
        GenerateBlocks();
    }

    private void ClearRemainingBlocks()
    {
        foreach (var block in RemainingBlocks)
        {
            Destroy(block.gameObject);
        }
    }

    public void LoadNextLevel()
    {
        CurrentLevel++;
        if (CurrentLevel >= LevelsData.Count)
        {
            GameManager.Instance.ShowVictoryScreen();
        }
        else
        {
            LoadLevel(CurrentLevel);    
        }
    }
}
