using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPropertyCard", menuName = "ScriptableObjects/PropertyCard", order = 1)]
public class PropertyCard : ScriptableObject
{
    [SerializeField] private int buyPrice = 60;
    public int BuyPrice { get { return buyPrice; } }
    [SerializeField] private int sellPrice = 40;
    public int SellPrice { get { return sellPrice; } }
    [SerializeField] private int[] tourFee = {20, 100, 300, 900, 1600, 2500};
    public int[] TourFee { get { return tourFee; } }
    [SerializeField] private Sprite sprite;
    public Sprite MySprite { get { return sprite; } }

    public PropertyCardSet MyCardSet { get; set; }
    public int PropertySetIndex { get; set; }
}
