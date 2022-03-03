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
    public int Money { get { return money; } set { playerDataUI.UpdateMoneyText(value, money); money = value; } }
    [SerializeField] private List<PropertyCard> propertyCards;
    public List<PropertyCard> PropertyCards { get { return propertyCards; } }

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
    }

    public void RemovePropertyCard(PropertyCard propertyCard)
    {
        propertyCard.PlayerOwningThis = null;
        propertyCards.Remove(propertyCard);
        playerDataUI.UpdateProperties(propertyCard, false);
    }

    public void CheckCurrentSpace(BoardSpace boardSpace)
    {
        currentBoardSpace = boardSpace;

        switch (currentBoardSpace.BoardSpaceType)
        {
            case BoardSpace.BoardSpaceTypes.SAFE:
                playerMovement.OnEndTurn();
                break;
            case BoardSpace.BoardSpaceTypes.START:
                playerMovement.OnEndTurn();
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
                        playerMovement.OnEndTurn();
                    }                    
                }
                else
                {
                    uiScriptsManager.BuyPropertyUIScript.ShowBuyPanel(boardSpace.PropertyCardOnSpace, this);
                }
                break;
            case BoardSpace.BoardSpaceTypes.TRAIN:
                playerMovement.OnEndTurn();
                break;
            case BoardSpace.BoardSpaceTypes.JAILVISIT:
                playerMovement.OnEndTurn();
                break;
            case BoardSpace.BoardSpaceTypes.LUCKY:
                playerMovement.OnEndTurn();
                break;
            case BoardSpace.BoardSpaceTypes.GOTOJAIL:
                playerMovement.OnEndTurn();
                break;
        }
    }

    public void ResumeAfterBuying()
    {
        playerMovement.OnEndTurn();
    }
}
