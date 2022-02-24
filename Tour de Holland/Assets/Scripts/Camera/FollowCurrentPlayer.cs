using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCurrentPlayer : MonoBehaviour
{
    private PlayerManager playerManager;
    private Transform playerToFollow;

    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float smoothSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.OnPlayersInitialized += UpdateCurrentPlayer;
        playerManager.OnPlayerNextTurn += UpdateCurrentPlayer;        
    }

    private void LateUpdate()
    {
        if (playerToFollow == null)
        {
            return;
        }

        Vector3 desiredPos = playerToFollow.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothSpeed * Time.deltaTime);
    }

    private void UpdateCurrentPlayer()
    {
        playerToFollow = playerManager.Players[playerManager.CurrentTurn].transform;
    }
}
