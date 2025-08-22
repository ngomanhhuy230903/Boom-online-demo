using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    // --- Biến để TỰ ĐỘNG TEST ---
    private string testRoomName = "PhongTest123";
    private string playerName; // Thêm biến để lưu tên người chơi

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
        Debug.Log("[NetworkManager] Đang kết nối tới Master Server...");
        PhotonNetwork.ConnectUsingSettings();
    }

    // =================================================================
    // CÁC HÀM PUBLIC DÀNH CHO UI (SẼ DÙNG Ở CÁC TASK SAU)
    // Hiện tại chúng ta chưa gọi các hàm này, nhưng đã viết sẵn cho Thắng (UI Dev)
    // =================================================================
    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public void CreateRoom(string roomName)
    {
        if (!PhotonNetwork.IsConnected) return;
        PhotonNetwork.NickName = playerName;
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinRoom(string roomName)
    {
        if (!PhotonNetwork.IsConnected) return;
        PhotonNetwork.NickName = playerName;
        PhotonNetwork.JoinRoom(roomName);
    }
    // =================================================================

    // --- Các sự kiện (Callback) của Photon ---

    public override void OnConnectedToMaster()
    {
        Debug.Log("<color=green>[NetworkManager] Đã kết nối tới Master Server!</color>");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("<color=cyan>[NetworkManager] Đã vào sảnh chờ (Lobby)! Sẵn sàng để Tạo/Vào phòng.</color>");

        //// =================================================================
        //// KHU VỰC TỰ ĐỘNG TEST
        //// =================================================================
        //// 1. Tự động gán một tên ngẫu nhiên cho người chơi để test
        //PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
        //Debug.Log($"[NetworkManager-TEST] Đã gán tên người chơi là: {PhotonNetwork.NickName}");

        //// 2. Tự động vào hoặc tạo phòng test
        //Debug.Log($"[NetworkManager-TEST] Đang cố gắng vào phòng '{testRoomName}'...");
        //PhotonNetwork.JoinOrCreateRoom(testRoomName, new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
        //// =================================================================
    }

    // Trong NetworkManager.cs

    public override void OnJoinedRoom()
    {
        Debug.Log($"<color=green>[NetworkManager] Người chơi '{PhotonNetwork.NickName}' đã vào phòng '{PhotonNetwork.CurrentRoom.Name}' thành công!</color>");

        // --- GỌI UIMANAGER ĐỂ CẬP NHẬT GIAO DIỆN ---
        // Dòng này sẽ được gọi ở TẤT CẢ client khi họ vào phòng thành công
        // Chúng ta sẽ cài đặt UI cho Lobby ở phần 3
        UIManager.Instance.SetActivePanel("lobby"); // Ẩn menu, hiện lobby
        UIManager.Instance.UpdateLobbyUI();

        // Chỉ Master Client mới Load Scene
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    PhotonNetwork.LoadLevel("LobbyScene");
        //}
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"<color=yellow>[NetworkManager] Người chơi mới '{newPlayer.NickName}' đã vào phòng!</color>");

        // --- GỌI UIMANAGER ĐỂ CẬP NHẬT LẠI DANH SÁCH NGƯỜI CHƠI ---
        if (UIManager.Instance != null) UIManager.Instance.UpdateLobbyUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"<color=orange>[NetworkManager] Người chơi '{otherPlayer.NickName}' đã rời phòng.</color>");

        // --- GỌI UIMANAGER ĐỂ CẬP NHẬT LẠI DANH SÁCH NGƯỜI CHƠI ---
        if (UIManager.Instance != null) UIManager.Instance.UpdateLobbyUI();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // Được gọi khi chủ phòng rời đi và một người khác được đôn lên làm chủ phòng mới
        Debug.Log($"<color=magenta>[NetworkManager] Chủ phòng đã thay đổi! Chủ phòng mới là '{newMasterClient.NickName}'.</color>");
    }
    // ---------------------------------------------

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"<color=red>[NetworkManager] Tạo phòng thất bại: {message} (Code: {returnCode})</color>");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"<color=red>[NetworkManager] Vào phòng thất bại: {message} (Code: {returnCode})</color>");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"<color=red>[NetworkManager] Mất kết nối tới server: {cause}</color>");
    }
    // Trong NetworkManager.cs
    public override void OnLeftRoom()
    {
        Debug.Log("<color=orange>[NetworkManager] Bạn đã rời phòng.</color>");
        // Yêu cầu UIManager chuyển về giao diện Menu
        UIManager.Instance.OnLeftRoom();
        // Tải lại MenuScene để đảm bảo mọi thứ được reset
        //SceneManager.LoadScene("MenuScene");
        // Kết nối lại với Master Server và vào lobby
    if (!PhotonNetwork.IsConnected)
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    else
    {
        PhotonNetwork.JoinLobby();
    }
    }
}