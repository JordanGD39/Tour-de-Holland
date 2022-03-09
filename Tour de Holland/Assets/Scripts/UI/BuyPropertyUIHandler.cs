using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyPropertyUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject objectHolder;
    [SerializeField] private Image propertyImage;
    private PlayerData playerData;
    private PropertyCard currentPropertyCard;

    private void Start()
    {
        objectHolder.SetActive(false);
    }

    public void ShowBuyPanel(PropertyCard propertyCard, PlayerData aPlayerData)
    {
        playerData = aPlayerData;
        currentPropertyCard = propertyCard;
        propertyImage.sprite = currentPropertyCard.MySprite;
        objectHolder.SetActive(true);
    }

    public void BuyProperty()
    {
        if (playerData.Money < currentPropertyCard.BuyPrice)
        {
            return;
        }

        playerData.Money -= currentPropertyCard.BuyPrice;
        objectHolder.SetActive(false);
        playerData.AddPropertyCard(currentPropertyCard);
        playerData.ResumeAfterBuying();
    }

    public void RejectProperty()
    {
        objectHolder.SetActive(false);
        playerData.ResumeAfterBuying();
    }
}
