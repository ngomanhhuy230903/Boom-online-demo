using UnityEngine;

public class TopDownFollowCamera : MonoBehaviour
{
	[Header("Target")]
	public Transform target;

	[Header("Positioning")]
	public float height = 8f;        // Độ cao so với nhân vật
	public float distance = 6f;      // Khoảng cách phía sau nhân vật
	public float yawOffset = 0f;     // Offset góc quay quanh trục Y
	public bool followTargetYaw = true; // Có bám theo hướng quay của nhân vật không

	[Header("Smoothing")]
	public float positionLerp = 0.12f;
	public float rotationLerp = 0.12f;

	[Header("Look Angle")]
	public float pitchDegrees = 55f; // Góc nhìn từ trên xuống

	void LateUpdate()
	{
		if (target == null)
		{
			Debug.LogWarning("TopDownFollowCamera: Target is not assigned!");
			return;
		}

		float yaw = followTargetYaw ? target.eulerAngles.y + yawOffset : yawOffset;

		// Tính vị trí mong muốn: quay quanh Y, lùi theo distance và nâng theo height
		Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);
		Vector3 desiredPosition = target.position + (yawRotation * new Vector3(0f, 0f, -distance)) + Vector3.up * height;
		transform.position = Vector3.Lerp(transform.position, desiredPosition, positionLerp);

		// Nhìn xuống mục tiêu với góc pitch cố định và yaw tính ở trên
		Quaternion desiredRotation = Quaternion.Euler(pitchDegrees, yaw, 0f);
		transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationLerp);
	}
} 