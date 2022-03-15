using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScriptsManager : MonoBehaviour
{
    [SerializeField] private BuyPropertyUIHandler buyPropertyUIHandler;
    public BuyPropertyUIHandler BuyPropertyUIScript { get { return buyPropertyUIHandler; } }

    [SerializeField] private GameObject luckyPanel;
    public GameObject LuckyPanel { get { return luckyPanel; } }
    [SerializeField] private GameObject managePanel;


    private void Start()
    {
        managePanel.SetActive(false);    
        luckyPanel.SetActive(false);    
    }

    public void ShowLuckyPanel()
    {
        luckyPanel.SetActive(false);
        luckyPanel.SetActive(true);
    }
}
