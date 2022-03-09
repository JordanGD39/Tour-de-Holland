using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScriptsManager : MonoBehaviour
{
    [SerializeField] private BuyPropertyUIHandler buyPropertyUIHandler;
    public BuyPropertyUIHandler BuyPropertyUIScript { get { return buyPropertyUIHandler; } }
}
