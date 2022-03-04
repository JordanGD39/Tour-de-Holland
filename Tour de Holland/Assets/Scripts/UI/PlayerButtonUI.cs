using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerButtonUI : MonoBehaviour
{
    private PlayerManager playerManager;
    [SerializeField] private Button spinButton;
    [SerializeField] private GameObject tradeButton;
    [SerializeField] private GameObject endTurnButton;
    [SerializeField] private Button manageButton;
    [SerializeField] private GameObject managePanel;

    [SerializeField] private Animator spinButtonAnim;
    [SerializeField] private Animator tradeButtonAnim;
    [SerializeField] private Animator endTurnButtonAnim;

    private bool spinned = false;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        managePanel.SetActive(false);
        spinButton.gameObject.SetActive(false);
        spinButton.gameObject.SetActive(true);
        spinButton.interactable = true;
        tradeButton.gameObject.SetActive(false);
        endTurnButton.gameObject.SetActive(false);
        manageButton.interactable = true;
        manageButton.gameObject.SetActive(false);
        manageButton.gameObject.SetActive(true);
    }

    public void Spin()
    {
        spinned = true;
        manageButton.interactable = false;
        PlayerMovement player = playerManager.Players[playerManager.CurrentTurn].playerMovement;
        spinButton.interactable = false;
        player.OnDoneMoving = () => { manageButton.interactable = true; spinButtonAnim.SetTrigger("FadeOut"); endTurnButton.SetActive(false); endTurnButton.SetActive(true); };
        player.SpinWheel();
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
            tradeButton.SetActive(false);
            tradeButton.SetActive(true);
        }        
    }

    public void EndTurn()
    {
        spinned = false;
        endTurnButtonAnim.SetTrigger("FadeOut");
        spinButton.gameObject.SetActive(false);
        spinButton.gameObject.SetActive(true);
        spinButton.interactable = true;

        playerManager.Players[playerManager.CurrentTurn].playerMovement.OnEndTurn();
    }

    public void DeactivateButton(GameObject button)
    {
        button.SetActive(false);
    }
}
