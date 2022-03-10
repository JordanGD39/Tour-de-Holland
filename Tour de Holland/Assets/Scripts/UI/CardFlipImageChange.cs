using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlipImageChange : MonoBehaviour
{
    private ManageUI manageUI;

    // Start is called before the first frame update
    void Start()
    {
        manageUI = GetComponentInParent<ManageUI>();
    }

    public void SwapCurrentCardImage()
    {
        manageUI.SwapSpriteFlip();
    }
}
