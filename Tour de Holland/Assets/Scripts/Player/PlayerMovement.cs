using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private SpacesManager spacesManager;
    [SerializeField] private int currentBoardPosition = 0;
    [SerializeField] private bool canSpin = false;
    [SerializeField] private bool goTowardsSpace = false;
    [SerializeField] private float journeyTime = 1;
    [SerializeField] private float arcHeight = 1;
    [SerializeField] private float extraY = 1;
    [SerializeField] private float rotateSpeed = 1;

    private BoardSpace currentBoardSpace;
    private Vector3 startingPosition;
    private Vector3 spacePos;
    private List<Vector3> spacePosistions = new List<Vector3>();
    private List<int> cutsceneSpaceIndexs = new List<int>();
    private float startTime;
    private int moveToSpaceIndex = 0;
    private bool goingToCutsceneSpace = false;
    private int previousSpace = 0;

    public delegate void EndTurn();
    public EndTurn OnEndTurn;

    private PlayerData playerData;

    private bool onTour = false;
    private bool rolledOnTour = false;
    private bool backOnNormalRoute = true;
    private TourRouteManager tourRouteManager;

    // Start is called before the first frame update
    void Start()
    {
        spacesManager = FindObjectOfType<SpacesManager>();
        playerData = GetComponent<PlayerData>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpin && Input.GetKeyDown(KeyCode.S))
        {
            SpinWheel();
        }

        if (goTowardsSpace)
        {
            MoveTowardsBoardSpace();
        }
    }

    public void PlaceOnTourRoute(TourRouteManager aTourRouteManager)
    {
        tourRouteManager = aTourRouteManager;
        onTour = true;
        rolledOnTour = false;
        backOnNormalRoute = false;
        SetStartingVariablesForStartMoving();

        spacePos = tourRouteManager.GiveFirstBoardSpaceLocation(currentBoardSpace.PropertyCardOnSpace.PropertySetIndex) + Vector3.up * extraY;
        goTowardsSpace = true;
    }

    public void ReceiveTurn()
    {
        canSpin = true;
    }

    private void SpinWheel()
    {
        canSpin = false;
        CalculatePosition(Random.Range(1, 7));
        //CalculatePosition(40);
    }

    private void CalculatePosition(int spinnedNumber)
    {
        if (onTour)
        {
            rolledOnTour = true;
            spacePosistions = tourRouteManager.GivePositionsAfterFirst(spinnedNumber);
            moveToSpaceIndex = 0;
            StartMoving();

            return;
        }

        int calcBoardPos = currentBoardPosition + spinnedNumber;

        GetInBetweenBoardSpaces(calcBoardPos);

        currentBoardSpace = spacesManager.GetBoardSpace(calcBoardPos);
        Debug.Log("BoardPos: " + currentBoardPosition + " spinnedNum: " + spinnedNumber);
        currentBoardPosition = currentBoardSpace.BoardIndex;

        moveToSpaceIndex = 0;
        StartMoving();
    }

    private void SetStartingVariablesForStartMoving()
    {
        startTime = Time.time;
        startingPosition = transform.position;        
    }

    private void StartMoving()
    {
        SetStartingVariablesForStartMoving();
        spacePos = spacePosistions[moveToSpaceIndex] + Vector3.up * extraY;

        goingToCutsceneSpace = false;

        if (cutsceneSpaceIndexs.Count > 0 && !onTour)
        {
            foreach (int item in cutsceneSpaceIndexs)
            {
                if (moveToSpaceIndex == item)
                {
                    goingToCutsceneSpace = true;
                }
            }
        }

        goTowardsSpace = true;
    }

    private void GetInBetweenBoardSpaces(int calcBoardPos)
    {        
        int numberOfSpacesBetween = calcBoardPos - currentBoardPosition;

        MoveToSpaceData moveToSpaceData = spacesManager.GetMoveToSpaces(numberOfSpacesBetween, currentBoardPosition);
        spacePosistions = moveToSpaceData.spacePositions;
        cutsceneSpaceIndexs = moveToSpaceData.cutsceneSpaceIndexs;
    }

    private void MoveTowardsBoardSpace()
    {
        // The center of the arc
        Vector3 center = (startingPosition + spacePos) * 0.5F;

        // move the center a bit downwards to make the arc vertical
        center -= new Vector3(0, arcHeight, 0);

        // Interpolate over the arc relative to center
        Vector3 startRelCenter = startingPosition - center;
        Vector3 spaceRelCenter = spacePos - center;

        // The fraction of the animation that has happened so far is
        // equal to the elapsed time divided by the desired time for
        // the total journey.
        float fracComplete = (Time.time - startTime) / journeyTime;

        transform.position = Vector3.Slerp(startRelCenter, spaceRelCenter, fracComplete);
        transform.position += center;

        Vector3 lookPos = spacePos;
        lookPos.y = transform.position.y;

        Vector3 diff = lookPos - transform.position;

        if (diff != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(diff);

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }        

        if (fracComplete >= 1)
        {
            moveToSpaceIndex++;
            goTowardsSpace = false;

            if (onTour && moveToSpaceIndex > spacePosistions.Count - 1)
            {
                TourCheck();

                return;
            }

            if (!goingToCutsceneSpace)
            {
                if (moveToSpaceIndex > spacePosistions.Count - 1)
                {
                    previousSpace = currentBoardPosition;
                    playerData.CheckCurrentSpace(currentBoardSpace);
                }
                else
                {
                    StartMoving();
                }
            }   
            else
            {
                ExtraSpace extraSpace = spacesManager.GetCutsceneSpaceFromStartingBoardPos(previousSpace);
                previousSpace = extraSpace.SpaceBeforeThisIndex + 1;

                if (previousSpace > spacesManager.GetSpacesCount() - 1)
                {
                    int index = previousSpace - spacesManager.GetSpacesCount();
                    previousSpace = index;
                }

                extraSpace.OnCutsceneDone = StartMoving;
                extraSpace.OnPlayCutscene(transform);
            }
        }
    }

    private void TourCheck()
    {
        if (!rolledOnTour)
        {
            OnEndTurn();
            return;
        }

        if (backOnNormalRoute)
        {
            onTour = false;
            OnEndTurn();
        }
        else
        {
            PropertyCardSet propertyCardSet = tourRouteManager.CardSet;

            foreach (int pos in propertyCardSet.ShopLocations)
            {
                if (moveToSpaceIndex == pos)
                {
                    int moneyLoss = currentBoardSpace.PropertyCardOnSpace.TourFee[propertyCardSet.UpgradeLevel];
                    playerData.Money -= moneyLoss;
                    currentBoardSpace.PropertyCardOnSpace.PlayerOwningThis.Money += moneyLoss;
                }                
            }

            ReturnToBoard();
        }
    }

    private void ReturnToBoard()
    {
        SetStartingVariablesForStartMoving();

        spacePos = currentBoardSpace.transform.position + Vector3.up * extraY;
        backOnNormalRoute = true;
        goTowardsSpace = true;
    }
}
