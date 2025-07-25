using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimalState
{
    void Enter();
    void Tick();
    void Exit();
}
