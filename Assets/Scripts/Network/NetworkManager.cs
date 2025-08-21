using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

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

    public override void OnConnectedToMaster()
    {
        Debug.Log("<color=green>[NetworkManager] Đã kết nối tới Master Server!</color>");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("<color=cyan>[NetworkManager] Đã vào sảnh chờ (Lobby)! Sẵn sàng để Tạo/Vào phòng.</color>");
        PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
        Debug.Log($"[NetworkManager-TEST] Đang cố gắng vào phòng '{testRoomName}'...");
        PhotonNetwork.JoinOrCreateRoom(testRoomName, new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"<color=green>[NetworkManager] Người chơi '{PhotonNetwork.NickName}' đã vào phòng '{PhotonNetwork.CurrentRoom.Name}' thành công!</color>");

        // --- LOGIC SPAWN MỚI ---
        // Đây là thời điểm an toàn để tạo nhân vật.

        // Sử dụng ActorNumber để tạo vị trí xuất hiện riêng cho mỗi người.
        Vector3 spawnPosition = new Vector3((PhotonNetwork.LocalPlayer.ActorNumber - 1) * 2.0f, 0, 0);

        // Tạo ra nhân vật tại vị trí đã tính toán
        PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);

        Debug.Log($"NetworkManager đã tạo nhân vật cho người chơi {PhotonNetwork.LocalPlayer.ActorNumber} tại vị trí {spawnPosition}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"<color=yellow>[NetworkManager] Người chơi mới '{newPlayer.NickName}' đã vào phòng!</color>");
    }

    // --- Các hàm callback khác giữ nguyên ---

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"<color=orange>[NetworkManager] Người chơi '{otherPlayer.NickName}' đã rời phòng.</color>");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"<color=magenta>[NetworkManager] Chủ phòng đã thay đổi! Chủ phòng mới là '{newMasterClient.NickName}'.</color>");
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
