using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPropertyInManageButton : MonoBehaviour
{
    private ManageUI manageUI;

    // Start is called before the first frame update
    void Start()
    {
        if (manageUI != null)
        {
            return;
        }

        manageUI = GetComponentInParent<ManageUI>();
    }

    public void ShowProperty()
    {
        manageUI.ShowPropertyImage(transform.GetSiblingIndex());
    }
}
