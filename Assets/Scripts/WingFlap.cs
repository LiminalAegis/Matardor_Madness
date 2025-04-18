using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingFlap : MonoBehaviour
{
    public Transform leftWing;
    public Transform rightWing;

    public float flapSpeed = 2f;
    public float flapAngle = 20f;

    public float flapTimer = 0f;

    public Quaternion leftStartRot;
    public Quaternion rightStartRot;

    // Start is called before the first frame update
    void Start()
    {
        leftStartRot = leftWing.localRotation;
        rightStartRot = rightWing.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        flapTimer += Time.deltaTime * flapSpeed;
        float flapValue = Mathf.Sin(flapTimer);
        float angle = flapValue * flapAngle;

        //leftWing.localRotation = Quaternion.Euler(leftWing.rotation.x, angle, leftWing.rotation.z);
        //rightWing.localRotation = Quaternion.Euler(rightWing.rotation.x, -angle, rightWing.rotation.z);
        //leftWing.localRotation = Quaternion.Euler(0, -angle, 0);
        //rightWing.localRotation = Quaternion.Euler(0, angle, 0);

        leftWing.localRotation = leftStartRot * Quaternion.Euler(0, angle, 0);
        rightWing.localRotation = rightStartRot * Quaternion.Euler(0, angle, 0);
    }
}
