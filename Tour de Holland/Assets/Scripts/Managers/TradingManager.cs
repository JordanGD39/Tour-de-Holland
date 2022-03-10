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
    [SerializeField] private ManageUI managePanel;

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
    [SerializeField] private float cooldownTime = 0.2f;
    [SerializeField] private float speedUpCooldownTime = 2;
    [SerializeField] private Image currentPlayerIcon;
    [SerializeField] private Image otherPlayerIcon;

    private int currentPlayerOfferedMoney = 0;
    private int otherPlayerWantedMoney = 0;
    private int totalOfferedValue = 0;
    private int totalWantedValue = 0;

    private List<PropertyCard> offeredPropertyCards = new List<PropertyCard>();
    private List<PropertyCard> wantedPropertyCards = new List<PropertyCard>();

    private List<Image> offeredPropertyCardsSpawned = new List<Image>();
    private List<Image> wantedPropertyCardsSpawned = new List<Image>();

    [SerializeField] private float cardOfferOffset = 10;
    private bool addMoney = false;
    private bool removeMoney = false;
    private bool currentPlayerChangingMoney = false;

    private float timer = 0;
    private float holdTimer = 0;
    private bool cooldown = false;
    private float actualCooldownTime = 0;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        centerCardImage.gameObject.SetActive(false);
        CloseTrading();
        actualCooldownTime = cooldownTime;
    }

    private void Update()
    {
        if (addMoney || removeMoney)
        {
            holdTimer += Time.deltaTime;
        }

        if (cooldown)
        {
            timer += Time.deltaTime;

            if (timer > actualCooldownTime)
            {
                cooldown = false;
                timer = 0;
            }

            return;
        }

        if (holdTimer > speedUpCooldownTime)
        {
            actualCooldownTime /= 2;
        }

        if (addMoney)
        {
            cooldown = true;            
            AddOfferMoney(currentPlayerChangingMoney);
        }

        if (removeMoney)
        {
            cooldown = true;
            RemoveOfferMoney(currentPlayerChangingMoney);
        }
    }

    public void StartAddingOfferMoney(bool currentPlayer)
    {
        actualCooldownTime = cooldownTime;
        addMoney = true;
        currentPlayerChangingMoney = currentPlayer;
    }

    public void StartRemovingOfferMoney(bool currentPlayer)
    {
        actualCooldownTime = cooldownTime;
        removeMoney = true;
        currentPlayerChangingMoney = currentPlayer;
    }

    public void StopAddingOfferMoney()
    {
        holdTimer = 0;
        addMoney = false;
    }

    public void StopRemovingOfferMoney()
    {
        holdTimer = 0;
        removeMoney = false;
    }

    private void CloseTrading()
    {
        tradingPanel.SetActive(false);
        managePanel.gameObject.SetActive(true);
        managePanel.HideOpenProperty();
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

    public void ShowPlayerTradeSelect()
    {
        tradingSelectPlayerPanel.SetActive(true);
        players.Clear();
        players.AddRange(playerManager.Players);

        players.RemoveAt(playerManager.Players[playerManager.CurrentTurn].playerData.PlayerNumber);

        for (int i = 0; i < tradingSelectPlayerPanel.transform.childCount; i++)
        {
            tradingSelectPlayerPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < players.Count; i++)
        {
            Transform iconTransform = tradingSelectPlayerPanel.transform.GetChild(i);

            iconTransform.gameObject.SetActive(true);

            if (players[i].playerData != this)
            {
                iconTransform.GetChild(0).GetComponent<Image>().sprite = players[i].playerData.PlayerIcon;
            }            
        }

        PlayerClassHolder playerClassHolder = playerManager.Players[playerManager.CurrentTurn];
        tradingPlayer = playerClassHolder.playerData;
        players.Remove(playerClassHolder);
    }

    public void ChooseTradePlayer(int index)
    {
        playerToTrade = players[index].playerData;
        currentPlayerIcon.sprite = playerManager.Players[playerManager.CurrentTurn].playerData.PlayerIcon;
        otherPlayerIcon.sprite = playerToTrade.PlayerIcon;
        otherPlayerIcon.sprite = playerToTrade.PlayerIcon;
        tradingSelectPlayerPanel.SetActive(false);
        tradingPanel.SetActive(true);
        currentPlayerMoney.text = "€" + tradingPlayer.Money.ToString();
        otherPlayerMoney.text = "€" + playerToTrade.Money.ToString();
        currentPlayerAddedMoney.text = "€0"; 
        otherPlayerAddedMoney.text = "€0";
        totalOfferedMoney.text = "€0";
        totalWantedMoney.text = "€0";
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
                currentPlayerAddedMoney.text = "€" + currentPlayerOfferedMoney.ToString();
                totalOfferedValue++;
                totalOfferedMoney.text = "€" + totalOfferedValue.ToString();
            }
        }
        else
        {
            if (otherPlayerWantedMoney < playerToTrade.Money)
            {
                otherPlayerWantedMoney++;
                otherPlayerAddedMoney.text = "€" + otherPlayerWantedMoney.ToString();
                totalWantedValue++;
                totalWantedMoney.text = "€" + totalWantedValue.ToString();
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
                currentPlayerAddedMoney.text = "€" + currentPlayerOfferedMoney.ToString();
                totalOfferedValue--;
                totalOfferedMoney.text = "€" + totalOfferedValue.ToString();
            }
        }
        else
        {
            if (otherPlayerWantedMoney > 0)
            {
                otherPlayerWantedMoney--;
                totalWantedValue--;
                otherPlayerAddedMoney.text = "€" + otherPlayerWantedMoney.ToString();
                totalWantedMoney.text = "€" + totalWantedValue.ToString();
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

            Vector3 cardPos = Vector3.zero;
            cardPos.x = cardOfferOffset * offeredPropertyCardsSpawned.Count;

            rectTransform.localPosition = cardPos;
            Image image = myNewPropertyCard.GetComponent<Image>();
            image.sprite = card.MySprite;

            offeredPropertyCardsSpawned.Add(image);
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
            UpdateAllPostionsOfShownCards(offeredPropertyCardsSpawned, false);
        }

        totalOfferedMoney.text = "€" + totalOfferedValue.ToString();
    }


    public void WantPropertyCard(string propertyCardID)
    {
        PropertyCard card = AddOrRemovePropertyToAList(wantedPropertyCards, GetPropertyCardOfID(propertyCardID));

        if (wantedPropertyCards.Contains(card))
        {
            var myNewPropertyCard = Instantiate(cardPrefab, placeWantedCardPrefab);
            totalWantedValue += card.BuyPrice;

            RectTransform rectTransform = myNewPropertyCard.GetComponent<RectTransform>();
            Vector3 cardPos = Vector3.zero;
            cardPos.x = -cardOfferOffset * wantedPropertyCardsSpawned.Count;

            rectTransform.localPosition = cardPos;
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

            UpdateAllPostionsOfShownCards(wantedPropertyCardsSpawned, true);

            totalWantedValue -= card.BuyPrice;
        }

        totalWantedMoney.text = "€" + totalWantedValue.ToString();
    }

    private void UpdateAllPostionsOfShownCards(List<Image> cards, bool inverted)
    {
        float offset = inverted ? -cardOfferOffset : cardOfferOffset;

        for (int i = 0; i < cards.Count; i++)
        {
            Image card = cards[i];

            Vector3 cardPos = Vector3.zero;
            cardPos.x = offset * i;

            card.rectTransform.localPosition = cardPos;
        }
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

    public void ConfirmTrade()
    {
        foreach (PropertyCard card in offeredPropertyCards)
        {
            tradingPlayer.RemovePropertyCard(card);
            playerToTrade.AddPropertyCard(card);
        }

        foreach (PropertyCard card in wantedPropertyCards)
        {
            playerToTrade.RemovePropertyCard(card);
            tradingPlayer.AddPropertyCard(card);
        }

        if (currentPlayerOfferedMoney > 0)
        {
            tradingPlayer.GiveMoneyToOtherPlayer(currentPlayerOfferedMoney, playerToTrade);
        }
        
        if(otherPlayerWantedMoney > 0)
        {
            playerToTrade.GiveMoneyToOtherPlayer(otherPlayerWantedMoney, tradingPlayer);
        }

        CloseTrading();
    }
}

