using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AnimationMovement))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Component References")]
    public GameObject playerCamera;
    private AnimationMovement animationMovement;
    private Rigidbody rb;
    private Transform cameraTransform;

    [Header("Movement Stats")]
    public float moveSpeed = 5f;
    public float sprintSpeedMultiplier = 2f;
    public float rotationSpeed = 10f;

    private PlayerControls playerControls;
    private Vector2 moveInput;
    private bool isSprinting = false;
    private bool isInteracting = false;

    PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        animationMovement = GetComponent<AnimationMovement>();
        playerControls = new PlayerControls();

        if (!photonView.IsMine)
        {
            if (playerCamera != null)
            {
                playerCamera.GetComponent<Camera>().enabled = false;
                playerCamera.GetComponent<AudioListener>().enabled = false;
            }
            this.enabled = false;
        }
    }

    void Start()
    {
        if (playerCamera != null)
        {
            cameraTransform = playerCamera.transform;
        }
        else
        {
            Debug.LogError("Player Camera chưa được gán trong Inspector!", this);
        }

        playerControls.PlayerMovement.Enable();
        playerControls.PlayerMovement.Walk.performed += OnWalkPerformed;
        playerControls.PlayerMovement.Walk.canceled += OnWalkCanceled;
        playerControls.PlayerMovement.Sprint.performed += OnSprintPerformed;
        playerControls.PlayerMovement.Sprint.canceled += OnSprintCanceled;
        playerControls.PlayerMovement.Bomb.performed += OnBombPerformed;
    }

    private void OnDestroy()
    {
        if (photonView.IsMine && playerControls != null)
        {
            playerControls.PlayerMovement.Walk.performed -= OnWalkPerformed;
            playerControls.PlayerMovement.Walk.canceled -= OnWalkCanceled;
            playerControls.PlayerMovement.Sprint.performed -= OnSprintPerformed;
            playerControls.PlayerMovement.Sprint.canceled -= OnSprintCanceled;
            playerControls.PlayerMovement.Bomb.performed -= OnBombPerformed;
            playerControls.PlayerMovement.Disable();
        }
    }

    private void OnWalkPerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnWalkCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        isSprinting = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }

    private void OnBombPerformed(InputAction.CallbackContext context)
    {
        if (!isInteracting)
        {
            isInteracting = true;
            animationMovement.PlayTarget("Bomb", isInteracting);
        }
    }

    void Update()
    {
        if (isInteracting) return;
        animationMovement.ChangeAnimatorValues(moveInput.x, moveInput.y, isSprinting);
    }

    void FixedUpdate()
    {
        if (isInteracting) return;
        if (cameraTransform == null) return;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        HandleMovement(moveDirection);

        if (moveInput.y >= 0)
        {
            HandleRotation(moveDirection);
        }
    }

    private void HandleMovement(Vector3 moveDirection)
    {
        float currentSpeed = isSprinting ? moveSpeed * sprintSpeedMultiplier : moveSpeed;
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }

    private void HandleRotation(Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRotation);
        }
    }

    public void OnInteractionEnd()
    {
        isInteracting = false;
        animationMovement.PlayTarget("PlayerMovement", isInteracting);
    }
}
