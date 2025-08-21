using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [Header("Component References")]
    private AnimationMovement animationMovement;
    private Rigidbody rb;

    [Header("Movement Stats")]
    public float moveSpeed = 5f;
    public float sprintSpeedMultiplier = 2f;
    public float rotationSpeed = 10f;

    private PlayerControls playerControls;
    private Vector2 moveInput;
    private bool isSprinting = false;
    private bool isInteracting = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animationMovement = GetComponent<AnimationMovement>();
        playerControls = new PlayerControls();
    }

    void OnEnable()
    {
        playerControls.PlayerMovement.Enable();
        playerControls.PlayerMovement.Walk.performed += OnWalkPerformed;
        playerControls.PlayerMovement.Walk.canceled += OnWalkCanceled;
        playerControls.PlayerMovement.Sprint.performed += OnSprintPerformed;
        playerControls.PlayerMovement.Sprint.canceled += OnSprintCanceled;

        playerControls.PlayerMovement.Bomb.performed += OnBombPerformed;
        //playerControls.PlayerMovement.Bomb.canceled += OnBombCanceled;
    }

    void OnDisable()
    {
        playerControls.PlayerMovement.Walk.performed -= OnWalkPerformed;
        playerControls.PlayerMovement.Walk.canceled -= OnWalkCanceled;
        playerControls.PlayerMovement.Sprint.performed -= OnSprintPerformed;
        playerControls.PlayerMovement.Sprint.canceled -= OnSprintCanceled;
        playerControls.PlayerMovement.Bomb.performed -= OnBombPerformed;
        //playerControls.PlayerMovement.Bomb.canceled -= OnBombCanceled;

        playerControls.PlayerMovement.Disable();
    }

    #region Input Event Handlers
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
    //private void OnBombCanceled(InputAction.CallbackContext context)
    //{
    //        isInteracting = false;
    //        animationMovement.PlayTarget("PlayerMovement", isInteracting);
    //}
    #endregion

    void Update()
    {
        if (isInteracting) return;

        animationMovement.ChangeAnimatorValues(moveInput.x, moveInput.y, isSprinting);
    }

    void FixedUpdate()
    {
        if (isInteracting) return;

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        HandleMovement(moveDirection);
        HandleRotation(moveDirection);
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
        Debug.Log("Interaction ended, resetting state.");
        isInteracting = false;
        // SỬA LỖI: Dùng tên state "PlayerMovement" thay vì "Empty" để khớp với Animator của bạn
        animationMovement.PlayTarget("PlayerMovement", isInteracting);
    }
}
