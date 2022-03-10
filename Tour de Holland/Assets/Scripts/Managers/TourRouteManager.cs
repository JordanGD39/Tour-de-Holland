using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TourRouteManager : MonoBehaviour
{
    [SerializeField] private Transform[] firstSafeboardSpaces;
    [SerializeField] private Transform[] boardSpaces;
    [SerializeField] private Transform shopsParent;
    [SerializeField] private Material shopMaterial;
    [SerializeField] private Material normalSpaceMaterial;
    private List<Animator> shopAnims = new List<Animator>();

    public PropertyCardSet CardSet { get; set; }

    private void Start()
    {
        for (int i = 0; i < shopsParent.childCount; i++)
        {
            Transform shop = shopsParent.GetChild(i);
            shop.gameObject.SetActive(i % 2 == 0);
            shopAnims.Add(shop.GetComponentInChildren<Animator>());
        }
    }

    public Vector3 GiveFirstBoardSpaceLocation(int index)
    {
        return firstSafeboardSpaces[index].position;
    }

    public List<Vector3> GivePositionsAfterFirst(int numberOfSpaces)
    {
        List<Vector3> boardPositions = new List<Vector3>();

        for (int i = 0; i < numberOfSpaces; i++)
        {
            boardPositions.Add(boardSpaces[i].position);
        }

        return boardPositions;
    }

    public void UpdateShops(PropertyCard card)
    {
        foreach (Transform space in boardSpaces)
        {
            space.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = normalSpaceMaterial;
        }

        for (int i = 0; i < card.ShopLocations.Count; i++)
        {
            boardSpaces[card.ShopLocations[i] - 1].GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = shopMaterial;

            shopsParent.GetChild(card.ShopLocations[i] - 1).gameObject.SetActive(false);
            shopsParent.GetChild(card.ShopLocations[i] - 1).gameObject.SetActive(true);
        }
    }

    public void HideShops()
    {
        bool secondUpgradeInSet = false;

        foreach (PropertyCard card in CardSet.PropertyCardsInSet)
        {
            if (card.UpgradeLevel > 1)
            {
                secondUpgradeInSet = true;
                break;
            }
        }

        if (!secondUpgradeInSet)
        {
            return;
        }

        foreach (Animator item in shopAnims)
        {
            item.SetTrigger("Hide");
        }
    }
}
