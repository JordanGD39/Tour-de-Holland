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

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Spin()
    {
        if (!isSpinning)
        {
            rigid.angularDrag = 0.05f;
            spinVelocity = Random.Range(130, 200);
            rigid.AddTorque(transform.up * spinVelocity);
            isSpinning = true;
        }
       else if (isSpinning)
        {
            rigid.angularDrag = 20;
            isSpinning = false;
        }
    }
}
