using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataUI : MonoBehaviour
{
    [SerializeField] private Text moneyText;
    [SerializeField] private Transform[] purpleProperties;
    [SerializeField] private Transform[] blueProperties;
    [SerializeField] private Transform[] redProperties;
    [SerializeField] private Transform[] greenProperties;

    public void UpdateMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }

    public void UpdateProperties(PropertyCard propertyCard, bool add)
    {
        switch (propertyCard.MyCardSet.PropertySetColor)
        {
            case PropertyCardSet.ColorOfSet.PURPLE:
                UpdateProperty(purpleProperties[propertyCard.PropertySetIndex], add);
                break;
            case PropertyCardSet.ColorOfSet.BLUE:
                UpdateProperty(blueProperties[propertyCard.PropertySetIndex], add);
                break;
            case PropertyCardSet.ColorOfSet.RED:
                UpdateProperty(redProperties[propertyCard.PropertySetIndex], add);
                break;
            case PropertyCardSet.ColorOfSet.GREEN:
                UpdateProperty(greenProperties[propertyCard.PropertySetIndex], add);
                break;
        }
    }

    private void UpdateProperty(Transform image, bool add)
    {
        image.transform.GetChild(add ? 0 : 1).gameObject.SetActive(true);
        image.transform.GetChild(add ? 1 : 0).gameObject.SetActive(false);
    }

    public void ShowBuyProperty()
    {

    }
}
