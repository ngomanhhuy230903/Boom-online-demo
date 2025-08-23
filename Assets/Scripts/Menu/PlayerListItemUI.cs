using UnityEngine;
using TMPro;
using Photon.Realtime; // Thêm thư viện Photon Realtime

public class PlayerListItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;

    // Một hàm public để UIManager có thể gọi và truyền thông tin người chơi vào
    public void SetUp(Player player)
    {
        playerNameText.text = player.NickName;

        // Mở rộng: Đánh dấu chủ phòng
        if (player.IsMasterClient)
        {
            playerNameText.color = Color.yellow;
            playerNameText.text += " (Chủ phòng)";
        }
    }
}