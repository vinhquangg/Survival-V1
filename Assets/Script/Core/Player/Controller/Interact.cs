using UnityEngine;

public class Interact : MonoBehaviour
{
    public GameObject player; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (player != null)
            {
                player.SetActive(false); 
            }
        }
    }
}
