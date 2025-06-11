using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;

    // blend tốc độ mượt hơn nếu cần
    private float acceleration = 4f;
    private float deceleration = 6f;
    private Vector2 currentVelocity = Vector2.zero;

    private int moveXHash;
    private int moveYHash;
    private int isRunHash;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found!");
            return;
        }

        moveXHash = Animator.StringToHash("Velocity X");
        moveYHash = Animator.StringToHash("Velocity Y");
        isRunHash = Animator.StringToHash("isRun");
    }

    public void UpdateAnimationState(Vector2 input, bool isRunning)
    {
        // mượt hóa input (nếu cần, không bắt buộc)
        currentVelocity = Vector2.MoveTowards(currentVelocity, input,
                            (isRunning ? acceleration : deceleration) * Time.deltaTime);

        // Gán giá trị vào Animator
        animator.SetFloat(moveXHash, currentVelocity.x);
        animator.SetFloat(moveYHash, currentVelocity.y);
        animator.SetBool(isRunHash, isRunning);
    }
}
