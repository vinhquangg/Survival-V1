using UnityEngine;

public class HotbarSelector : MonoBehaviour
{
    [SerializeField] private RectTransform selector;   // Hình highlight (UI)
    [SerializeField] private InventoryUIHandler hotbarUI; // Tham chiếu đến Hotbar UI (InventoryUIHandler với area = Hotbar)

    private int currentIndex = -1;

    private void Start()
    {
        if (hotbarUI != null && hotbarUI.GetSlotCount() > 0)
        {
            // 🔹 Khi bắt đầu game, auto highlight slot 0
            SelectSlot(currentIndex);
        }
        else if (selector != null)
        {
            selector.gameObject.SetActive(false);
        }
    }

    public void SelectSlot(int index)
    {
        if (hotbarUI == null || selector == null) return;
        if (index < 0 || index >= hotbarUI.GetSlotCount()) return;

        currentIndex = index;

        // Lấy vị trí slot trong hotbar
        RectTransform target = hotbarUI.transform.GetChild(index).GetComponent<RectTransform>();

        // Đặt selector vào đúng slot
        selector.position = target.position;

        // Bật highlight
        selector.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (selector != null)
            selector.gameObject.SetActive(false);

        currentIndex = -1;
    }

    public int GetCurrentIndex() => currentIndex;
}
