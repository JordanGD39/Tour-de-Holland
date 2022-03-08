using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    public enum CharacterSelection { MOLEN, TULP, KAASBLOK, RONDEKAAS, KLOMP, PATAT, VIS, OLIEBOL };
    public List<CharacterSelection> characterSelections = new List<CharacterSelection>();
    private GameObject playerpref;
    private PlayerData players;
    public List<PlayerData> amountOfPlayers = new List<PlayerData>();
    

    public void CharSelect(int characterIndex)
    {
        characterSelections.Add((CharacterSelection)characterIndex);
        players.PlayerModel = characterIndex;
    }

    public void Confirm()
    {
        amountOfPlayers.Add(players);
        Debug.Log(amountOfPlayers.Count);
    }
}
