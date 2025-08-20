using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    // --- Biến để TỰ ĐỘNG TEST ---
    // Đặt tên phòng bạn muốn test ở đây
    private string testRoomName = "PhongTest123";

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

        // =================================================================
        // KHU VỰC TỰ ĐỘNG TEST - BẠN CÓ THỂ CHỈNH SỬA Ở ĐÂY
        // =================================================================
        // Khi vào sảnh, chúng ta sẽ tự động tạo hoặc vào một phòng

        Debug.Log($"[NetworkManager-TEST] Đang cố gắng vào phòng '{testRoomName}'...");
        PhotonNetwork.JoinOrCreateRoom(testRoomName, new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);

        // PhotonNetwork.JoinOrCreateRoom() là một hàm rất tiện lợi:
        // - Nếu phòng `testRoomName` đã tồn tại, nó sẽ cố gắng vào.
        // - Nếu phòng chưa tồn tại, nó sẽ tự động tạo ra phòng đó.
        // Điều này giúp bạn dễ dàng test với nhiều người chơi.
        // =================================================================
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"<color=green>[NetworkManager] Đã vào phòng '{PhotonNetwork.CurrentRoom.Name}' thành công!</color>");
        Debug.Log($"[NetworkManager] Số người chơi hiện tại: {PhotonNetwork.CurrentRoom.PlayerCount}");

        // Khi vào phòng, chúng ta sẽ in ra danh sách người chơi
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"[NetworkManager] Người chơi trong phòng: {player.NickName}");
        }

        // Chúng ta có thể load scene game ở đây để test
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     Debug.Log("[NetworkManager] Tôi là Master Client, đang load scene game...");
        //     PhotonNetwork.LoadLevel("InGame"); // Thay "InGame" bằng tên scene game của bạn
        // }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Được gọi khi một người chơi mới vào phòng MÀ BẠN ĐANG Ở TRONG
        Debug.Log($"<color=yellow>[NetworkManager] Người chơi mới '{newPlayer.NickName}' đã vào phòng!</color>");
    }

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
}