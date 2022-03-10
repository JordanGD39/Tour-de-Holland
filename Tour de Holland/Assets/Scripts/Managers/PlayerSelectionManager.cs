using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectionManager : MonoBehaviour
{
    public enum PlayerModelChosen { CHEESE, FISH, OLIEBOL, PLANT, ROUNDCHEESE, SHOE, STROOPWAFEL, WINDMILL }
    [SerializeField] private List<PlayerModelChosen> chosenPlayerModels = new List<PlayerModelChosen>();
    [SerializeField] private List<PlayerModelChosen> notChosenModels = new List<PlayerModelChosen>();
    public List<PlayerModelChosen> ChosenPlayerModels { get { return chosenPlayerModels; } }

    public static PlayerSelectionManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        AddNotChosen();
    }

    private void AddNotChosen()
    {
        notChosenModels.Clear();
        notChosenModels.Add(PlayerModelChosen.CHEESE);
        notChosenModels.Add(PlayerModelChosen.FISH);
        notChosenModels.Add(PlayerModelChosen.OLIEBOL);
        notChosenModels.Add(PlayerModelChosen.PLANT);
        notChosenModels.Add(PlayerModelChosen.ROUNDCHEESE);
        notChosenModels.Add(PlayerModelChosen.SHOE);
        notChosenModels.Add(PlayerModelChosen.STROOPWAFEL);
        notChosenModels.Add(PlayerModelChosen.WINDMILL);
    }

    public void ChooseRandomModels(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
        {
            int index = Random.Range(0, notChosenModels.Count);
            chosenPlayerModels.Add(notChosenModels[index]);
            notChosenModels.RemoveAt(index);
        }

        AddNotChosen();
    }
}
