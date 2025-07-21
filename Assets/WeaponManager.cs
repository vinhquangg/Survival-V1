using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    public WeaponHolder weaponHolder;
    public Animator playerAnimator;

    private WeaponClass currentWeaponItem;

    public void EquipWeapon(ItemClass item)
    {
        if (item is WeaponClass weapon)
        {
            currentWeaponItem = weapon;

            weaponHolder.DisplayWeapon(weapon);

            if (playerAnimator != null)
            {
                playerAnimator.SetInteger("WeaponType", (int)weapon.weaponType);
                playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("UpperLayer"), 1f);
            }
        }
        else
        {
            UnequipWeapon();
        }
    }

    public void UnequipWeapon()
    {
        currentWeaponItem = null;
        weaponHolder.HideWeapon();

        if (playerAnimator != null)
        {
            playerAnimator.SetInteger("WeaponType", 0);
            playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("UpperLayer"), 0f);
        }
    }

    public bool HasWeaponEquipped()
    {
        return currentWeaponItem != null;
    }

    public WeaponClass GetCurrentWeapon()
    {
        return currentWeaponItem;
    }
}
