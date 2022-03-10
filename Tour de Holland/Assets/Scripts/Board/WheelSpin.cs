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

    [SerializeField]
    private float minLerpTime = 3;
    [SerializeField] private float maxLerpTime = 5;
    [SerializeField]
    private float targetDrag = 20;

    [SerializeField] private Transform rayPoint;
    private Transform currentTopNumber;

    [SerializeField] private float spinWithoutSlowdownTime = 2;
    [SerializeField] private float waitTime = 2;

    public delegate void NumberDetermined(int num);
    public NumberDetermined OnNumberDetermined;

    public delegate void SpinningWheel();
    public SpinningWheel OnSpinningWheel;

    public delegate void SpinStop();
    public SpinStop OnSpinStop;

    private int currentNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isSpinning)
        {
            RaycastHit hit;

            if (Physics.Raycast(rayPoint.position, -Vector3.up, out hit, 100))
            {
                if (hit.collider.gameObject.CompareTag("Number") && currentTopNumber != hit.collider.transform)
                {
                    currentTopNumber = hit.collider.transform;
                    AudioManager.instance.Play("WheelTickSFX");
                }
            }
        }
    }

    public void Spin()
    {
        if (isSpinning)
        {
            return;
        }

        OnSpinningWheel();

        rigid.angularDrag = 0f;
        isSpinning = true;
        StartCoroutine(nameof(SpinCoroutine));
    }

    private IEnumerator SpinCoroutine()
    {
        yield return new WaitForSeconds(waitTime);

        spinVelocity = Random.Range(300, 500);
        rigid.AddTorque(transform.up * spinVelocity);

        yield return new WaitForSeconds(spinWithoutSlowdownTime);

        float startTime = Time.time;
        float startDrag = rigid.angularDrag;
        float lerpTime = Random.Range(minLerpTime, maxLerpTime);

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
                currentNumber = hit.collider.transform.GetSiblingIndex() + 1;
                
                AudioManager.instance.Play("WheelResultSFX");
                OnSpinStop();

                Invoke(nameof(CallOnNumberDetermined), waitTime);
            }
        }

        isSpinning = false;
    }

    private void CallOnNumberDetermined()
    {
        OnNumberDetermined(currentNumber);
    }
}
