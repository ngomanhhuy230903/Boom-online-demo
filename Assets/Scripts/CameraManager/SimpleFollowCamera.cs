using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    // Mục tiêu mà camera sẽ đi theo (chính là nhân vật Player)
    public Transform target;

    // Tốc độ di chuyển và xoay mượt mà của camera
    public float smoothSpeed = 0.125f;

    // Khoảng cách từ đầu nhân vật. Ví dụ: (0, 1.8f, -1.0f) cho góc nhìn qua vai.
    public Vector3 offset;

    /// <summary>
    /// LateUpdate được gọi sau khi tất cả các hàm Update đã được thực thi.
    /// </summary>
    void LateUpdate()
    {
        // Kiểm tra xem target có tồn tại không để tránh lỗi
        if (target == null)
        {
            Debug.LogWarning("Camera Follow target is not assigned!");
            return;
        }

        // Vị trí mong muốn của camera = vị trí nhân vật + offset được xoay theo hướng của nhân vật
        Vector3 desiredPosition = target.position + (target.rotation * offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Xoay camera để nhìn cùng hướng với nhân vật một cách mượt mà
        Quaternion desiredRotation = target.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed);
    }
}
