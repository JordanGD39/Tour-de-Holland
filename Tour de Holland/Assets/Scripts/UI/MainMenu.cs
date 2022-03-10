using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject firstPanel;
    [SerializeField] private GameObject secondPanel;
    [SerializeField] private GameObject fadeIn;
    [SerializeField] private GameObject fadeout;
    [SerializeField] private Text playerCountText;

    private int playerCount = 2;

    // Start is called before the first frame update
    void Start()
    {
        firstPanel.SetActive(true);
        secondPanel.SetActive(false);
        fadeIn.SetActive(true);
        fadeout.SetActive(false);
    }

    public void PlayButton()
    {
        Invoke(nameof(ShowSecondPanel), 0.5f);
    }

    private void ShowSecondPanel()
    {
        firstPanel.SetActive(false);
        secondPanel.SetActive(true);
        fadeIn.SetActive(false);
        fadeIn.SetActive(true);
        fadeout.SetActive(false);
    }

    public void ChangePlayerCount(bool add)
    {
        playerCount += add ? 1 : -1;

        playerCount = Mathf.Clamp(playerCount, 2, 4);

        playerCountText.text = playerCount.ToString();
    }

    public void Confirm()
    {
        PlayerSelectionManager.instance.ChooseRandomModels(playerCount);

        fadeout.SetActive(true);
        Invoke(nameof(GoToBoardScene), 0.5f);
    }

    private void GoToBoardScene()
    {
        SceneManager.LoadScene("BoardScene");
    }

    public void QuitButton()
    {
        Invoke(nameof(Quiting), 0.5f);
    }

    private void Quiting()
    {
        Application.Quit();
    }
}
