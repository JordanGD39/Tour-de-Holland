using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacesManager : MonoBehaviour
{
    [SerializeField] private List<BoardSpace> boardSpaces = new List<BoardSpace>();
    [SerializeField] private List<ExtraSpace> extraSpaces = new List<ExtraSpace>();
    [SerializeField] private List<ExtraSpace> cutsceneSpaces = new List<ExtraSpace>();

    private int extraCutsceneIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
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

    public MoveToSpaceData GetMoveToSpaces(int numberOfSpaces, int currentPos)
    {
        extraCutsceneIndex = 1;
        List<Vector3> spacePositions = new List<Vector3>();
        MoveToSpaceData moveToSpaceData = new MoveToSpaceData();
        Debug.Log(currentPos);
        CheckExtraSpaces(currentPos, -1, spacePositions, moveToSpaceData);

        for (int i = 0; i < numberOfSpaces; i++)
        {
            int calcPos = currentPos + i + 1;

            if (calcPos > boardSpaces.Count - 1)
            {
                int index = calcPos - boardSpaces.Count;
                calcPos = index;
            }

            BoardSpace space = GetBoardSpace(calcPos);
            spacePositions.Add(space.transform.position);

            if (i < numberOfSpaces - 1)
            {
                CheckExtraSpaces(calcPos, i, spacePositions, moveToSpaceData);
            }
        }
        
        moveToSpaceData.spacePositions = spacePositions;

        return moveToSpaceData;
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
}
