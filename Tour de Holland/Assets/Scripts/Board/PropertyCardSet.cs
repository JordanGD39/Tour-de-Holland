using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyCardSet : MonoBehaviour
{
    public enum ColorOfSet { PURPLE, BLUE, RED, GREEN}
    [SerializeField] private ColorOfSet propertySetColor = ColorOfSet.PURPLE;
    public ColorOfSet PropertySetColor { get { return propertySetColor; } }
    [SerializeField] private PropertyCard[] propertyCards;
    [SerializeField] private int[] shopLocations = { 1, 2, 5 };
    public int[] ShopLocations { get { return shopLocations; } }
    [SerializeField] private int upgradeLevel = 0;
    public int UpgradeLevel { get { return upgradeLevel; } set { upgradeLevel = value; } }

    private void Start()
    {
        for (int i = 0; i < propertyCards.Length; i++)
        {
            PropertyCard card = propertyCards[i];
            card.MyCardSet = this;
            card.PropertySetIndex = i;
            card.Owned = false;
        }
    }
}
