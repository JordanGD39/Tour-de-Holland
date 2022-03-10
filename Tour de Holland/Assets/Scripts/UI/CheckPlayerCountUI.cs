using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerCountUI : MonoBehaviour
{
    public void RemovePlayerPanels(int playerCount)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < playerCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
