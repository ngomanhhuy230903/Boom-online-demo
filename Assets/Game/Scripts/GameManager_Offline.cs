using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager_Offline : MonoBehaviourPunCallbacks
{
    public static GameManager_Offline Instance;

    [Header("Game State")]
    public bool isGameActive = false;
    public bool isGamePaused = false;
    public float gameTime = 0f;
    public int maxGameTime = 300; // 5 phút

    [Header("Player Health & Win/Lose")]
    public int playerMaxHealth = 3;
    public int currentPlayerHealth = 3;
    public bool isPlayerDead = false;
    private float lastDamageTime = -10f; // Thời gian cuối cùng bị trừ máu
    public float damageCooldown = 1f; // Khoảng thời gian chờ giữa các lần trừ máu (1 giây)

    [Header("Player Management")]
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    public GameObject currentPlayer;

    [Header("Bomb Management")]
    private PlayerBombSpawner playerBombSpawner;

    [Header("UI References")]
    public GameObject gameUI;
    public GameObject pauseUI;
    public GameObject gameOverUI;
    public GameObject winUI;

    [Header("Game Settings")]
    public bool enableNetworkMode = false; // Tắt network cho offline mode

    [Header("Camera Setup")]
    public TopDownFollowCamera topDownCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeGame();
    }

void Update()
{
    if (isGameActive && !isGamePaused)
    {
        UpdateGameTime();   // chỉ gọi 1 lần
        HandleInput();

        // Kiểm tra điều kiện thắng
        CheckWinCondition();
    }
}

    private void InitializeGame()
    {
        Debug.Log("[GameManager] Khởi tạo game offline...");

        // Tắt network mode nếu cần
        if (!enableNetworkMode)
        {
            PhotonNetwork.OfflineMode = true;
        }

        // Spawn player
        SpawnPlayer();

        // Bắt đầu game
        StartGame();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[GameManager] Player prefab chưa được gán!");
            return;
        }

        Transform spawnPoint = GetRandomSpawnPoint();
        if (spawnPoint != null)
        {
            currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("[GameManager] Đã spawn player tại: " + spawnPoint.position);

            // Setup camera sau khi spawn player
            SetupCamera();

            // Setup PlayerBombSpawner
            SetupPlayerBombSpawner();
        }
        else
        {
            Debug.LogError("[GameManager] Không tìm thấy spawn point!");
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            GameObject defaultSpawn = new GameObject("DefaultSpawn");
            defaultSpawn.transform.position = Vector3.zero;
            return defaultSpawn.transform;
        }

        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    private void StartGame()
    {
        isGameActive = true;
        gameTime = maxGameTime; // Reset thời gian khi bắt đầu
        currentPlayerHealth = playerMaxHealth;
        isPlayerDead = false;
        lastDamageTime = -10f; // Reset thời gian cuối cùng bị trừ máu
        Debug.Log("[GameManager] Game đã bắt đầu! Máu: " + currentPlayerHealth);

        if (gameUI != null)
            gameUI.SetActive(true);
    }

private void UpdateGameTime()
{
    gameTime -= Time.deltaTime;

    if (gameTime <= 0)
    {
        gameTime = 0;
        GameOver("Hết thời gian!");
    }
}

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f;

        if (pauseUI != null)
            pauseUI.SetActive(isGamePaused);

        Debug.Log("[GameManager] Game " + (isGamePaused ? "đã pause" : "đã resume"));
    }

    public void GameOver(string reason = "Game Over")
    {
        isGameActive = false;
        Debug.Log("[GameManager] Game Over: " + reason);

        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }

    public void Win()
    {
        isGameActive = false;
        Debug.Log("[GameManager] Chiến thắng!");

        if (winUI != null)
            winUI.SetActive(true);
    }

    public void RestartGame()
    {
        Debug.Log("[GameManager] Restart game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("[GameManager] Thoát game...");
        Application.Quit();
    }

    public void OnPauseButtonClicked() => TogglePause();
    public void OnResumeButtonClicked() => TogglePause();
    public void OnRestartButtonClicked() => RestartGame();
    public void OnQuitButtonClicked() => QuitGame();

    private void SetupCamera()
    {
        if (topDownCamera == null)
        {
            topDownCamera = Camera.main.GetComponent<TopDownFollowCamera>();
            Debug.Log("[GameManager] Tìm thấy TopDownFollowCamera: " + (topDownCamera != null));
        }

        if (topDownCamera != null && currentPlayer != null)
        {
            topDownCamera.target = currentPlayer.transform;
            Debug.Log("[GameManager] TopDownFollowCamera đã được setup để follow player: " + currentPlayer.name);
        }
        else
        {
            Debug.LogWarning($"[GameManager] Không thể setup camera - topDownCamera: {topDownCamera != null}, currentPlayer: {currentPlayer != null}");
        }
    }

    private void SetupPlayerBombSpawner()
    {
        if (currentPlayer != null)
        {
            playerBombSpawner = currentPlayer.GetComponent<PlayerBombSpawner>();

            if (playerBombSpawner != null)
            {
                playerBombSpawner.enabled = true;
                Debug.Log("[GameManager] PlayerBombSpawner đã được setup và sẵn sàng");
            }
            else
            {
                Debug.LogWarning("[GameManager] Player không có PlayerBombSpawner component");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isPlayerDead || !isGameActive) return;

        // Kiểm tra thời gian cooldown để tránh trừ máu nhiều lần ngay lập tức
        if (Time.time - lastDamageTime >= damageCooldown)
        {
            currentPlayerHealth -= damage;
            lastDamageTime = Time.time; // Cập nhật thời gian cuối cùng bị trừ máu
            Debug.Log("[GameManager] Player bị trúng bom! Máu còn: " + currentPlayerHealth);

            if (currentPlayerHealth <= 0)
            {
                currentPlayerHealth = 0;
                isPlayerDead = true;
                Debug.Log("[GameManager] Máu về 0! Dừng game.");
                GameOver("Bạn đã thua (hết máu)");
                isGameActive = false; // Dừng game khi máu về 0
                Time.timeScale = 0f; // Tạm dừng thời gian
            }
        }
    }

    private void CheckWinCondition()
    {
        if (!isPlayerDead)
        {
            // Có thể thêm điều kiện: nếu clear hết enemy thì Win();
        }
    }
}