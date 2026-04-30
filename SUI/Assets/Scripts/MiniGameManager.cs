using UnityEngine;
using UnityEngine.XR;
using TMPro;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    [Header("Game Settings")]
    [SerializeField] float gameDuration = 60f;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Reset Input")]
    [SerializeField] XRNode resetController = XRNode.RightHand;

    [Header("Target Spawn Area")]
    [SerializeField] Transform spawnCenter;
    [SerializeField] float spawnRadius = 5f;
    [SerializeField] float minHeight = 1f;
    [SerializeField] float maxHeight = 3f;

    [Header("Targets")]
    [SerializeField] GameObject[] targetPrefabs;
    [SerializeField] int activeTargetCount = 3;

    int score;
    float timeRemaining;
    bool gameActive;
    bool bButtonWasPressed;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetGame();
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
            gameActive = false;
        }

        UpdateUI();
    }

    public void AddPoint(int amount)
    {
        if (!gameActive)
            return;

        score += amount;
        UpdateUI();
    }

    public void ResetGame()
    {
        score = 0;
        timeRemaining = gameDuration;
        gameActive = true;

        ClearExistingTargets();
        SpawnStartingTargets();

        UpdateUI();
    }

    private void SpawnStartingTargets()
    {
        for (int i = 0; i < activeTargetCount; i++)
        {
            SpawnTarget();
        }
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

        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;

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
        {
            Destroy(target.gameObject);
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);
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