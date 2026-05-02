using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    [Header("Game Settings")]
    [SerializeField] float gameDuration = 60f;
    [SerializeField] int maxSavedGames = 5;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI historyText;

    [Header("Reset Input")]
    [SerializeField] XRNode resetController = XRNode.RightHand;

    [Header("Target Spawn Area")]
    [SerializeField] Transform spawnCenter;
    [SerializeField] float minSpawnRadius = 2f;
    [SerializeField] float spawnRadius = 5f;
    [SerializeField] float minHeight = 1f;
    [SerializeField] float maxHeight = 3f;

    [Header("Targets")]
    [SerializeField] GameObject[] targetPrefabs;
    [SerializeField] int activeTargetCount = 3;

    int score;
    int gameNumber = 1;

    float timeRemaining;
    bool gameActive;
    bool bButtonWasPressed;
    bool scoreSavedForThisGame;

    List<string> gameHistory = new List<string>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartNewGame();
    }

    private void Update()
    {
        CheckResetButton();

        if (!gameActive)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndGame();
        }

        UpdateUI();
    }

    public void AddPoint(int amount)
    {
        if (!gameActive || timeRemaining <= 0f)
            return;

        score += amount;
        UpdateUI();
    }

    public void ResetGame()
    {
        EndGame();
        StartNewGame();
    }

    private void StartNewGame()
    {
        score = 0;
        timeRemaining = gameDuration;
        gameActive = true;
        scoreSavedForThisGame = false;

        ClearExistingTargets();
        SpawnStartingTargets();

        UpdateUI();
    }

    private void EndGame()
    {
        if (scoreSavedForThisGame)
            return;

        gameActive = false;
        scoreSavedForThisGame = true;

        gameHistory.Add("Game " + gameNumber + " Score: " + score);
        gameNumber++;

        while (gameHistory.Count > maxSavedGames)
            gameHistory.RemoveAt(0);

        ClearExistingTargets();
        UpdateUI();
    }

    private void SpawnStartingTargets()
    {
        for (int i = 0; i < activeTargetCount; i++)
            SpawnTarget();
    }

    public void SpawnTarget()
    {
        if (!gameActive)
            return;

        if (targetPrefabs == null || targetPrefabs.Length == 0)
            return;

        if (spawnCenter == null)
        {
            Debug.LogWarning("Spawn Center is missing.");
            return;
        }

        if (minSpawnRadius > spawnRadius)
        {
            Debug.LogWarning("Min Spawn Radius is greater than Spawn Radius. Clamping min radius.");
            minSpawnRadius = spawnRadius;
        }

        float randomRadius = Random.Range(minSpawnRadius, spawnRadius);
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);

        Vector2 randomCircle = new Vector2(
            Mathf.Cos(randomAngle),
            Mathf.Sin(randomAngle)
        ) * randomRadius;

        Vector3 spawnPosition = spawnCenter.position + new Vector3(
            randomCircle.x,
            Random.Range(minHeight, maxHeight),
            randomCircle.y
        );

        int randomIndex = Random.Range(0, targetPrefabs.Length);

        GameObject target = Instantiate(
            targetPrefabs[randomIndex],
            spawnPosition,
            Quaternion.identity
        );

        TargetHit targetHit = target.GetComponent<TargetHit>();

        if (targetHit == null)
            targetHit = target.AddComponent<TargetHit>();

        targetHit.SetMiniGameManager(this);
    }

    private void ClearExistingTargets()
    {
        TargetHit[] targets = FindObjectsByType<TargetHit>(FindObjectsSortMode.None);

        foreach (TargetHit target in targets)
            Destroy(target.gameObject);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);

        if (historyText != null)
            historyText.text = string.Join("\n", gameHistory);
    }

    private void CheckResetButton()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(resetController);

        if (!device.isValid)
            return;

        device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bPressed);

        if (bPressed && !bButtonWasPressed)
            ResetGame();

        bButtonWasPressed = bPressed;
    }
}