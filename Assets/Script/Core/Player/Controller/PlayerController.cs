using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public InputHandler inputHandler { get; private set; }
    public CharacterController controller { get; private set; }
    public AnimationStateController animationController { get; private set; }
    public PlayerStateMachine playerStateMachine { get; private set; }
    public PlayerCombat combat { get; private set; }
    public EquipManager equipManager;
    public float moveSpeed = 5f;
    public float lookSensitivity = 1f;
    public Transform playerCamera;
    private float xRotation = 0f;
    private Coroutine rotateCoroutine;
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.4f;
    public Transform groundCheck;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    public GameObject mainCamera;

    //public float weaponHitRadius;
    //public Transform weaponHitPoint;
    //public int damage;
    //public LayerMask targetMask;
    private void Awake()
    {
        // Lấy camera trong scene
        if (mainCamera == null)
        {
            mainCamera = Camera.main?.gameObject;
        }

        if (mainCamera != null && playerCamera == null)
        {
            playerCamera = mainCamera.transform;
        }
        inputHandler = GetComponent<InputHandler>();
        controller = GetComponent<CharacterController>();
        animationController = GetComponent<AnimationStateController>();
        combat = GetComponent<PlayerCombat>();
        playerStateMachine = new PlayerStateMachine();
    }

    private void Start()
    {
        inputHandler.EnablePlayerInput();
        if (GameManager.instance != null)
        {
            GameManager.instance.SetCursorLock(true);
        }

        // 🔥 Gắn actor cho FootStepManager ngay khi player spawn
        if (FootStepManager.Instance != null)
        {
            FootStepManager.Instance.AttachToActor(transform);
        }

        playerStateMachine.Initialized(new IdleState(playerStateMachine, this));
    }


    private void Update()
    {
        playerStateMachine.Update();
    }

    public void HandleLook()
    {
        Vector2 look = inputHandler.playerAction.Look.ReadValue<Vector2>();
        look *= lookSensitivity * Time.deltaTime;

        xRotation -= look.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * look.x);
    }

    public void UpdatePlayerInputState()
    {
        // Chỉ cho phép input nếu cả Inventory và Crafting đóng
        bool inventoryOpen = InventoryManager.Instance != null && InventoryManager.Instance.IsOpen();
        bool craftingOpen = CraftingManager.instance != null && CraftingManager.instance.IsOpen();

        if (inventoryOpen || craftingOpen)
            inputHandler.DisablePlayerInput();
        else
            inputHandler.EnablePlayerInput();
    }


    public void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void RotateTowards(Vector3 targetPos, float rotateSpeed = 10f)
    {
        // Ngắt coroutine cũ nếu đang chạy
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        Vector3 dir = targetPos - transform.position;
        dir.y = 0; // chỉ quay ngang
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            rotateCoroutine = StartCoroutine(SmoothRotate(targetRot, rotateSpeed));
        }
    }

    private IEnumerator SmoothRotate(Quaternion targetRot, float rotateSpeed)
    {
        float angle = Quaternion.Angle(transform.rotation, targetRot);

        // Xoay cho đến khi chênh lệch góc rất nhỏ
        while (angle > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * rotateSpeed
            );

            angle = Quaternion.Angle(transform.rotation, targetRot);
            yield return null;
        }

        transform.rotation = targetRot; // fix lệch cuối
    }

}
