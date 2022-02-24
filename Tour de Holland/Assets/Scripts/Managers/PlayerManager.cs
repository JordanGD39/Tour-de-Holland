using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<PlayerMovement> players = new List<PlayerMovement>();
    public List<PlayerMovement> Players { get { return players; } }
    [SerializeField] private int currentTurn = 0;
    public int CurrentTurn { get { return currentTurn; } }
    public delegate void PlayersInitialized();
    public PlayersInitialized OnPlayersInitialized;
    public delegate void PlayerNextTurn();
    public PlayerNextTurn OnPlayerNextTurn;

    // Start is called before the first frame update
    void Start()
    {
        players.AddRange(FindObjectsOfType<PlayerMovement>());
        players.Sort((p1, p2) => p1.PlayerNumber.CompareTo(p2.PlayerNumber));

        foreach (PlayerMovement player in players)
        {
            player.OnEndTurn += NextTurn;
        }

        OnPlayersInitialized();
        GiveTurnToCurrentPlayer();
    }

    private void GiveTurnToCurrentPlayer()
    {
        players[currentTurn].ReceiveTurn();
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
