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
    [SerializeField] private Button[] purpleButtons;
    [SerializeField] private Button[] blueButtons;
    [SerializeField] private Button[] redButtons;
    [SerializeField] private Button[] greenButtons;
    [SerializeField] private PropertyCardSet[] allPropertyCardSet;

    private int currentPlayerOfferedMoney = 0;
    private int otherPlayerWantedMoney = 0;
    private List<PropertyCard> offeredPropertyCards = new List<PropertyCard>();
    private List<PropertyCard> wantedPropertyCards = new List<PropertyCard>();

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
        currentPlayerAddedMoney.text = "$0"; 
        otherPlayerAddedMoney.text = "$0";
        currentPlayerOfferedMoney = 0;
        otherPlayerWantedMoney = 0;

        SetButtonsUninteractable(purpleButtons);
        SetButtonsUninteractable(blueButtons);
        SetButtonsUninteractable(redButtons);
        SetButtonsUninteractable(greenButtons);

        CheckPropertyCardsOfPlayer(tradingPlayer, true);
        CheckPropertyCardsOfPlayer(playerToTrade, false);
    }

    private void CheckPropertyCardsOfPlayer(PlayerData playerData, bool tradingPlayer)
    {
        List<PropertyCard> purpleCards = CheckPropertyCardsOfColor(playerData, PropertyCardSet.ColorOfSet.PURPLE);
        List<PropertyCard> blueCards = CheckPropertyCardsOfColor(playerData, PropertyCardSet.ColorOfSet.BLUE);
        List<PropertyCard> redCards = CheckPropertyCardsOfColor(playerData, PropertyCardSet.ColorOfSet.RED);
        List<PropertyCard> greenCards = CheckPropertyCardsOfColor(playerData, PropertyCardSet.ColorOfSet.GREEN);

        SetButtonsInteractable(purpleButtons, purpleCards, tradingPlayer);
        SetButtonsInteractable(blueButtons, blueCards, tradingPlayer);
        SetButtonsInteractable(redButtons, redCards, tradingPlayer);
        SetButtonsInteractable(greenButtons, greenCards, tradingPlayer);
    }

    private void SetButtonsUninteractable(Button[] buttons)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
    }

    private void SetButtonsInteractable(Button[] buttons, List<PropertyCard> propertyCards, bool tradingPlayer)
    {
        for (int i = 0; i < propertyCards.Count; i++)
        {
            buttons[propertyCards[i].PropertySetIndex + (tradingPlayer ? 0: 3)].interactable = true;
        }
    }

    private List<PropertyCard> CheckPropertyCardsOfColor(PlayerData playerData, PropertyCardSet.ColorOfSet colorSet)
    {
        List<PropertyCard> cards = new List<PropertyCard>();

        for (int i = 0; i < playerData.PropertyCards.Count; i++)
        {
            PropertyCard card = playerData.PropertyCards[i];

            if (card.MyCardSet.SetColor == colorSet)
            {
                cards.Add(card);
            }
        }

        return cards;
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

    public void OfferPropertyCard(string propertyCardID)
    {
        AddOrRemovePropertyToAList(offeredPropertyCards, GetPropertyCardOfID(propertyCardID));       
    }


    public void WantPropertyCard(string propertyCardID)
    {
        AddOrRemovePropertyToAList(wantedPropertyCards, GetPropertyCardOfID(propertyCardID));
    }

    private void AddOrRemovePropertyToAList(List<PropertyCard> propertyCards, PropertyCard card)
    {
        if (!propertyCards.Contains(card))
        {
            propertyCards.Add(card);
        }
        else
        {
            propertyCards.Remove(card);
        }
    }

    private PropertyCard GetPropertyCardOfID(string propertyCardID)
    {
        char colorOfSet = propertyCardID[0];

        PropertyCardSet.ColorOfSet color = PropertyCardSet.ColorOfSet.PURPLE;

        switch (colorOfSet)
        {
            case 'B':
                color = PropertyCardSet.ColorOfSet.BLUE;
                break;
            case 'R':
                color = PropertyCardSet.ColorOfSet.RED;
                break;
            case 'G':
                color = PropertyCardSet.ColorOfSet.GREEN;
                break;
        }

        PropertyCardSet cardSet = allPropertyCardSet[(int)color];

        char indexOfSet = propertyCardID[1];
        string s = indexOfSet.ToString();

        int.TryParse(s, out int i);

        return cardSet.PropertyCardsInSet[i];
    }
}

