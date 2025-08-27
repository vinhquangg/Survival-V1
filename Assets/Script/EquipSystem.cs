using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSystem : MonoBehaviour
{
    //------UI--------
    public GameObject quickSlot;
    public List<GameObject> quickSlotItems = new List<GameObject>();
    [SerializeField] private EquipHolder equipHolder;
    public static EquipSystem instance { get;  set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        RefreshQuickSlot();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshQuickSlot()
    {
        quickSlotItems.Clear();
        for (int i = 0; i < quickSlot.transform.childCount; i++)
        {
            GameObject child = quickSlot.transform.GetChild(i).gameObject;
            quickSlotItems.Add(child);
        }

    }

    public void TryEquipWeaponHotbar()
    {
        foreach(var slotUI in quickSlotItems)
        {
            var uiRef = slotUI.GetComponent<SlotUIRef>();
            if(uiRef == null)
            {
                continue;
            }

            //var inventorySlot = uiRef.
        }
    }
}
