using UnityEngine;
using Photon.Pun;

public class AnimationMovement : MonoBehaviour
{
    private Animator animator;
    private PhotonView photonView;

    void Awake()
    {
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
    }

    public void ChangeAnimatorValues(float horizontal, float vertical, bool isSprinting)
    {
        // Chỉ chủ sở hữu mới có quyền thay đổi animation của chính mình
        if (!photonView.IsMine)
        {
            return;
        }

        float snappedHorizontal = SnapValue(horizontal);
        float snappedVertical = SnapValue(vertical);

        if (isSprinting)
        {
            snappedVertical = 2f;
        }

        animator.SetFloat("Horizontal", snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", snappedVertical, 0.1f, Time.deltaTime);
    }

    public void PlayTarget(string targetAnimation, bool isInteracting)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0.2f);
    }

    private float SnapValue(float value)
    {
        if (value > 0 && value < 0.55f) return 0.5f;
        if (value > 0.55f) return 1f;
        if (value < 0 && value > -0.55f) return -0.5f;
        if (value < -0.55f) return -1f;
        return 0f;
    }
}
