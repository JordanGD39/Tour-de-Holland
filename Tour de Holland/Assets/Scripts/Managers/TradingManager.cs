using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradingManager : MonoBehaviour
{
    private PlayerManager playerManager;
    private List<PlayerClassHolder> players = new List<PlayerClassHolder>();

    private PlayerData tradingPlayer;
    private PlayerData playerToTrade;

    [SerializeField] private GameObject tradingPanel;
    [SerializeField] private GameObject tradingSelectPlayerPanel;

    [SerializeField] private Text currentPlayerMoney;
    [SerializeField] private Text otherPlayerMoney;
    [SerializeField] private Text currentPlayerAddedMoney;
    [SerializeField] private Text otherPlayerAddedMoney;

    private int currentPlayerOfferedMoney = 0;
    private int otherPlayerWantedMoney = 0;

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
        currentPlayerMoney.text = "$" + tradingPlayer.Money.ToString();
        otherPlayerMoney.text = "$" + playerToTrade.Money.ToString();
        
        otherPlayerAddedMoney.text = "$"; 
    }

    public void AddOfferMoney(bool currentPlayer)
    {
        if (currentPlayer)
        {
            if (currentPlayerOfferedMoney < tradingPlayer.Money)
            {
                currentPlayerOfferedMoney++;
                currentPlayerAddedMoney.text = "$" + currentPlayerOfferedMoney.ToString();
            }
        }
        else
        {
            if (otherPlayerWantedMoney < playerToTrade.Money)
            {
                otherPlayerWantedMoney++;
                otherPlayerAddedMoney.text = "$" + otherPlayerWantedMoney.ToString();
            }
        }
    }

    public void RemoveOfferMoney(bool currentPlayer)
    {
        if (currentPlayer)
        {
            if (currentPlayerOfferedMoney > 0)
            {
                currentPlayerOfferedMoney--;
                currentPlayerAddedMoney.text = "$" + currentPlayerOfferedMoney.ToString();
            }
        }
        else
        {
            if (otherPlayerWantedMoney > 0)
            {
                otherPlayerWantedMoney--;
                otherPlayerAddedMoney.text = "$" + otherPlayerWantedMoney.ToString();
            }
        }
    }
}

