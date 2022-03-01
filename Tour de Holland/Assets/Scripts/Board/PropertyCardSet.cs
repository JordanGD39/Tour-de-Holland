using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyCardSet : MonoBehaviour
{
    public enum ColorOfSet { PURPLE, BLUE, RED, GREEN}
    [SerializeField] private int[] shopLocations = { 1, 2, 5 };
    [SerializeField] private int upgradeLevel = 0;
}
