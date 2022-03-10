using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCurrentPlayer : MonoBehaviour
{
    private PlayerManager playerManager;
    private Transform playerToFollow;
    private WheelSpin wheelSpin;

    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private float smoothToPlayerSpeed = 2;
    [SerializeField] private float smoothTurnSpeed = 2;
    [SerializeField] private float rotateSpeed = 2;
    [SerializeField] private Transform wheelFocus;

    private Vector3 usedOffset;
    private float smoothSpeed;
    private bool focusedOnPlayer = true;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.OnPlayersInitialized += UpdateCurrentPlayer;
        playerManager.OnPlayersInitialized += UpdateDirection;
        playerManager.OnPlayerNextTurn += UpdateCurrentPlayer;

        wheelSpin = FindObjectOfType<WheelSpin>();
        wheelSpin.OnSpinningWheel += ShowWheel;
        wheelSpin.OnSpinStop += UpdateCurrentPlayer;
    }

    private void LateUpdate()
    {
        if (playerToFollow == null)
        {
            return;
        }

        Vector3 desiredPos;

        if (focusedOnPlayer)
        {
            desiredPos = playerToFollow.position + usedOffset;
        }
        else
        {
            desiredPos = wheelFocus.position;
        }

        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(playerToFollow.position - transform.position);

        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    private void UpdateDirection()
    {
        PlayerMovement playerMovement = playerManager.Players[playerManager.CurrentTurn].playerMovement;

        Vector3 vec;

        switch (playerMovement.BoardDirection)
        {
            case PlayerMovement.BoardDirections.DOWN:
                usedOffset = offset;
                break;
            case PlayerMovement.BoardDirections.RIGHT:
                vec = offset;
                vec.x = -offset.z;
                vec.z = offset.x;
                usedOffset = vec;
                break;
            case PlayerMovement.BoardDirections.UP:
                vec = offset;
                vec.z = -offset.z;
                usedOffset = vec;
                break;
            case PlayerMovement.BoardDirections.LEFT:
                vec = offset;
                vec.x = offset.z;
                vec.z = offset.x;
                usedOffset = vec;
                break;
        }
    }

    private void UpdateCurrentPlayer()
    {
        focusedOnPlayer = true;
        smoothSpeed = smoothToPlayerSpeed;

        PlayerMovement playerMovement = playerManager.Players[playerManager.CurrentTurn].playerMovement;

        playerToFollow = playerMovement.transform;
        playerMovement.OnUpdateBoardDirection = () => { UpdateDirection(); smoothSpeed = smoothTurnSpeed;};

        UpdateDirection();
    }

    private void ShowWheel()
    {
        focusedOnPlayer = false;
        playerToFollow = wheelSpin.transform.parent;
    }
}
