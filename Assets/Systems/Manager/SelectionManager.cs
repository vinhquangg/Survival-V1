using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public float interactionDistance = 3f;
    public Transform cursorTransform;
    public LayerMask interactableLayer;
    private BuildableObject currentBuildable;
    private IInteractable currentInteractable;
    public IInteractable CurrentInteractable => currentInteractable;

    private PlayerUIManager uiManager;

    void Start()
    {
        uiManager = FindObjectOfType<PlayerUIManager>();
        if (uiManager == null)
            Debug.LogError("Không tìm thấy PlayerUIManager trong scene!");
    }

    void Update()
    {
        Ray ray = new Ray(cursorTransform.position, cursorTransform.forward);
        RaycastHit hit;

        float sphereRadius = 0.3f;

        if (Physics.SphereCast(ray, sphereRadius, out hit, interactionDistance, interactableLayer))
        {
            var interactable = hit.transform.GetComponent<IInteractable>();
            var info = hit.transform.GetComponent<IInteractableInfo>();

            if (interactable != null && info != null)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable = interactable;

                    // Cập nhật UI dựa vào loại interaction
                    switch (info.GetInteractionType())
                    {
                        case InteractionType.Pickup:
                            uiManager.ShowPrompt(info);
                            uiManager.HideCraftingInfo();
                            break;

                        case InteractionType.Chop:
                            uiManager.ShowPrompt(info);
                            uiManager.HideCraftingInfo();
                            break;

                        case InteractionType.Placeable:
                            var buildable = hit.transform.GetComponent<BuildableObject>();
                            if (buildable != null)
                            {
                                // Nếu đã đổi sang buildable khác, bỏ đăng ký sự kiện buildable cũ
                                if (currentBuildable != buildable)
                                {
                                    if (currentBuildable != null)
                                        currentBuildable.OnMaterialChanged -= OnBuildableMaterialChanged;

                                    currentBuildable = buildable;

                                    if (!buildable.IsBuilt)
                                        buildable.OnMaterialChanged += OnBuildableMaterialChanged;
                                }

                                if (buildable.IsBuilt)
                                {
<<<<<<< HEAD
<<<<<<< HEAD
                                    var campfire = hit.transform.GetComponent<Campfire>();
                                    if (campfire != null)
=======
                                    var cookable = hit.transform.GetComponent<Cookable>();
                                    if (cookable != null)
>>>>>>> parent of 1f79ee6 (make cooked meat)
                                    {
                                        uiManager.ShowPrompt(cookable);  // Cookable cũng phải implement IInteractableInfo
                                        uiManager.HideCraftingInfo();
                                    }
                                    else
                                    {
                                        uiManager.HidePrompt();
                                        uiManager.HideCraftingInfo();
                                    }
=======
                                    uiManager.HidePrompt();
                                    uiManager.HideCraftingInfo();
>>>>>>> parent of 7862a86 (make cooked meat)
                                }
                                else
                                {
                                    uiManager.ShowCraftingInfo(buildable.GetBlueprint(), buildable);
                                    uiManager.HidePrompt();
                                }
                            }

                            else
                            {
                                uiManager.HidePrompt();
                                uiManager.HideCraftingInfo();
                            }
                            break;



                        default:
                            uiManager.HidePrompt();
                            uiManager.HideCraftingInfo();
                            break;
                    }
                }
                return;
            }
        }

        // Không trúng gì → clear
        currentInteractable = null;
        uiManager.HidePrompt();
        uiManager.HideCraftingInfo();
        if (currentBuildable != null)
        {
            currentBuildable.OnMaterialChanged -= OnBuildableMaterialChanged;
            currentBuildable = null;
        }

    }

    private void OnBuildableMaterialChanged()
    {
        if (currentBuildable != null && !currentBuildable.IsBuilt)
        {
            uiManager.ShowCraftingInfo(currentBuildable.GetBlueprint(), currentBuildable);
        }
        else
        {
            uiManager.HidePrompt();
            uiManager.HideCraftingInfo();
        }
    }


    //void Update()
    //{
    //    Ray ray = new Ray(cursorTransform.position, cursorTransform.forward);
    //    RaycastHit hit;

    //    // Thêm bán kính "tương tác" → giúp dễ trúng object nhỏ
    //    float sphereRadius = 0.3f;

    //    if (Physics.SphereCast(ray, sphereRadius, out hit, interactionDistance, interactableLayer))
    //    {
    //        var interactable = hit.transform.GetComponent<IInteractable>();
    //        var info = hit.transform.GetComponent<IInteractableInfo>();

    //        if (interactable != null && info != null)
    //        {
    //            if (currentInteractable != interactable)
    //            {
    //                currentInteractable = interactable;
    //                uiManager.ShowPrompt(info);
    //            }
    //            return;
    //        }
    //        if (info != null && info.GetInteractionType() == InteractionType.Placeable)
    //        {
    //            var buildable = hit.transform.GetComponent<BuildableObject>();
    //            if (buildable != null)
    //            {
    //                // Gọi hiển thị crafting UI qua PlayerUIManager
    //                uiManager.ShowCraftingInfo(buildable.GetBlueprint(), buildable);

    //                // Nếu trước đó có prompt UI thì ẩn đi
    //                currentInteractable = null;
    //                uiManager.HidePrompt();
    //                //uiManager.HideCraftingInfo();
    //            }
    //            return;
    //        }
    //    }

    //    //// Không trúng gì
    //    //currentInteractable = null;
    //    //uiManager.HidePrompt();



    //    currentInteractable = null;
    //    uiManager.HidePrompt();
    //    uiManager.HideCraftingInfo();
    //}
}
