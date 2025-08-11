using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine : MonoBehaviour
{
    public IInterfaceMonsterState _currentState { get; private set; }
    //public MonsterData monsterData;   
    public Animator animator { get; private set; }
    public Rigidbody _rigidbody { get; private set; }
    public BaseMonster baseMonster { get; private set; }

    public Dictionary<string, System.Func<IInterfaceMonsterState>> stateFactory;
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        baseMonster = GetComponent<BaseMonster>();
    }
    protected virtual void Start()
    {
        stateFactory = new Dictionary<string, System.Func<IInterfaceMonsterState>>()
        {

            { "MonsterIdleState",() => new MonsterIdleState(this) },
            { "MonsterPatrolState",() => new MonsterPatrolState(this)},
            { "MonsterChaseState",() => new MonsterChaseState(this)},
            { "MonsterAttackState",() => new MonsterAttackState(this)},
            //{ "MonsterDeadState",() => new MonsterDeadState(this)}

        };
        SwitchState(stateFactory["MonsterIdleState"]());
    }

    public void SwitchState(IInterfaceMonsterState newState)
    {
        //if (monsterState != null) return;
        if(_currentState != null)
        {
            _currentState.ExitState();
        }
        _currentState = newState;
        _currentState.EnterState();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(_currentState != null)
        {
            _currentState.UpdateState();
        }
    }
    protected virtual void FixedUpdate()
    {
        if (_currentState != null)
        {
            _currentState.FixedUpdateState();
        }
    }

    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }
}
