using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private SpacesManager spacesManager;
    [SerializeField] private int currentBoardPosition = 0;
    public int CurrentBoardPosition { get { return currentBoardPosition; } }
    [SerializeField] private bool canSpin = false;
    [SerializeField] private bool goTowardsSpace = false;
    [SerializeField] private float journeyTime = 1;
    [SerializeField] private float arcHeight = 1;
    [SerializeField] private float extraY = 1;
    [SerializeField] private float rotateSpeed = 1;
    [SerializeField] private float extraTrainSpaceYToStartFalling = 5;
    [SerializeField] private float fallTime = 1;
    [SerializeField] private int debugMove = 1;
    [SerializeField] private bool moveDebugSpaces = false;

    private BoardSpace currentBoardSpace;
    private Vector3 startingPosition;
    private Vector3 spacePos;
    private List<Vector3> spacePosistions = new List<Vector3>();
    private List<int> cutsceneSpaceIndexs = new List<int>();
    private int startMoveIndex = -1;
    private float startTime;
    private int moveToSpaceIndex = 0;
    private bool goingToCutsceneSpace = false;
    private bool onAJailSpace = false;
    private int previousSpace = 0;

    public delegate void DoneMoving();
    public DoneMoving OnDoneMoving;

    public delegate void EndTurn();
    public EndTurn OnEndTurn;

    public delegate void UpdateBoardDirection();
    public UpdateBoardDirection OnUpdateBoardDirection;
    public enum BoardDirections { DOWN, RIGHT, UP, LEFT }
    [SerializeField] private BoardDirections boardDirection = BoardDirections.DOWN;
    public BoardDirections BoardDirection { get { return boardDirection; } }

    private PlayerData playerData;

    private bool onTour = false;
    public bool OnTour { get { return onTour; } }
    private bool rolledOnTour = false;
    private bool backOnNormalRoute = true;
    private TourRouteManager tourRouteManager;

    private int jailTurns = 0;
    public int JailTurns { get { return jailTurns; } }
    private PlayerButtonUI buttonUI;

    // Start is called before the first frame update
    void Start()
    {
        spacesManager = FindObjectOfType<SpacesManager>();
        playerData = GetComponent<PlayerData>();
        buttonUI = FindObjectOfType<PlayerButtonUI>();
    }

    // Update is called once per frame
    void Update()
    {
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

        Vector3 pos = tourRouteManager.GiveFirstBoardSpaceLocation(currentBoardSpace.PropertyCardOnSpace.PropertySetIndex);

        spacePos = spacesManager.CheckOtherPlayersOnSpace(currentBoardPosition, pos, true, playerData.PlayerNumber) + Vector3.up * extraY;
        goTowardsSpace = true;
    }

    public void ReceiveTurn()
    {
        canSpin = true;

        if (onTour)
        {
            tourRouteManager.UpdateShops(currentBoardSpace.PropertyCardOnSpace);
        }

        if (playerData.InJail && jailTurns >= 3)
        {
            buttonUI.ShowPayJailButton(true);
        }
    }

    public void SpinWheel(bool lucky)
    {
        if (lucky)
        {
            playerData.CheckLuckyNumber(Random.Range(1, 7));
        }

        if (!canSpin)
        {
            return;
        }

        canSpin = false;

        if (!moveDebugSpaces)
        {
            CalculatePosition(Random.Range(1, 7));
        }
        else
        {
            CalculatePosition(debugMove);
        }       
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

        if (playerData.InJail)
        {
            if (spinnedNumber != 6)
            {
                if (jailTurns < 3)
                {
                    jailTurns++;
                    OnDoneMoving();
                    return;
                }
                else
                {
                    jailTurns = 0;
                    playerData.GetOutOfJail(true);
                }
            }
            else
            {
                playerData.GetOutOfJail(false);
            }
        }

        onAJailSpace = currentBoardPosition < 0;

        if (onAJailSpace)
        {
            currentBoardPosition = 6;
            onAJailSpace = true;
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

        MoveToSpaceData moveToSpaceData = spacesManager.GetMoveToSpaces(numberOfSpacesBetween, currentBoardPosition, onAJailSpace);
        spacePosistions = moveToSpaceData.spacePositions;
        cutsceneSpaceIndexs = moveToSpaceData.cutsceneSpaceIndexs;
        startMoveIndex = moveToSpaceData.startIndex;
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
                bool startPassed = moveToSpaceIndex == startMoveIndex;

                if (moveToSpaceIndex > spacePosistions.Count - 1)
                {
                    previousSpace = currentBoardPosition;
                    playerData.CheckCurrentSpace(currentBoardSpace);

                    if (startPassed)
                    {
                        playerData.Money += 400;
                    }
                }
                else
                {
                    StartMoving();

                    if (startPassed)
                    {
                        playerData.Money += 200;
                    }
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
                ChangeBoardDirection(1);
                extraSpace.OnPlayCutscene(transform);
            }
        }
    }

    private void TourCheck()
    {
        if (!rolledOnTour)
        {
            OnDoneMoving();
            return;
        }

        if (backOnNormalRoute)
        {
            onTour = false;
            OnDoneMoving();
        }
        else
        {
            PropertyCard propertyCard = currentBoardSpace.PropertyCardOnSpace;

            foreach (int pos in propertyCard.ShopLocations)
            {
                if (moveToSpaceIndex == pos)
                {
                    playerData.GiveMoneyToOtherPlayer(propertyCard.TourFee[propertyCard.UpgradeLevel], currentBoardSpace.PropertyCardOnSpace.PlayerOwningThis);
                }                
            }

            ReturnToBoard();
        }
    }

    private void ReturnToBoard()
    {
        SetStartingVariablesForStartMoving();

        spacePos = spacesManager.CheckOtherPlayersOnSpace(currentBoardPosition, currentBoardSpace.transform.position, false, playerData.PlayerNumber) + Vector3.up * extraY;
        backOnNormalRoute = true;
        goTowardsSpace = true;
    }
    
    public void TeleportToCurrentGivenSpace()
    {
        BoardSpace spaceToGoTo = currentBoardSpace.SpaceToTransportTo;

        TeleportToGivenSpace(spaceToGoTo);
    }

    private void TeleportToGivenSpace(BoardSpace spaceToGoTo)
    {
        currentBoardPosition = spaceToGoTo.BoardIndex;

        StartCoroutine(FallToGivenSpacePos(spaceToGoTo));
    }

    public void ChangeBoardDirection(int changeNum)
    {
        int i = (int)boardDirection + changeNum;

        int enumLength = System.Enum.GetNames(typeof(BoardDirections)).Length;

        if (i > enumLength - 1)
        {
            i -= enumLength;
        }

        boardDirection = (BoardDirections)i;

        OnUpdateBoardDirection();
    }

    private IEnumerator FallToGivenSpacePos(BoardSpace space)
    {
        Vector3 startPos = transform.position;
        Vector3 flyPos = transform.position + Vector3.up * extraTrainSpaceYToStartFalling;

        float fracComplete = 0;
        float startTime = Time.time;

        while (fracComplete < 1)
        {
            fracComplete = (Time.time - startTime) / fallTime;

            transform.position = Vector3.Lerp(startPos, flyPos, fracComplete);

            yield return null;
        }

        BoardSpace spaceToGoTo = space;

        Vector3 modifiedPos = spacesManager.CheckOtherPlayersOnSpace(spaceToGoTo.BoardIndex, spaceToGoTo.transform.position, false, playerData.PlayerNumber);

        transform.position = modifiedPos + Vector3.up * extraTrainSpaceYToStartFalling;
        startPos = transform.position;
        Vector3 trainSpacePos = modifiedPos + Vector3.up * extraY;
        fracComplete = 0;
        startTime = Time.time;

        while (fracComplete < 1)
        {
            fracComplete = (Time.time - startTime) / fallTime;

            transform.position = Vector3.Lerp(startPos, trainSpacePos, fracComplete);

            yield return null;
        }

        currentBoardSpace = spaceToGoTo;

        if (previousSpace > 20 && currentBoardPosition < 10 && currentBoardPosition > 0)
        {
            playerData.Money += 200;
        }

        previousSpace = currentBoardSpace.BoardIndex;

        if (currentBoardPosition < 0)
        {
            boardDirection = BoardDirections.DOWN;
            OnUpdateBoardDirection();
        }
        else
        {
            ChangeBoardDirection(1);
        }

        OnDoneMoving();
    }
}
