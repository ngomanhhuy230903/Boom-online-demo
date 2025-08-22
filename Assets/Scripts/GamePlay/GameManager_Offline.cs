using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;

public class GameManager_Offline : MonoBehaviourPunCallbacks
{
    public static GameManager_Offline Instance;

    [Header("Game State")]
    public bool isGameActive = false;
    public bool isGamePaused = false;
    public float gameTime = 0f;
    public int maxGameTime = 300; // 5 phút

    [Header("Player Management")]
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    public GameObject currentPlayer;
    public Text healthText;
    private Health currentPlayerHealth;

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
            UpdateGameTime();
            HandleInput();
            CheckWinCondition();

            // Update máu của player local
            if (currentPlayerHealth != null)
            {
                UpdateHealthUI(currentPlayerHealth.currentHealth, currentPlayerHealth.maxHealth);
            }
        }
    }

    private void UpdateHealthUI(int current, int max)
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + current + "/" + max;
        }
    }

    private void InitializeGame()
    {
        Debug.Log("[GameManager] Khởi tạo game offline...");

        if (!enableNetworkMode)
            PhotonNetwork.OfflineMode = true;

        SpawnPlayer();
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

            // Lấy Health của nhân vật mình điều khiển
            currentPlayerHealth = currentPlayer.GetComponent<Health>();

            SetupCamera();
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
        gameTime = maxGameTime;

        if (gameUI != null)
            gameUI.SetActive(true);

        Debug.Log("[GameManager] Game đã bắt đầu!");
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

        Debug.Log("[GameManager] Game " + (isGamePaused ? "pause" : "resume"));
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
        }

        if (topDownCamera != null && currentPlayer != null)
        {
            topDownCamera.target = currentPlayer.transform;
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
            }
        }
    }

    public void CheckWinCondition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        int aliveCount = 0;
        GameObject lastAlive = null;

        foreach (GameObject player in players)
        {
            Health h = player.GetComponent<Health>();
            if (h != null && h.IsAlive())
            {
                aliveCount++;
                lastAlive = player;
            }
        }

        if (aliveCount == 1)
        {
            if (lastAlive == currentPlayer)
            {
                Win(); // bạn là người sống sót cuối
            }
            else
            {
                GameOver("Bạn đã thua!"); // người khác sống sót
            }
        }
        else if (aliveCount == 0)
        {
            GameOver("Không còn ai sống sót!");
        }
    }

}