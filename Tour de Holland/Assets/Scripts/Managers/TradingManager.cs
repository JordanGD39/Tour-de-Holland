using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingManager : MonoBehaviour
{
    private PlayerManager playerManager;
    private List<PlayerClassHolder> players = new List<PlayerClassHolder>();

    private PlayerData tradingPlayer;
    private PlayerData playerToTrade;

    [SerializeField] private GameObject tradingPanel;
    [SerializeField] private GameObject tradingSelectPlayerPanel;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!tradingPanel.activeSelf && !tradingSelectPlayerPanel.activeSelf)
            {
                ShowPlayerTradeSelect();
            }
            else
            {
                tradingPanel.SetActive(false);
                tradingSelectPlayerPanel.SetActive(false);
            }            
        }
        
    }

    private void ShowPlayerTradeSelect()
    {
        tradingSelectPlayerPanel.SetActive(true);
        players.Clear();
        players.AddRange(playerManager.Players);

        PlayerClassHolder playerClassHolder = playerManager.Players[playerManager.CurrentTurn];
        tradingPlayer = playerClassHolder.playerData;
        players.Remove(playerClassHolder);
    }

    public void ChooseTradePlayer(int index)
    {
        playerToTrade = players[index].playerData;
        tradingSelectPlayerPanel.SetActive(false);
        tradingPanel.SetActive(true);
    }
}
