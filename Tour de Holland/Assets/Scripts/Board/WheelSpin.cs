using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpin : MonoBehaviour
{
    private Rigidbody rigid;
    [SerializeField]
    private float spinTime;
    [SerializeField]
    private bool isSpinning = false;
    [SerializeField]
    private float spinVelocity;
    private bool canSpin = true;

    [SerializeField]
    private float lerpTime = 3;
    [SerializeField]
    private float targetDrag = 20;

    [SerializeField]
    private Transform rayPoint;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }


    public void Spin()
    {
        if (!canSpin)
        {
            return;
        }

        if (!isSpinning)
        {
            rigid.angularDrag = 0f;
            spinVelocity = Random.Range(230, 300);
            rigid.AddTorque(transform.up * spinVelocity);
            isSpinning = true;
        }
       else if (isSpinning)
        {
            canSpin = false;
            StartCoroutine(LerpDrag());
            isSpinning = false;
        }
    }

    private IEnumerator LerpDrag()
    {
        float startTime = Time.time;
        float startDrag = rigid.angularDrag;

        while (rigid.angularDrag < targetDrag)
        {
            float frac = (Time.time - startTime) / lerpTime;
            rigid.angularDrag = Mathf.Lerp(startDrag, targetDrag, frac);
            yield return null;
        }

        RaycastHit hit;

        if (Physics.Raycast(rayPoint.position, -Vector3.up, out hit, 100))
        {
            if (hit.collider.gameObject.CompareTag("Number"))
            {
                int number = hit.collider.transform.GetSiblingIndex() + 1;
                Debug.Log(number);
            }
        }
    }
}
