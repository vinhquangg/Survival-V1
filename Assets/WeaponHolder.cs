using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [System.Serializable]
    public class WeaponSlot
    {
        public WeaponClass weaponData;           
        public GameObject weaponObject;
    }

    public List<WeaponSlot> weaponSlots = new List<WeaponSlot>();
    public GameObject currentWeapon { set; get; }

    public void DisplayWeapon(ItemClass weaponID)
    {
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }

        foreach (var weapon in weaponSlots)
        {
            if (weapon.weaponData == weaponID)
            {
                currentWeapon = weapon.weaponObject;
                currentWeapon.SetActive(true);
                return;
            }
        }

        Debug.Log($"Weapon with ID {weaponID} not found in weapon slots.");
    }

    public void HideWeapon()
    {
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }
    }

    public bool HasWeaponEquipped()
    {
        return currentWeapon != null && currentWeapon.activeSelf;
    }
}
