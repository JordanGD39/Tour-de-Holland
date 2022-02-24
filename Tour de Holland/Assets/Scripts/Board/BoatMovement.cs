using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    [SerializeField] private Transform moveToTransform;
    [SerializeField] private float lerpSpeed = 5;
    [SerializeField] private float rotateSpeed = 5;
    [SerializeField] private float distanceToPoint = 2;

    private Vector3 startPos;
    private Transform player;
    private ExtraSpace extraSpace;
    private bool startMovingTowardsPoint = false;
    private bool startMovingTowardsStart = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;

        extraSpace = GetComponentInChildren<ExtraSpace>();
        extraSpace.OnPlayCutscene = StartMoving;
    }

    private void StartMoving(Transform aPlayer)
    {
        startMovingTowardsPoint = true;
        player = aPlayer;

        player.SetParent(transform, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (startMovingTowardsPoint)
        {
            Vector3 moveToPos = moveToTransform.position;
            moveToPos.y = transform.position.y;

            MoveTowardsPoint(moveToPos, false);

            if (Vector3.Distance(transform.position, moveToPos) < distanceToPoint)
            {
                player.SetParent(null, true);
                startMovingTowardsPoint = false;
                startMovingTowardsStart = true;
                extraSpace.OnCutsceneDone();
            }
        }
        else if (startMovingTowardsStart)
        {
            MoveTowardsPoint(startPos, true);
        }
    }

    private void MoveTowardsPoint(Vector3 moveToPos, bool invert)
    {
        transform.position = Vector3.Lerp(transform.position, moveToPos, lerpSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(invert ? transform.position - moveToPos : moveToPos - transform.position);

        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
}
