using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractUI : MonoBehaviour
{

    public float interactionDistance;
    public GameObject interaction_Info;
    public LayerMask interactableLayer;
    TextMeshProUGUI interact_text;
    void Start()
    {
        interact_text = interaction_Info.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, interactableLayer))
        {
            var selectionTransform = hit.transform;

            var interactable = selectionTransform.GetComponent<HelicopterController>();
            if (interactable)
            {
                float distance = Vector3.Distance(Camera.main.transform.position, hit.point);
                if (distance <= interactionDistance)
                {
                    interaction_Info.SetActive(true);
                }
                else
                {
                    HideInteractionUI();
                }
            }
            else
            {
                HideInteractionUI();
            }
        }
        else
        {
            HideInteractionUI();
        }
    }



    void HideInteractionUI()
    {
        interaction_Info.SetActive(false);

    }



    //public GameObject interact;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    interact.SetActive(false);
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.CompareTag("Player"))
    //    {
    //        //gameObject.SetActive(true);
    //        interact.SetActive(true);
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        //gameObject.SetActive(false);
    //        interact.SetActive(false);
    //    }
    //}
}

