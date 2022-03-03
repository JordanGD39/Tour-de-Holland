using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        List<PlayerData> playerDatas = new List<PlayerData>();
        playerDatas.AddRange(FindObjectsOfType<PlayerData>());
        playerDatas.Sort((p1, p2) => p1.PlayerNumber.CompareTo(p2.PlayerNumber));

        foreach (PlayerData item in playerDatas)
        {
            PlayerClassHolder playerClassHolder = new PlayerClassHolder();
            playerClassHolder.playerData = item;
            playerClassHolder.playerMovement = item.GetComponent<PlayerMovement>();
            players.Add(playerClassHolder);
        }

        foreach (PlayerClassHolder player in players)
        {
            player.playerMovement.OnEndTurn += NextTurn;
        }

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
}

public class PlayerClassHolder
{
    public PlayerData playerData;
    public PlayerMovement playerMovement;
}