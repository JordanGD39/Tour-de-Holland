using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageUI : MonoBehaviour
{
    private PlayerManager playerManager;

    [SerializeField] private Image showedPropertyCard;
    [SerializeField] private PropertyCardSet[] cardSets;
    [SerializeField] private GameObject managePropertyButtons;
    [SerializeField] private RectTransform sellTourTransform;
    [SerializeField] private RectTransform removeUpgradeButton;
    [SerializeField] private GameObject upgradeButton;
    //[SerializeField] private GameObject editShopsButton;
    [SerializeField] private RectTransform rebuyButton;
    [SerializeField] private Text rebuyPriceText;
    [SerializeField] private Text propertyCardText;
    [SerializeField] private Vector3 startingSellTourPos;
    [SerializeField] private Transform starsParent;

    private List<PropertyCard> allPropertyCards = new List<PropertyCard>();

    private PropertyCard currentCard;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        startingSellTourPos = sellTourTransform.localPosition;
        showedPropertyCard.gameObject.SetActive(false);
        managePropertyButtons.SetActive(false);
        upgradeButton.SetActive(false);
        //editShopsButton.SetActive(false);
        removeUpgradeButton.gameObject.SetActive(false);
        sellTourTransform.gameObject.SetActive(true);
        rebuyButton.gameObject.SetActive(false);
        rebuyPriceText.gameObject.SetActive(false);

        for (int i = 0; i < starsParent.childCount; i++)
        {
            starsParent.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (allPropertyCards.Count > 0)
        {
            return;
        }

        playerManager = FindObjectOfType<PlayerManager>();

        foreach (PropertyCardSet item in cardSets)
        {
            allPropertyCards.AddRange(item.PropertyCardsInSet);
        }
    }

    public void HideOpenProperty()
    {
        showedPropertyCard.gameObject.SetActive(false);
        managePropertyButtons.SetActive(false);
    }

    public void ShowPropertyImage(int index)
    {
        PropertyCard card = allPropertyCards[index];
        currentCard = card;

        showedPropertyCard.gameObject.SetActive(true);
        showedPropertyCard.sprite = card.MySprite;
        propertyCardText.text = card.GetCardDataText();

        for (int i = 0; i < starsParent.childCount; i++)
        {
            starsParent.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < card.UpgradeLevel - 1; i++)
        {
            starsParent.transform.GetChild(i).gameObject.SetActive(true);
        }

        CheckButtons();
    }

    private void CheckButtons()
    {
        PlayerData playerData = playerManager.Players[playerManager.CurrentTurn].playerData;

        PropertyCard card = currentCard;

        if (card.PlayerOwningThis == playerData)
        {
            managePropertyButtons.SetActive(true);

            if (!card.Sold && playerData.PropertyCardSets.Contains(card.MyCardSet))
            {
                //editShopsButton.SetActive(true);
                upgradeButton.SetActive(currentCard.UpgradeLevel < currentCard.TourFee.Length - 1);

                if (card.UpgradeLevel > 0)
                {
                    sellTourTransform.gameObject.SetActive(false);
                    removeUpgradeButton.gameObject.SetActive(true);
                }
                else
                {
                    sellTourTransform.gameObject.SetActive(!card.Sold);
                    rebuyButton.gameObject.SetActive(card.Sold);
                    sellTourTransform.localPosition = removeUpgradeButton.localPosition;
                    removeUpgradeButton.gameObject.SetActive(false);
                }
            }
            else
            {
                ShowOnlySellOrRebuy();
            }
        }
        else
        {
            managePropertyButtons.SetActive(false);
        }
    }

    private void ShowOnlySellOrRebuy()
    {
        upgradeButton.SetActive(false);
        removeUpgradeButton.gameObject.SetActive(false);
        //editShopsButton.SetActive(false);
        sellTourTransform.gameObject.SetActive(!currentCard.Sold);
        sellTourTransform.localPosition = startingSellTourPos;
        rebuyButton.gameObject.SetActive(currentCard.Sold);
        rebuyPriceText.gameObject.SetActive(currentCard.Sold);
        rebuyPriceText.text = "Rebuy price: €" + currentCard.RebuyPrice.ToString();
    }

    public void SellTour()
    {
        if (currentCard.Sold)
        {
            return;
        }

        currentCard.Sold = true;
        playerManager.Players[playerManager.CurrentTurn].playerData.Money += currentCard.SellPrice;

        ShowOnlySellOrRebuy();
    }

    public void RebuyTour()
    {
        PlayerData playerData = playerManager.Players[playerManager.CurrentTurn].playerData;

        if (!currentCard.Sold || playerData.Money < currentCard.RebuyPrice)
        {
            return;
        }

        currentCard.Sold = false;
        playerData.Money -= currentCard.RebuyPrice;

        CheckButtons();
    }

    public void UpgradePropertyCard()
    {
        PlayerData playerData = playerManager.Players[playerManager.CurrentTurn].playerData;

        if (playerData.Money >= currentCard.UpgradePrice && currentCard.UpgradeLevel < currentCard.TourFee.Length - 1)
        {
            currentCard.UpgradeLevel++;

            playerData.Money -= currentCard.UpgradePrice;

            starsParent.transform.GetChild(currentCard.UpgradeLevel - 1).gameObject.SetActive(true);

            SetShopLocations(currentCard.UpgradeLevel, false);

            CheckButtons();
        }
    }

    private void SetShopLocations(int level, bool removing)
    {
        switch (level)
        {
            case 1:
                if (!removing)
                {
                    return;
                }

                currentCard.ShopLocations.Clear();
                currentCard.ShopLocations.Add(1);
                currentCard.ShopLocations.Add(3);
                currentCard.ShopLocations.Add(5);
                break;
            case 2:
                currentCard.ShopLocations.Add(2);
                break;
            case 5:
                currentCard.ShopLocations.Add(4);
                break;
        }
    }

    public void RemovePropertyCard()
    {
        PlayerData playerData = playerManager.Players[playerManager.CurrentTurn].playerData;

        if (currentCard.UpgradeLevel > 0)
        {
            starsParent.transform.GetChild(currentCard.UpgradeLevel - 1).gameObject.SetActive(false);

            currentCard.UpgradeLevel--;

            playerData.Money += Mathf.RoundToInt((float)currentCard.UpgradePrice / 2);
            SetShopLocations(currentCard.UpgradeLevel, true);

            CheckButtons();
        }
    }
}
