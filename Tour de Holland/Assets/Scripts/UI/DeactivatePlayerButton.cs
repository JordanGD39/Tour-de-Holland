using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivatePlayerButton : MonoBehaviour
{
    private PlayerButtonUI buttonUI;

    // Start is called before the first frame update
    void Start()
    {
        if (buttonUI != null)
        {
            return;
        }

        buttonUI = GetComponentInParent<PlayerButtonUI>();
    }

    public void HideButton()
    {
        buttonUI.DeactivateButton(gameObject);
    }
}
