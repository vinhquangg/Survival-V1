using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputHandler inputHandler;
    private CharacterController controller;
    private AnimationStateController animationController;

    public float moveSpeed = 5f;
    public float lookSensitivity = 1f;
    public Transform playerCamera;
    private float xRotation = 0f;

    public float gravity = -9.81f;
    public float groundCheckDistance = 0.4f;
    public Transform groundCheck;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        controller = GetComponent<CharacterController>();
        animationController = GetComponent<AnimationStateController>();
    }

    private void Start()
    {
        inputHandler.EnablePlayerInput();
        if (GameManager.instance != null)
        {
            GameManager.instance.SetCursorLock(true);
        }
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 moveInput = inputHandler.playerAction.Move.ReadValue<Vector2>();
        HandleMove(moveInput);
        HandleLook();

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        HandleAtack();
    }

    private void HandleMove(Vector2 moveInput)
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float inputMagnitude = moveInput.magnitude;

        bool isMoving = inputMagnitude >= 0.1f;

        bool isRunning = isMoving && moveInput.y > 0f && Input.GetKey(KeyCode.LeftShift);

        float speedMultiplier = isRunning ? 1f : (isMoving ? 0.5f : 0f);
        controller.Move(move * moveSpeed * speedMultiplier * Time.deltaTime);

        animationController.UpdateAnimationState(moveInput, isRunning);
    }

    private void HandleLook()
    {
        Vector2 look = inputHandler.playerAction.Look.ReadValue<Vector2>();
        look *= lookSensitivity * Time.deltaTime;

        xRotation -= look.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * look.x);
    }

    private void HandleAtack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animationController.TriggerAttack();
        }
    }
}
