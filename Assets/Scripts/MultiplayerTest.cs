using UnityEngine;
using Photon.Pun;

public class MultiplayerTest : MonoBehaviour
{
    PhotonView photonView;
    public GameObject playerCamera; // Bắt buộc phải gán trong Inspector

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        Renderer rend = GetComponentInChildren<Renderer>();

        // THAY ĐỔI: Thêm kiểm tra "photonView.Owner != null"
        // để đảm bảo thông tin chủ sở hữu đã được đồng bộ qua mạng.
        if (rend != null && photonView.Owner != null)
        {
            float hue = (photonView.Owner.ActorNumber * 0.2f) % 1f;
            rend.material.color = Color.HSVToRGB(hue, 1, 1);
        }

        // THAY ĐỔI: Cập nhật Log để an toàn hơn khi Owner có thể bị null
        string ownerId = photonView.Owner == null ? "UNKNOWN" : photonView.Owner.ActorNumber.ToString();
        Debug.Log($"AWAKE cho người chơi {ownerId}. Đây có phải nhân vật của tôi không? {photonView.IsMine}", this.gameObject);

        if (!photonView.IsMine)
        {
            Debug.Log($"Người chơi {ownerId} KHÔNG PHẢI CỦA TÔI. Vô hiệu hóa camera.", this.gameObject);
            if (playerCamera != null)
            {
                playerCamera.SetActive(false);
            }
        }
        else
        {
            Debug.Log($"Người chơi {ownerId} LÀ CỦA TÔI. Camera sẽ hoạt động.", this.gameObject);
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 3f);
        }
    }
}
