using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyPropertyUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject objectHolder;
    [SerializeField] private Image propertyImage;
    [SerializeField] private Text propertyText;
    [SerializeField] private Text buyPriceText;
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
        buyPriceText.text = "Buy for €" + currentPropertyCard.BuyPrice + "?";
        propertyText.text = propertyCard.GetCardDataText();
        AudioManager.instance.Play("CardAppearSFX");
    }

    public void BuyProperty()
    {
        if (playerData.Money < currentPropertyCard.BuyPrice)
        {
            return;
        }

        AudioManager.instance.Play("TourBoughtSFX");

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
