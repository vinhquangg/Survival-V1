using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInterfaceMonsterState
{
    void EnterState();
    void UpdateState();
    void FixedUpdateState();
    void ExitState();
}
