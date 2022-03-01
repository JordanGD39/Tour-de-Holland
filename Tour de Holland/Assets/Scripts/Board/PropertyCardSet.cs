using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyCardSet : MonoBehaviour
{
    public enum ColorOfSet { PURPLE, BLUE, RED, GREEN}
    [SerializeField] private ColorOfSet setColor = ColorOfSet.PURPLE;
    public ColorOfSet SetColor { get { return setColor; } }
    [SerializeField] private PropertyCard[] propertyCards;
    [SerializeField] private int[] shopLocations = { 1, 2, 5 };
    public int[] ShopLocations { get { return shopLocations; } }
    [SerializeField] private int upgradeLevel = 0;
    public int UpgradeLevel { get { return upgradeLevel; } set { upgradeLevel = value; } }

    private void Start()
    {
        foreach (PropertyCard card in propertyCards)
        {
            card.MyCardSet = this;
        }
    }
}
