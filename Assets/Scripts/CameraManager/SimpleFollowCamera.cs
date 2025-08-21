using UnityEngine;
using Photon.Pun;

public class SimpleFollowCamera : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;

    public Vector3 offset = new Vector3(0, 3f, -0.1f);
    public Vector3 focusPointOffset = new Vector3(0, 2.9f, 0);

    PhotonView photonView;
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
            else
            {
                Debug.LogError("Camera target not set and could not find an object with the 'Player' tag.");
            }
        }
        offset = new Vector3(0, 2.5f, -2.5f);
        focusPointOffset = new Vector3(0, 1.8f, 5);
    }
    private void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<SimpleFollowCamera>().gameObject);
        }
    }

    void LateUpdate()
    {
        
        if (target == null)
        {
            return;
        }

        float scaleMultiplier = target.localScale.y;

        Vector3 scaledOffset = offset * scaleMultiplier;
        Vector3 scaledFocusPointOffset = focusPointOffset * scaleMultiplier;

        Vector3 desiredPosition = target.position + (target.rotation * scaledOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        Vector3 focusPoint = target.position + (target.rotation * scaledFocusPointOffset);
        Quaternion desiredRotation = Quaternion.LookRotation(focusPoint - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed);
    }
}
