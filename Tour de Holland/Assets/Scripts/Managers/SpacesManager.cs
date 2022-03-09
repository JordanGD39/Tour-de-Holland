using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacesManager : MonoBehaviour
{
    private PlayerManager playerManager;

    [SerializeField] private List<BoardSpace> boardSpaces = new List<BoardSpace>();
    [SerializeField] private List<ExtraSpace> extraSpaces = new List<ExtraSpace>();
    [SerializeField] private List<ExtraSpace> cutsceneSpaces = new List<ExtraSpace>();

    private int extraCutsceneIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        Transform spacesParent = GameObject.FindGameObjectWithTag("RealSpaces").transform;

        for (int i = 0; i < spacesParent.childCount; i++)
        {
            BoardSpace space = spacesParent.GetChild(i).GetComponent<BoardSpace>();
            boardSpaces.Add(space);
            space.BoardIndex = i;
        }

        extraSpaces.AddRange(FindObjectsOfType<ExtraSpace>());

        foreach (ExtraSpace extraSpace in extraSpaces)
        {
            if (extraSpace.CutsceneSpace)
            {
                cutsceneSpaces.Add(extraSpace);
            }            
        }

        cutsceneSpaces.Sort((p1, p2) => p1.SpaceBeforeThisIndex.CompareTo(p2.SpaceBeforeThisIndex));
    }

    public BoardSpace GetBoardSpace(int spaceIndex)
    {
        if (spaceIndex > boardSpaces.Count - 1)
        {
            int index = spaceIndex - boardSpaces.Count;
            spaceIndex = index;
        }

        return boardSpaces[spaceIndex];
    }

    public MoveToSpaceData GetMoveToSpaces(int numberOfSpaces, int currentPos, bool jail)
    {
        extraCutsceneIndex = 1;
        List<Vector3> spacePositions = new List<Vector3>();
        MoveToSpaceData moveToSpaceData = new MoveToSpaceData();
        int startIndex = -1;
        Debug.Log(currentPos);

        CheckExtraSpaces(!jail ? currentPos : -1, -1, spacePositions, moveToSpaceData);

        for (int i = 0; i < numberOfSpaces; i++)
        {
            int calcPos = currentPos + i + 1;

            if (calcPos > boardSpaces.Count - 1)
            {
                int index = calcPos - boardSpaces.Count;
                calcPos = index;
            }

            BoardSpace space = GetBoardSpace(calcPos);
            spacePositions.Add(CheckOtherPlayersOnSpace(calcPos, space.transform.position, false, -1));

            int extraI = 0;

            if (jail)
            {
                extraI = 1;
                jail = false;
            }

            if (i < numberOfSpaces - 1)
            {
                CheckExtraSpaces(calcPos + extraI, i, spacePositions, moveToSpaceData);
            }

            if (calcPos == 0)
            {
                startIndex = spacePositions.Count;
            }
        }
        
        moveToSpaceData.spacePositions = spacePositions;
        moveToSpaceData.startIndex = startIndex;

        return moveToSpaceData;
    }

    public Vector3 CheckOtherPlayersOnSpace(int boardIndex, Vector3 spacePosition, bool onTourCheck, int playerIndex)
    {
        int playerCount = 0;

        foreach (PlayerClassHolder player in playerManager.Players)
        {
            if (player.playerData.PlayerNumber != playerIndex && boardIndex == player.playerMovement.CurrentBoardPosition && player.playerMovement.OnTour == onTourCheck)
            {
                playerCount++;
            } 
        }

        if (playerCount > 0)
        {
            spacePosition.y -= 0.06f;
        }

        switch (playerCount)
        {
            case 1:
                spacePosition.z -= 0.6f;
                break;
            case 2:
                spacePosition.z += 0.6f;
                break;
            case 3:
                spacePosition.x -= 0.6f;
                break;
            default:
                break;
        }

        return spacePosition;
    }

    private void CheckExtraSpaces(int pos, int index, List<Vector3> spacePositions, MoveToSpaceData moveToSpaceData)
    {
        for (int i = 0; i < extraSpaces.Count; i++)
        {
            if (pos == extraSpaces[i].SpaceBeforeThisIndex)
            {
                spacePositions.Add(extraSpaces[i].ExtraSpacePos);

                if (extraSpaces[i].CutsceneSpace)
                {
                    moveToSpaceData.cutsceneSpaceIndexs.Add(index + extraCutsceneIndex);                    
                }

                extraCutsceneIndex++;
            }
        }      
    }

    public ExtraSpace GetCutsceneSpaceFromStartingBoardPos(int startingSpace)
    {
        foreach (ExtraSpace extraSpace in cutsceneSpaces)
        {
            if (startingSpace <= extraSpace.SpaceBeforeThisIndex)
            {
                return extraSpace;
            }
        }

        return null;
    }

    public int GetSpacesCount()
    {
        return boardSpaces.Count;
    }
}

public class MoveToSpaceData
{
    public List<Vector3> spacePositions = new List<Vector3>();
    public List<int> cutsceneSpaceIndexs = new List<int>();
    public int startIndex = -1;
}
