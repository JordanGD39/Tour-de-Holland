using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<PlayerClassHolder> players = new List<PlayerClassHolder>();
    public List<PlayerClassHolder> Players { get { return players; } }
    [SerializeField] private int currentTurn = 0;
    public int CurrentTurn { get { return currentTurn; } }
    public delegate void PlayersInitialized();
    public PlayersInitialized OnPlayersInitialized;
    public delegate void PlayerNextTurn();
    public PlayerNextTurn OnPlayerNextTurn;

    [SerializeField] private Text playerWinText;
    [SerializeField] private List<GameObject> playerModelPrefabs = new List<GameObject>();
    [SerializeField] private List<Sprite> playerIcons = new List<Sprite>();
    [SerializeField] private GameObject fadeOut;
    [SerializeField] private GameObject fadeIn;

    private void Start()
    {
        fadeOut.SetActive(false);
        fadeIn.SetActive(true);

        CreatePlayerList();

        if (playerWinText != null)
        {
            playerWinText.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    private void CreatePlayerList()
    {
        List<PlayerData> playerDatas = new List<PlayerData>();
        playerDatas.AddRange(FindObjectsOfType<PlayerData>());
        playerDatas.Sort((p1, p2) => p1.PlayerNumber.CompareTo(p2.PlayerNumber));

        int removePlayersCount = playerDatas.Count - PlayerSelectionManager.instance.ChosenPlayerModels.Count;

        for (int i = 0; i < removePlayersCount; i++)
        {
            playerDatas.RemoveAt(playerDatas.Count - 1);
        }

        foreach (PlayerData item in playerDatas)
        {
            PlayerClassHolder playerClassHolder = new PlayerClassHolder();
            playerClassHolder.playerData = item;
            playerClassHolder.playerMovement = item.GetComponent<PlayerMovement>();
            players.Add(playerClassHolder);
        }

        int playerIndex = 0;

        foreach (PlayerClassHolder player in players)
        {
            player.playerMovement.OnEndTurn += NextTurn;
            player.playerData.OnLost += RemovePlayerFromTurnList;

            int index = (int)PlayerSelectionManager.instance.ChosenPlayerModels[playerIndex];
            player.playerData.PlayerIcon = playerIcons[index];
            player.playerData.SetIcon();

            GameObject model = Instantiate(playerModelPrefabs[index], player.playerData.transform);
            model.transform.localPosition = Vector3.zero;
            playerIndex++;
        }

        FindObjectOfType<CheckPlayerCountUI>().RemovePlayerPanels(players.Count);

        OnPlayersInitialized();
        GiveTurnToCurrentPlayer();
    }

    private void GiveTurnToCurrentPlayer()
    {
        players[currentTurn].playerMovement.ReceiveTurn();
    }

    private void NextTurn()
    {
        currentTurn++;

        if (currentTurn > players.Count - 1)
        {
            currentTurn = 0;
        }

        OnPlayerNextTurn();
        GiveTurnToCurrentPlayer();
    }

    public void EveryPlayerCheckCorrectPosition(PlayerMovement playerMovement, int spaceIndex, bool tour)
    {
        foreach (PlayerClassHolder player in players)
        {
            if (player.playerMovement != playerMovement)
            {
                player.playerMovement.CheckIfPlayersOnCurrentSpace(spaceIndex, tour);
            }
        }
    }

    private void RemovePlayerFromTurnList()
    {
        int index = players.IndexOf(players[currentTurn]);

        players[currentTurn].playerMovement.gameObject.SetActive(false);

        players.RemoveAt(currentTurn);

        if (currentTurn >= index && currentTurn > 0)
        {
            currentTurn--;
        }

        if (players.Count > 1)
        {
            NextTurn();
        }
        else
        {
            GameEnd();
        }        
    }

    private void GameEnd()
    {
        playerWinText.text = "Player " + (players[currentTurn].playerData.PlayerNumber + 1) + " wins!";

        playerWinText.gameObject.SetActive(true);

        Invoke(nameof(FadeStart), 3);
        Invoke(nameof(ToMainMenu), 3.5f);
    }

    private void FadeStart()
    {
        fadeIn.SetActive(true);
    }

    private void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

public class PlayerClassHolder
{
    public PlayerData playerData;
    public PlayerMovement playerMovement;
}