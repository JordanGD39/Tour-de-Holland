using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private int playerNumber = 0;
    public int PlayerNumber { get { return playerNumber; } }
    [SerializeField] private int money = 1500;
    public int Money { get { return money; } set { money = value; } }
    [SerializeField] private List<PropertyCard> propertyCards;
    public List<PropertyCard> PropertyCards { get { return propertyCards; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
