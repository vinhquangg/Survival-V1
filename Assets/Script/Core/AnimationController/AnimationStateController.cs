using System.Collections;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;
    private PlayerCombat playerCombat;
    private bool isAttacking = false;

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

        playerController = GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombat>();
        moveXHash = Animator.StringToHash("Velocity X");
        moveYHash = Animator.StringToHash("Velocity Y");
        isRunHash = Animator.StringToHash("isRun");
    }

    public void UpdateAnimationState(Vector2 input, bool isRunning)
    {
        currentVelocity = Vector2.MoveTowards(currentVelocity, input,
                            (isRunning ? acceleration : deceleration) * Time.deltaTime);

        animator.SetFloat(moveXHash, currentVelocity.x);
        animator.SetFloat(moveYHash, currentVelocity.y);
        animator.SetBool(isRunHash, isRunning);
    }

    public void TriggerAttack()
    {
        if (isAttacking) return; 
        isAttacking = true;
        animator.SetTrigger("isAttack");
    }

    public void ResetAttack()
    {
        isAttacking = false;
        SetUpperBodyLayerWeight(0f);
    }

    public void Attack()
    {
        Debug.Log("⚔️ Animation event gọi Attack()");
        playerCombat.HandleAtack();
    }


    public void SetUpperBodyLayerWeight(float weight)
    {
        animator.SetLayerWeight(1, weight); 
    }

    public void DisableUpperBodyLayerDelayed(float delay)
    {
        StartCoroutine(DisableLayerCoroutine(delay));
    }

    private IEnumerator DisableLayerCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetLayerWeight(1, 0f);
    }
}
