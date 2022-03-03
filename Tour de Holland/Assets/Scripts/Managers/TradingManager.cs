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
    [SerializeField] private Text totalOfferedMoney;
    [SerializeField] private Text totalWantedMoney;

    [SerializeField] private Button[] purpleButtons;
    [SerializeField] private Button[] blueButtons;
    [SerializeField] private Button[] redButtons;
    [SerializeField] private Button[] greenButtons;

    [SerializeField] private PropertyCardSet[] allPropertyCardSet;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform placeOfferCardPrefab;
    [SerializeField] private Transform placeWantedCardPrefab;
    [SerializeField] private Image centerCardImage;

    private int currentPlayerOfferedMoney = 0;
    private int otherPlayerWantedMoney = 0;
    private int totalOfferedValue = 0;
    private int totalWantedValue = 0;

    private List<PropertyCard> offeredPropertyCards = new List<PropertyCard>();
    private List<PropertyCard> wantedPropertyCards = new List<PropertyCard>();

    private List<Image> offeredPropertyCardsSpawned = new List<Image>();
    private List<Image> wantedPropertyCardsSpawned = new List<Image>();

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        centerCardImage.gameObject.SetActive(false);
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

                centerCardImage.gameObject.SetActive(false);

                foreach (Image item in offeredPropertyCardsSpawned)
                {
                    Destroy(item.gameObject);
                }

                foreach (Image item in wantedPropertyCardsSpawned)
                {
                    Destroy(item.gameObject);
                }

                offeredPropertyCards.Clear();
                wantedPropertyCards.Clear();
                offeredPropertyCardsSpawned.Clear();
                wantedPropertyCardsSpawned.Clear();
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
        totalOfferedMoney.text = "$0";
        totalWantedMoney.text = "$0";
        currentPlayerOfferedMoney = 0;
        otherPlayerWantedMoney = 0;
        totalOfferedValue = 0;
        totalWantedValue = 0;

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

            if (card.MyCardSet.PropertySetColor == colorSet)
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
                totalOfferedValue++;
                totalOfferedMoney.text = totalOfferedValue.ToString();
            }
        }
        else
        {
            if (otherPlayerWantedMoney < playerToTrade.Money)
            {
                otherPlayerWantedMoney++;
                otherPlayerAddedMoney.text = "$" + otherPlayerWantedMoney.ToString();
                totalWantedValue++;
                totalWantedMoney.text = totalWantedValue.ToString();
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
                totalOfferedValue--;
                totalOfferedMoney.text = totalOfferedValue.ToString();
            }
        }
        else
        {
            if (otherPlayerWantedMoney > 0)
            {
                otherPlayerWantedMoney--;
                totalWantedValue--;
                otherPlayerAddedMoney.text = "$" + otherPlayerWantedMoney.ToString();
                totalWantedMoney.text = totalWantedValue.ToString();
            }
        }
    }

    public void OfferPropertyCard(string propertyCardID)
    {
        PropertyCard card = AddOrRemovePropertyToAList(offeredPropertyCards, GetPropertyCardOfID(propertyCardID));

        if (offeredPropertyCards.Contains(card))
        {
            var myNewPropertyCard = Instantiate(cardPrefab, placeOfferCardPrefab);
            totalOfferedValue += card.BuyPrice;

            RectTransform rectTransform = myNewPropertyCard.GetComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            Image image = myNewPropertyCard.GetComponent<Image>();
            image.sprite = card.MySprite;

            offeredPropertyCardsSpawned.Add(image);
            totalOfferedMoney.text = totalOfferedValue.ToString();
        }
        else
        {
            foreach (Image item in offeredPropertyCardsSpawned)
            {
                if (item.sprite == card.MySprite)
                {
                    offeredPropertyCardsSpawned.Remove(item);
                    Destroy(item.gameObject);
                    break;
                }
            }

            totalOfferedValue -= card.BuyPrice;
        }
        totalOfferedMoney.text = totalOfferedValue.ToString();
    }


    public void WantPropertyCard(string propertyCardID)
    {
        PropertyCard card = AddOrRemovePropertyToAList(wantedPropertyCards, GetPropertyCardOfID(propertyCardID));

        if (wantedPropertyCards.Contains(card))
        {
            var myNewPropertyCard = Instantiate(cardPrefab, placeWantedCardPrefab);
            totalWantedValue += card.BuyPrice;

            RectTransform rectTransform = myNewPropertyCard.GetComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            Image image = myNewPropertyCard.GetComponent<Image>();
            image.sprite = card.MySprite;        
            wantedPropertyCardsSpawned.Add(image);
        }
        else
        {
            foreach (Image item in wantedPropertyCardsSpawned)
            {
                if (item.sprite == card.MySprite)
                {
                    wantedPropertyCardsSpawned.Remove(item);                    
                    Destroy(item.gameObject);
                    break;
                }
            }

            totalWantedValue -= card.BuyPrice;
        }

        totalWantedMoney.text = totalWantedValue.ToString();
    }

    private PropertyCard AddOrRemovePropertyToAList(List<PropertyCard> propertyCards, PropertyCard card)
    {
        centerCardImage.gameObject.SetActive(true);
        centerCardImage.sprite = card.MySprite;

        if (!propertyCards.Contains(card))
        {
            propertyCards.Add(card);
        }
        else
        {
            propertyCards.Remove(card);
        }

        return card;
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

