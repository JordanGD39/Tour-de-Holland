using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageUI : MonoBehaviour
{
    [SerializeField] private Image showedPropertyCard;
    [SerializeField] private PropertyCardSet[] cardSets;

    private List<PropertyCard> allPropertyCards = new List<PropertyCard>();

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        showedPropertyCard.gameObject.SetActive(false);

        if (allPropertyCards.Count > 0)
        {
            return;
        }

        foreach (PropertyCardSet item in cardSets)
        {
            allPropertyCards.AddRange(item.PropertyCardsInSet);
        }
    }

    public void ShowPropertyImage(int index)
    {
        showedPropertyCard.gameObject.SetActive(true);
        showedPropertyCard.sprite = allPropertyCards[index].MySprite;
    }
}
