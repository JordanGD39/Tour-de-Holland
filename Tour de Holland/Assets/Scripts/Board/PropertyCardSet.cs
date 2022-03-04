using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyCardSet : MonoBehaviour
{
    [SerializeField] private TourRouteManager tourRouteManager;
    public TourRouteManager TourRoute { get { return tourRouteManager; } }

    public enum ColorOfSet { PURPLE, BLUE, RED, GREEN}
    [SerializeField] private ColorOfSet propertySetColor = ColorOfSet.PURPLE;
    public ColorOfSet PropertySetColor { get { return propertySetColor; } }
    [SerializeField] private PropertyCard[] propertyCards;
    [SerializeField] public PropertyCard[] PropertyCardsInSet { get { return propertyCards; } }

    public PlayerData PlayerOwningThis { get; set; }

    private void Start()
    {
        for (int i = 0; i < propertyCards.Length; i++)
        {
            PropertyCard card = propertyCards[i];
            card.MyCardSet = this;
            card.PropertySetIndex = i;
            card.PlayerOwningThis = null;
            card.UpgradeLevel = 0;
            card.ShopLocations.Clear();
            card.ShopLocations.Add(1);
            card.ShopLocations.Add(2);
            card.ShopLocations.Add(5);
        }

        if (tourRouteManager != null)
        {
            tourRouteManager.CardSet = this;
        }        
    }
}
