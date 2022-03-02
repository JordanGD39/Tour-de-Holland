using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private PlayerDataUI playerDataUI;

    [SerializeField] private int playerNumber = 0;
    public int PlayerNumber { get { return playerNumber; } }
    [SerializeField] private int money = 1500;
    public int Money { get { return money; } set { playerDataUI.UpdateMoneyText(value); money = value; } }
    [SerializeField] private List<PropertyCard> propertyCards;
    public List<PropertyCard> PropertyCards { get { return propertyCards; } }

    // Start is called before the first frame update
    void Start()
    {
        playerDataUI = GameObject.FindGameObjectWithTag("PlayerPanels").transform.GetChild(playerNumber).GetComponent<PlayerDataUI>();
    }

    public void AddPropertyCard(PropertyCard propertyCard)
    {
        propertyCards.Add(propertyCard);
        playerDataUI.UpdateProperties(propertyCard, true);
    }

    public void RemovePropertyCard(PropertyCard propertyCard)
    {
        propertyCards.Remove(propertyCard);
        playerDataUI.UpdateProperties(propertyCard, false);
    }
}
