using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private UIScriptsManager uiScriptsManager;
    private PlayerDataUI playerDataUI;
    private PlayerMovement playerMovement;

    [SerializeField] private int playerNumber = 0;
    public int PlayerNumber { get { return playerNumber; } }

    [SerializeField] private int money = 1500;
    public int Money
    {
        get
        {
            return money;
        }
        set
        {
            if (debt < 0 && money <= 0)
            {
                Debt += value;
                return;
            }

            playerDataUI.UpdateMoneyText(value, money, false);

            if (value < 0)
            {
                money = 0;
                Debt = value;
            }
            else
            {
                money = value;
            }
        }
    }

    [SerializeField] private int debt = 0;

    public int Debt
    {
        get
        {
            return debt;
        }
        set
        {
            if (inDebtedTo != null && debt < 0)
            {
                int debtMoneyToGive = value < 0 ? debt - value : debt;

                inDebtedTo.Money += Mathf.Abs(debtMoneyToGive);
            }

            if (value >= 0)
            {
                debt = 0;
                inDebtedTo = null;
                Money = value;
            }

            playerDataUI.UpdateMoneyText(value, debt, true);
            debt = value;
        }
    }

    [SerializeField] private PlayerData inDebtedTo;
    public PlayerData InDebtedTo { get { return inDebtedTo; } }

    [SerializeField] private List<PropertyCard> propertyCards;
    public List<PropertyCard> PropertyCards { get { return propertyCards; } }

    [SerializeField] private List<PropertyCardSet> propertyCardSets;
    public List<PropertyCardSet> PropertyCardSets { get { return propertyCardSets; } }

    private BoardSpace currentBoardSpace;

    public delegate void Lost();
    public Lost OnLost;

    public bool DidLose { get; set; } = false;
    private bool inJail = false;
    public bool InJail { get { return inJail; } }

    private PlayerButtonUI buttonUI;

    // Start is called before the first frame update
    void Start()
    {
        playerDataUI = GameObject.FindGameObjectWithTag("PlayerPanels").transform.GetChild(playerNumber).GetComponent<PlayerDataUI>();
        playerMovement = GetComponent<PlayerMovement>();
        uiScriptsManager = FindObjectOfType<UIScriptsManager>();
        buttonUI = FindObjectOfType<PlayerButtonUI>();
    }

    public void AddPropertyCard(PropertyCard propertyCard)
    {
        propertyCard.PlayerOwningThis = this;
        propertyCards.Add(propertyCard);
        playerDataUI.UpdateProperties(propertyCard, true);

        CheckGotCardSet(propertyCard);
    }

    private void CheckGotCardSet(PropertyCard propertyCard)
    {
        PropertyCardSet propertyCardSet = propertyCard.MyCardSet;

        int cardsInSetCount = propertyCardSet.PropertyCardsInSet.Length;
        PropertyCardSet.ColorOfSet propertyCardSetColor = propertyCardSet.PropertySetColor;
        int owningCardsOfSetCount = 0;

        foreach (PropertyCard card in propertyCards)
        {
            if (card.MyCardSet.PropertySetColor == propertyCardSetColor)
            {
                owningCardsOfSetCount++;
            }
        }

        if (owningCardsOfSetCount == cardsInSetCount)
        {
            propertyCardSet.PlayerOwningThis = this;
            propertyCardSets.Add(propertyCardSet);
        }
    }

    public void RemovePropertyCard(PropertyCard propertyCard)
    {
        propertyCard.PlayerOwningThis = null;
        propertyCards.Remove(propertyCard);
        playerDataUI.UpdateProperties(propertyCard, false);

        if (propertyCardSets.Contains(propertyCard.MyCardSet))
        {
            propertyCardSets.Remove(propertyCard.MyCardSet);
            ResetCardPropertiesUpgrade(propertyCard.MyCardSet);
        }
    }

    public void ResetCardPropertiesUpgrade(PropertyCardSet propertyCardSet)
    {
        int allMoneyGot = 0;
        int removedUpgrades = 0;

        foreach (PropertyCard card in propertyCardSet.PropertyCardsInSet)
        {
            while (card.UpgradeLevel > 0)
            {
                allMoneyGot += Mathf.RoundToInt((float)card.UpgradePrice / 2);

                card.UpgradeLevel--;
                removedUpgrades++;
            }
        }

        if (allMoneyGot > 0)
        {
            Money += allMoneyGot;
        }
    }

    public void CheckCurrentSpace(BoardSpace boardSpace)
    {
        currentBoardSpace = boardSpace;

        switch (currentBoardSpace.BoardSpaceType)
        {
            case BoardSpace.BoardSpaceTypes.SAFE:
                playerMovement.OnDoneMoving();
                break;
            case BoardSpace.BoardSpaceTypes.START:
                playerMovement.OnDoneMoving();
                break;
            case BoardSpace.BoardSpaceTypes.PROPERTY:
                if (boardSpace.PropertyCardOnSpace.PlayerOwningThis != null)
                {
                    if (!boardSpace.PropertyCardOnSpace.Sold && boardSpace.PropertyCardOnSpace.PlayerOwningThis != this)
                    {
                        playerMovement.PlaceOnTourRoute(boardSpace.PropertyCardOnSpace.MyCardSet.TourRoute);
                    }
                    else
                    {
                        playerMovement.OnDoneMoving();
                    }                    
                }
                else
                {
                    uiScriptsManager.BuyPropertyUIScript.ShowBuyPanel(boardSpace.PropertyCardOnSpace, this);
                }
                break;
            case BoardSpace.BoardSpaceTypes.TRAIN:
                playerMovement.TeleportToCurrentGivenSpace();
                break;
            case BoardSpace.BoardSpaceTypes.JAILVISIT:
                playerMovement.TeleportToCurrentGivenSpace();
                break;
            case BoardSpace.BoardSpaceTypes.LUCKY:
                //playerMovement.SpinWheel(true);
                playerMovement.OnDoneMoving();
                break;
            case BoardSpace.BoardSpaceTypes.GOTOJAIL:
                playerMovement.TeleportToCurrentGivenSpace();

                inJail = true;
                break;
        }
    }

    public void GiveMoneyToOtherPlayer(int moneyLoss, PlayerData playerOwningProperty)
    {
        if (playerOwningProperty == this)
        {
            return;
        }

        bool inDebted = money - moneyLoss < 0;

        if (inDebted)
        {
            inDebtedTo = playerOwningProperty;
        }

        Money -= moneyLoss;

        if (inDebted)
        {
            moneyLoss -= Mathf.Abs(debt);
        }

        if (moneyLoss > 0)
        {
            StartCoroutine(DelayGivingMoney(playerOwningProperty, moneyLoss));
        }        
    }

    private IEnumerator DelayGivingMoney(PlayerData playerOwningProperty, int moneyGain)
    {
        yield return new WaitForSeconds(1);

        playerOwningProperty.Money += moneyGain;
    }

    public void ResumeAfterBuying()
    {
        playerMovement.OnDoneMoving();
    }

    public void CheckLost()
    {
        if (debt < 0)
        {
            DidLose = true;

            List<PropertyCard> cardsToRemove = new List<PropertyCard>();

            foreach (PropertyCard card in propertyCards)
            {
                cardsToRemove.Add(card);

                if (inDebtedTo)
                {
                    inDebtedTo.AddPropertyCard(card);
                }
                else
                {
                    card.PlayerOwningThis = null;
                    card.Sold = false;
                }
            }

            foreach (PropertyCard card in cardsToRemove)
            {
                RemovePropertyCard(card);
            }

            OnLost();
        }
    }

    public void GetOutOfJail(bool pay)
    {
        inJail = false;

        if (pay)
        {
            Money -= 50;
        }        
    }

    public void CheckLuckyNumber(int num)
    {
        switch (num)
        {
            case 1:
                //Jail
                //playerMovement.TeleportToGivenSpace();
                break;
            case 2:
                buttonUI.ShowSpin();
                break;
            case 3:

                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
        }
    }
}
