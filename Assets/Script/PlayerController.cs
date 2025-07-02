using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public InputHandler inputHandler { get; private set; }
    public CharacterController controller { get; private set; }
    public AnimationStateController animationController { get; private set; }
    private PlayerStateMachine playerStateMachine;

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

    public float weaponHitRadius;
    public Transform weaponHitPoint;
    public int damage;
    public LayerMask targetMask;
    public GameObject camera;
    private void Awake()
    {
        if (camera == null)
        {
            camera = Camera.main.gameObject;
        }
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
        playerStateMachine = new PlayerStateMachine();
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

    public void HandleAtack()
    {
        Collider[] hitTargets = Physics.OverlapSphere(weaponHitPoint.position, weaponHitRadius, targetMask);
        Debug.Log($"Found {hitTargets.Length} targets");
        foreach (var target in hitTargets)
        {
            Debug.Log($"Hit {target.name} with damage: {damage}");

            if (target.TryGetComponent<BaseMonster>(out var monster))
            {
                monster.TakeDamage(damage);
            }
        }
    }

    public void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}
