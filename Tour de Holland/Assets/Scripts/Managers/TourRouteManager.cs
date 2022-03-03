using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TourRouteManager : MonoBehaviour
{
    [SerializeField] private Transform[] firstSafeboardSpaces;
    [SerializeField] private Transform[] boardSpaces;

    public PropertyCardSet CardSet { get; set; }

    public Vector3 GiveFirstBoardSpaceLocation(int index)
    {
        return firstSafeboardSpaces[index].position;
    }

    public List<Vector3> GivePositionsAfterFirst(int numberOfSpaces)
    {
        List<Vector3> boardPositions = new List<Vector3>();

        for (int i = 0; i < numberOfSpaces; i++)
        {
            boardPositions.Add(boardSpaces[i].position);
        }

        return boardPositions;
    }
}
