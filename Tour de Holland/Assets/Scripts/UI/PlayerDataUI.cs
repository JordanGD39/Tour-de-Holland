using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataUI : MonoBehaviour
{
    [SerializeField] private Text moneyText;
    [SerializeField] private Text addMoneyText;
    [SerializeField] private Transform[] purpleProperties;
    [SerializeField] private Transform[] blueProperties;
    [SerializeField] private Transform[] redProperties;
    [SerializeField] private Transform[] greenProperties;
    [SerializeField] private float lerpSpeed = 2;

    private float currentMoneyShowed = 0;
    private float targetMoney = 0;
    

    private void Start()
    {
        addMoneyText.gameObject.SetActive(false);
    }

    private IEnumerator CountMoneyToCurrentRealMoney()
    {
        yield return new WaitForSeconds(0.75f);

        while (currentMoneyShowed != targetMoney)
        {
            currentMoneyShowed = Mathf.Lerp(currentMoneyShowed, targetMoney, lerpSpeed * Time.deltaTime);

            moneyText.text = Mathf.RoundToInt(currentMoneyShowed).ToString();

            yield return null;
        }
    }

    public void UpdateMoneyText(int money, int oldMoney)
    {
        int diff = money - oldMoney;

        string plusOrMin = diff > 0 ? "+" : "-";

        addMoneyText.text = plusOrMin + " €" + Mathf.Abs(diff).ToString();
        addMoneyText.gameObject.SetActive(false);
        addMoneyText.gameObject.SetActive(true);

        currentMoneyShowed = oldMoney;
        targetMoney = money;

        StartCoroutine(nameof(CountMoneyToCurrentRealMoney));
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
