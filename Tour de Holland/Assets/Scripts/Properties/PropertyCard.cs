using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPropertyCard", menuName = "ScriptableObjects/PropertyCard", order = 1)]
public class PropertyCard : ScriptableObject
{
    [SerializeField] private int buyPrice = 60;
    public int BuyPrice { get { return buyPrice; } }
    [SerializeField] private int upgradePrice = 50;
    public int UpgradePrice { get { return upgradePrice; } }
    [SerializeField] private int sellPrice = 40;
    public int SellPrice { get { return sellPrice; } }
    [SerializeField] private int rebuyPrice = 44;
    public int RebuyPrice { get { return rebuyPrice; } set { rebuyPrice = value; } }
    [SerializeField] private int[] tourFee = {20, 100, 300, 900, 1600, 2500};
    public int[] TourFee { get { return tourFee; } }
    [SerializeField] private Sprite sprite;
    public Sprite MySprite { get { return sprite; } }
    [SerializeField] private Sprite backSprite;
    public Sprite BackSprite { get { return backSprite; } }
    [SerializeField] private int upgradeLevel = 0;
    public int UpgradeLevel { get { return upgradeLevel; } set { upgradeLevel = value; } }
    [SerializeField] private List<int> shopLocations = new List<int>();
    public List<int> ShopLocations { get { return shopLocations; } set { shopLocations = value; } }

    public PropertyCardSet MyCardSet { get; set; }
    public int PropertySetIndex { get; set; }
    public PlayerData PlayerOwningThis { get; set; }
    public bool Sold { get; set; } = false;

    public string GetCardDataText()
    {
        return "No upgrades: €" + tourFee[0] + "\n Full set: €" + (tourFee[0] * 2) + "\n 1st upgrade: €" + tourFee[1] + "\n 2nd upgrade: €" + tourFee[2] + "\n 3rd upgrade: €" + tourFee[3] + "\n 4th upgrade: €" + tourFee[5] + "\n 5th upgrade: €" + tourFee[5] + "\n\n Upgrade cost: €" + upgradePrice + "\n\n Sell price: €" + sellPrice;
    }
}
