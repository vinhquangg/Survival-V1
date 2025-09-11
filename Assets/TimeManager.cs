using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static TimeManager Instance { get; private set; }

    public int dayInGame = 1;
    public TextMeshProUGUI dayUI;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    public void NextDay()
    {
        dayInGame+=1;
        dayUI.text = "Day: " + dayInGame;
        //Debug.Log("Day: " + dayInGame);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
