using UnityEngine;
using TMPro;
using UnityEngine.UI; // Thư viện để làm việc với TextMeshPro

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    // --- UI ở màn hình MENU ---
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_InputField createRoomInput;
    [SerializeField] private TMP_InputField joinRoomInput;

    // --- BIẾN ĐỂ QUẢN LÝ UI TRONG LOBBY ---
    [Header("Lobby Panel")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private GameObject playerListItemPrefab; // "Mẫu" cho mỗi người chơi
    [SerializeField] private Transform playerListContent;   // Nơi để chứa danh sách người chơi

    // Thêm một nút để rời phòng
    [SerializeField] private Button leaveRoomButton;
    [SerializeField] private TextMeshProUGUI playerListText; // Sẽ nâng cấp sau

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
        InitializeUIListeners();
    }
    private void InitializeUIListeners()
    {
        // Gán sự kiện OnClick cho nút Rời phòng (nếu button tồn tại)
        if (leaveRoomButton != null)
        {
            leaveRoomButton.onClick.RemoveAllListeners(); // Xóa listener cũ để tránh duplicate
            leaveRoomButton.onClick.AddListener(OnClick_LeaveRoom);
        }

        // Tương tự, nếu các button tạo/join phòng là [SerializeField], gắn listener ở đây
        // Giả sử bạn có Button createButton và joinButton (thêm vào nếu chưa có)
        // [SerializeField] private Button createRoomButton;
        // [SerializeField] private Button joinRoomButton;
        // if (createRoomButton != null) {
        //     createRoomButton.onClick.RemoveAllListeners();
        //     createRoomButton.onClick.AddListener(OnClick_CreateRoom);
        // }
        // if (joinRoomButton != null) {
        //     joinRoomButton.onClick.RemoveAllListeners();
        //     joinRoomButton.onClick.AddListener(OnClick_JoinRoom);
        // }
    }
    // --- CÁC HÀM SẼ ĐƯỢC GỌI BỞI CÁC NÚT BẤM ---
    // Trong UIManager.cs
    void Start()
    {
        // Gán sự kiện OnClick cho nút Rời phòng
        leaveRoomButton.onClick.AddListener(OnClick_LeaveRoom);
    }

    void OnClick_LeaveRoom()
    {
        Photon.Pun.PhotonNetwork.LeaveRoom(); // Gọi hàm rời phòng của Photon
    }
    public void OnClick_CreateRoom()
    {
        // Kiểm tra xem input có rỗng không
        if (string.IsNullOrEmpty(createRoomInput.text))
        {
            Debug.Log("Tên phòng không được để trống!");
            return;
        }
        NetworkManager.Instance.SetPlayerName(playerNameInput.text);
        NetworkManager.Instance.CreateRoom(createRoomInput.text);
    }

    public void OnClick_JoinRoom()
    {
        // Kiểm tra xem input có rỗng không
        if (string.IsNullOrEmpty(joinRoomInput.text))
        {
            Debug.Log("Tên phòng không được để trống!");
            return;
        }
        NetworkManager.Instance.SetPlayerName(playerNameInput.text);
        NetworkManager.Instance.JoinRoom(joinRoomInput.text);
    }

    // --- CÁC HÀM ĐỂ CẬP NHẬT GIAO DIỆN ---

    // Trong UIManager.cs, thay thế hoàn toàn hàm UpdateLobbyUI cũ
    public void UpdateLobbyUI()
    {
        // Hiển thị ID/Tên phòng
        roomNameText.text = "Phòng: " + Photon.Pun.PhotonNetwork.CurrentRoom.Name;

        // Xóa danh sách người chơi cũ đi trước khi cập nhật
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // Tạo item mới cho mỗi người chơi trong phòng
        foreach (Photon.Realtime.Player player in Photon.Pun.PhotonNetwork.PlayerList)
        {
            // Tạo một bản sao của prefab
            GameObject playerItemObject = Instantiate(playerListItemPrefab, playerListContent);

            // Lấy script PlayerListItemUI từ bản sao vừa tạo và gọi hàm SetUp
            playerItemObject.GetComponent<PlayerListItemUI>().SetUp(player);
        }
    }
    // Trong UIManager.cs
    public void OnLeftRoom()
    {
        SetActivePanel("menu");
        InitializeUIListeners();

    }
    public void SetActivePanel(string panelName)
    {
        menuPanel.SetActive(panelName.Equals("menu"));
        lobbyPanel.SetActive(panelName.Equals("lobby"));
    }
}