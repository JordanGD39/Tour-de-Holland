using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private UIScriptsManager uiScriptsManager;
    private PlayerDataUI playerDataUI;
    private PlayerMovement playerMovement;
    private int playerModel;
    public int PlayerModel { get { return playerModel; } set { playerModel = value; }  }

    [SerializeField] private int playerNumber = 0;
    public int PlayerNumber { get { return playerNumber; } }
    [SerializeField] private int money = 1500;
    public int Money { get { return money; } set { playerDataUI.UpdateMoneyText(value, money); money = value; } }
    [SerializeField] private List<PropertyCard> propertyCards;
    public List<PropertyCard> PropertyCards { get { return propertyCards; } }

    [SerializeField] private List<PropertyCardSet> propertyCardSets;
    public List<PropertyCardSet> PropertyCardSets { get { return propertyCardSets; } }

    private BoardSpace currentBoardSpace;

    // Start is called before the first frame update
    void Start()
    {
        playerDataUI = GameObject.FindGameObjectWithTag("PlayerPanels").transform.GetChild(playerNumber).GetComponent<PlayerDataUI>();
        playerMovement = GetComponent<PlayerMovement>();
        uiScriptsManager = FindObjectOfType<UIScriptsManager>();
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
                    if (boardSpace.PropertyCardOnSpace.PlayerOwningThis != this)
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
                playerMovement.OnDoneMoving();
                break;
            case BoardSpace.BoardSpaceTypes.JAILVISIT:
                playerMovement.OnDoneMoving();
                break;
            case BoardSpace.BoardSpaceTypes.LUCKY:
                playerMovement.OnDoneMoving();
                break;
            case BoardSpace.BoardSpaceTypes.GOTOJAIL:
                playerMovement.OnDoneMoving();
                break;
        }
    }

    public void GiveMoneyToOtherPlayer(int moneyLoss, PlayerData playerOwningProperty)
    {
        Money -= moneyLoss;

        StartCoroutine(DelayGivingMoney(playerOwningProperty, moneyLoss));
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
}
