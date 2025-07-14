using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface  IUsableItem 
{
    void UseItem(PlayerStatus playerStatus,PlayerInventory player);
}
