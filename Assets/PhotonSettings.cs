using UnityEngine;
using Photon.Pun;

public class PhotonSettings : MonoBehaviour
{
    void Awake()
    {
        // Tăng số lần gửi gói tin mỗi giây
        PhotonNetwork.SendRate = 30;

        // Đồng bộ tốc độ nhận với tốc độ gửi
        PhotonNetwork.SerializationRate = 30;
    }
}
