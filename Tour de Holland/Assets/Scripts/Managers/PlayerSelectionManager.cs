using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectionManager : MonoBehaviour
{
    public enum PlayerModelChosen { CHEESE, FISH, OLIEBOL, PLANT, ROUNDCHEESE, SHOE, STROOPWAFEL, WINDMILL }
    [SerializeField] private List<PlayerModelChosen> chosenPlayerModels = new List<PlayerModelChosen>();
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

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
