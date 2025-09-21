using System.Collections;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Animator bowAnimator;
    private PlayerController playerController;
    private PlayerCombat playerCombat;

    private bool isAttacking = false;
    private bool isChop = false;
    private bool isAiming = false;

    private float acceleration = 4f;
    private float deceleration = 6f;
    private Vector2 currentVelocity = Vector2.zero;

    // Parameter Hashes
    private int moveXHash;
    private int moveYHash;
    private int isRunHash;
    private int isAimingHash;
    private int bowRecoilTriggerHash;

    public Animator Animator => animator;
    public bool IsAttacking => isAttacking;
    public bool IsChopping => isChop;
    public bool IsDead => animator.GetBool("isDead");

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
        isAimingHash = Animator.StringToHash("isBowDraw");
        bowRecoilTriggerHash = Animator.StringToHash("BowRecoil");
    }

    // ---------------- Movement ----------------
    public void UpdateAnimationState(Vector2 input, bool isRunning, float currentSpeed)
    {
        Vector2 targetVelocity = input.normalized * (currentSpeed / playerController.moveSpeed);
        currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity,
                            (isRunning ? acceleration : deceleration) * Time.deltaTime);

        animator.SetFloat(moveXHash, currentVelocity.x);
        animator.SetFloat(moveYHash, currentVelocity.y);
        animator.SetBool(isRunHash, isRunning);
    }

    // ---------------- Combat ----------------
    public void TriggerAttack(WeaponClass.WeaponType type)
    {
        if (isAttacking) return;
        isAttacking = true;

        switch (type)
        {
            case WeaponClass.WeaponType.Machete:
                animator.SetTrigger("isAttack");
                break;
            case WeaponClass.WeaponType.Sword:
            case WeaponClass.WeaponType.Bow:
                StartAim();
                break;
        }

        SetUpperBodyLayerWeight(1f);
    }

    public void StartAim()
    {
        if (isAiming) return;
        isAiming = true;

        animator.SetTrigger(isAimingHash);
        if (bowAnimator != null)
            bowAnimator.SetTrigger(isAimingHash);
    }

    public void ReleaseBow()
    {
        animator.SetTrigger(bowRecoilTriggerHash);
        if (bowAnimator != null)
            bowAnimator.SetTrigger(bowRecoilTriggerHash);
    }

    public void StopAimImmediate()
    {
        isAiming = false;
        animator.SetBool(isAimingHash, false);
        if (bowAnimator != null)
            bowAnimator.SetBool(isAimingHash, false);
    }

    // ---------------- Chop ----------------
    public void TriggerChop()
    {
        if (isChop) return;
        isChop = true;
        animator.SetBool("isChop", true);
    }

    public void ResetChop()
    {
        isChop = false;
        animator.SetBool("isChop", false);
    }

    public void OnChopImpact()
    {
        if (playerController.playerStateMachine.currentState is ChopState chopState)
        {
            chopState.OnChopImpact();
        }
    }

        // Animation Event cuối clip Chop gọi hàm này
    public void ChopEnd()
    {
        ResetChop();

        if (playerController.playerStateMachine.currentState is ChopState chopState)
        {
            chopState.OnChopEnd();
        }
    }

    // ---------------- Dead ----------------
    public void TriggerDead()
    {
        if (animator.GetBool("isDead")) return;
        animator.SetBool("isDead", true);
    }

    public void ResetDead()
    {
        animator.SetBool("isDead", false);
    }

    public void ResetAttack()
    {
        isAttacking = false;
        isAiming = false;
        SetUpperBodyLayerWeight(0f);
        if (bowAnimator != null)
        {
            bowAnimator.SetBool(isAimingHash, false);
            bowAnimator.ResetTrigger(bowRecoilTriggerHash);
        }
    }

    public void Attack()
    {
        playerCombat.HandleAtack();
    }

    // ---------------- Upper Body Layer ----------------
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
