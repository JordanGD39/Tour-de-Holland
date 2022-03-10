using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllRoutesManager : MonoBehaviour
{
    private List<TourRouteManager> tourRouteManagers = new List<TourRouteManager>();

    // Start is called before the first frame update
    void Start()
    {
        tourRouteManagers.AddRange(FindObjectsOfType<TourRouteManager>());
    }

    public void HideAllShops()
    {
        foreach (TourRouteManager item in tourRouteManagers)
        {
            item.HideShops();
        }
    }
}
