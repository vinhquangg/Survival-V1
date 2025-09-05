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

    public static SelectionManager Instance { get; private set; }

    void Awake()
    {
        Instance = this; 
    }
    void Start()
    {
        uiManager = FindObjectOfType<PlayerUIManager>();
        if (uiManager == null)
            Debug.LogError("Không tìm thấy PlayerUIManager trong scene!");
    }

    //void Update()
    //{
    //    Ray ray = new Ray(cursorTransform.position, cursorTransform.forward);
    //    RaycastHit hit;

    //    float sphereRadius = 0.3f;

    //    if (Physics.SphereCast(ray, sphereRadius, out hit, interactionDistance, interactableLayer, QueryTriggerInteraction.Ignore))
    //    {
    //        var interactable = hit.transform.GetComponent<IInteractable>();
    //        var info = hit.transform.GetComponent<IInteractableInfo>();

    //        if (interactable != null && info != null)
    //        {
    //            if (currentInteractable != interactable)
    //            {
    //                currentInteractable = interactable;

    //                switch (info.GetInteractionType())
    //                {
    //                    case InteractionType.Pickup:
    //                        uiManager.ShowPrompt(info);
    //                        uiManager.HideCraftingInfo();
    //                        break;

    //                    case InteractionType.Chop:
    //                        uiManager.ShowPrompt(info);
    //                        uiManager.HideCraftingInfo();
    //                        break;

    //                    case InteractionType.Placeable:
    //                        var buildable = hit.transform.GetComponent<BuildableObject>();
    //                        if (buildable != null)
    //                        {
    //                            if (currentBuildable != buildable)
    //                            {
    //                                if (currentBuildable != null)
    //                                    currentBuildable.OnMaterialChanged -= OnBuildableMaterialChanged;

    //                                currentBuildable = buildable;

    //                                if (!buildable.IsBuilt)
    //                                    buildable.OnMaterialChanged += OnBuildableMaterialChanged;

    //                            }

    //                            if (!buildable.IsBuilt)
    //                            {

    //                                uiManager.ShowCraftingInfo(buildable.GetBlueprint(), buildable);
    //                            }
    //                            else
    //                            {

    //                                uiManager.HideCraftingInfo();
    //                            }

    //                            if (buildable.IsBuilt)
    //                            {
    //                                var campfire = hit.transform.GetComponent<Campfire>();
    //                                if (campfire != null)
    //                                {
    //                                    currentInteractable = campfire;
    //                                    if (campfire.IsCooking)
    //                                    {
    //                                        uiManager.ShowCookingUI(
    //                                            campfire.CurrentCookingName,
    //                                            campfire.CurrentCookingIcon,
    //                                            campfire.CurrentCookingQty
    //                                        );
    //                                    }
    //                                    else
    //                                    {
    //                                        uiManager.ShowPrompt(campfire);
    //                                    }
    //                                    uiManager.HideCraftingInfo();
    //                                }
    //                                else
    //                                {
    //                                    var cookable = hit.transform.GetComponent<Cookable>();
    //                                    if (cookable != null)
    //                                    {
    //                                        currentInteractable = cookable;
    //                                        uiManager.ShowPrompt(cookable);
    //                                        uiManager.HideCraftingInfo();
    //                                    }
    //                                    else
    //                                    {
    //                                        uiManager.HidePrompt();
    //                                        uiManager.HideCraftingInfo();
    //                                    }
    //                                }
    //                            }


    //                        }

    //                        else
    //                        {
    //                            uiManager.HidePrompt();
    //                            uiManager.HideCraftingInfo();
    //                        }
    //                        break;

    //                    case InteractionType.Drink:
    //                        var lake = hit.transform.GetComponent<Lake>();
    //                        if (lake != null && lake.IsPlayerInRange())
    //                        {
    //                            uiManager.ShowPrompt(info);
    //                        }
    //                        else
    //                        {
    //                            uiManager.HidePrompt();
    //                        }
    //                        break;


    //                    default:
    //                        uiManager.HidePrompt();
    //                        uiManager.HideCraftingInfo();
    //                        break;
    //                }
    //            }
    //            return;
    //        }
    //    }

    //    currentInteractable = null;
    //    uiManager.HidePrompt();
    //    uiManager.HideCraftingInfo();
    //    if (currentBuildable != null)
    //    {
    //        currentBuildable.OnMaterialChanged -= OnBuildableMaterialChanged;
    //        currentBuildable = null;
    //    }

    //}


    void Update()
    {
        Ray ray = new Ray(cursorTransform.position, cursorTransform.forward);
        RaycastHit hit;

        float sphereRadius = 0.3f;

        if (Physics.SphereCast(ray, sphereRadius, out hit, interactionDistance, interactableLayer, QueryTriggerInteraction.Ignore))
        {
            var interactable = hit.transform.GetComponent<IInteractable>();
            var info = hit.transform.GetComponent<IInteractableInfo>();

            if (interactable != null && info != null)
            {
                if (currentInteractable != interactable)
                {
                    currentInteractable = interactable;

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
                                if (currentBuildable != buildable)
                                {
                                    if (currentBuildable != null)
                                        currentBuildable.OnMaterialChanged -= OnBuildableMaterialChanged;

                                    currentBuildable = buildable;

                                    if (!buildable.IsBuilt)
                                        buildable.OnMaterialChanged += OnBuildableMaterialChanged;

                                }

                                if (!buildable.IsBuilt)
                                {

                                    uiManager.ShowCraftingInfo(buildable.GetBlueprint(), buildable);
                                }
                                else
                                {

                                    uiManager.HideCraftingInfo();
                                }

                                if (buildable.IsBuilt)
                                {
                                    var campfire = hit.transform.GetComponent<Campfire>();
                                    if (campfire != null)
                                    {
                                        currentInteractable = campfire;
                                        if (campfire.IsCooking)
                                        {
                                            uiManager.ShowCookingUI(
                                                campfire.CurrentCookingName,
                                                campfire.CurrentCookingIcon,
                                                campfire.CurrentCookingQty
                                            );
                                        }
                                        else
                                        {
                                            uiManager.ShowPrompt(campfire);
                                        }
                                        uiManager.HideCraftingInfo();
                                    }
                                    else
                                    {
                                        var cookable = hit.transform.GetComponent<Cookable>();
                                        if (cookable != null)
                                        {
                                            currentInteractable = cookable;
                                            uiManager.ShowPrompt(cookable);
                                            uiManager.HideCraftingInfo();
                                        }
                                        else
                                        {
                                            uiManager.HidePrompt();
                                            uiManager.HideCraftingInfo();
                                        }
                                    }
                                }


                            }

                            else
                            {
                                uiManager.HidePrompt();
                                uiManager.HideCraftingInfo();
                            }
                            break;

                        case InteractionType.Drink:
                            var lake = hit.transform.GetComponent<Lake>();
                            if (lake != null && lake.IsPlayerInRange())
                            {
                                uiManager.ShowPrompt(info);
                            }
                            else
                            {
                                uiManager.HidePrompt();
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

        // --- CHỖ NÀY LÀ CHỖ QUAN TRỌNG ---
        if (currentInteractable is Lake lakeInteractable)
        {
            if (lakeInteractable.IsPlayerInRange())
            {
                // Nếu đang trong range mà chưa show prompt thì show lại
                var info = lakeInteractable.GetComponent<IInteractableInfo>();
                if (info != null)
                    uiManager.ShowPrompt(info);

                return; // KHÔNG hide prompt
            }
        }


        // Ngược lại (ra khỏi hồ hoặc không có interactable nào)
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
}
