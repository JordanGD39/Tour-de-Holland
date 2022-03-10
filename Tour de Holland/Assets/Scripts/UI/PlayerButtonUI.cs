using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerButtonUI : MonoBehaviour
{
    private PlayerManager playerManager;
    private TradingManager tradingManager;
    [SerializeField] private Button spinButton;
    [SerializeField] private GameObject tradeButton;
    [SerializeField] private GameObject endTurnButton;
    [SerializeField] private Button manageButton;
    [SerializeField] private GameObject managePanel;
    [SerializeField] private ManageUI manageUI;

    [SerializeField] private Animator spinButtonAnim;
    [SerializeField] private Animator tradeButtonAnim;
    [SerializeField] private Animator endTurnButtonAnim;
    [SerializeField] private Animator payJailAnim;

    private bool spinned = false;
    private bool canEndTurn = true;
    private RectTransform payJailRect;
    private Button payJailButton;
    private RectTransform spinRect;
    private Vector3 startPosPay;

    // Start is called before the first frame update
    void Start()
    {
        tradingManager = FindObjectOfType<TradingManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        managePanel.SetActive(false);
        spinButton.gameObject.SetActive(false);
        spinButton.gameObject.SetActive(true);
        spinButton.interactable = true;
        tradeButton.gameObject.SetActive(false);
        endTurnButton.gameObject.SetActive(false);
        manageButton.interactable = true;
        manageButton.gameObject.SetActive(false);
        payJailAnim.gameObject.SetActive(false);
        manageButton.gameObject.SetActive(true);

        payJailRect = payJailAnim.GetComponent<RectTransform>();
        spinRect = spinButtonAnim.GetComponent<RectTransform>();
        payJailButton = payJailAnim.GetComponent<Button>();
        startPosPay = payJailRect.localPosition;
    }

    public void Spin()
    {
        spinned = true;
        manageButton.interactable = false;
        PlayerMovement player = playerManager.Players[playerManager.CurrentTurn].playerMovement;
        spinButton.interactable = false;
        player.OnDoneMoving = AfterSpin;
        player.SpinWheel();
    }

    private void AfterSpin()
    {
        manageButton.interactable = true;
        spinButtonAnim.SetTrigger("FadeOut");

        PlayerClassHolder player = playerManager.Players[playerManager.CurrentTurn];

        if (player.playerData.InJail && player.playerMovement.JailTurns <= 3)
        {
            ShowPayJailButton(false);
        }

        endTurnButton.SetActive(false);
        endTurnButton.SetActive(true);
    }

    public void ToggleManage()
    {
        managePanel.SetActive(!managePanel.activeSelf);

        if (!spinned)
        {
            spinButton.interactable = !managePanel.activeSelf;
        }
        else
        {
            if (managePanel.activeSelf)
            {
                endTurnButtonAnim.SetTrigger("FadeOut");
            }
            else
            {
                endTurnButton.SetActive(false);
                endTurnButton.SetActive(true);
            }
            
        }

        if (!managePanel.activeSelf)
        {
            tradeButtonAnim.SetTrigger("FadeOut");
        }
        else
        {
            manageUI.HideOpenProperty();
            tradeButton.SetActive(false);
            tradeButton.SetActive(true);
        }        
    }

    public void ShowSpin()
    {
        endTurnButtonAnim.SetTrigger("FadeOut");
        payJailAnim.SetTrigger("FadeOut");
        spinButton.gameObject.SetActive(false);
        spinButton.gameObject.SetActive(true);
        spinButton.interactable = true;
    }

    public void EndTurn()
    {
        if (!canEndTurn)
        {
            return;
        }

        canEndTurn = false;

        spinned = false;
        ShowSpin();

        PlayerClassHolder player = playerManager.Players[playerManager.CurrentTurn];

        player.playerData.CheckLost();

        if (!player.playerData.DidLose)
        {
            player.playerMovement.OnEndTurn();
        }

        Invoke(nameof(MayEndTurnAgain), 1);
    }

    private void MayEndTurnAgain()
    {
        canEndTurn = true;
    }

    public void OpenTradePanel()
    {
        managePanel.SetActive(false);
        tradingManager.ShowPlayerTradeSelect();
    }

    public void ShowPayJailButton(bool forced)
    {
        if (!forced)
        {
            PlayerData player = playerManager.Players[playerManager.CurrentTurn].playerData;

            payJailButton.interactable = player.Money >= 50;
        }
        else
        {
            payJailButton.interactable = true;
            spinButton.interactable = false;
        }

        payJailAnim.gameObject.SetActive(false);
        payJailAnim.gameObject.SetActive(true);
    }

    public void PayJail()
    {
        if (!spinButton.interactable)
        {
            spinButton.interactable = true;
        }

        playerManager.Players[playerManager.CurrentTurn].playerData.GetOutOfJail(true);

        payJailAnim.SetTrigger("FadeOut");
    }

    public void DeactivateButton(GameObject button)
    {
        button.SetActive(false);
    }
}
