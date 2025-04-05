using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

[RequireComponent(typeof(Rigidbody))]
public class NetworkRigidbody : NetworkComponent
{
    //sync vars
    public Vector3 lastPosition, lastRotation, lastVelocity, lastAngularVelocity;

    //unsync vars
    public float threshold = 1.0f, emergencyThreshold = 1.0f;
    public bool useAdjustedVelocity = false;
    public Vector3 adjustedVelocity = Vector3.zero;
    public Rigidbody rb;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "CHANGE_POSITION" && IsClient)
        {
            lastPosition = NetworkCore.Vector3FromString(value);

            if (useAdjustedVelocity)
            {
                adjustedVelocity = lastPosition - rb.position;
            }

            if ((lastPosition - rb.position).magnitude > emergencyThreshold)
            {
                rb.position = lastPosition;
                adjustedVelocity = Vector3.zero;
            }
        }

        if (flag == "CHANGE_VELOCITY" && IsClient)
        {
            lastVelocity = NetworkCore.Vector3FromString(value);

            if (lastVelocity.magnitude <= threshold)
            {
                adjustedVelocity = Vector3.zero;
            }
        }

        if(flag == "CHANGE_ROTATION" && IsClient)
        {
            lastRotation = NetworkCore.Vector3FromString(value);
            rb.rotation = Quaternion.Euler(lastRotation);
        }

        if( flag == "CHANGE_ANGULAR_VELOCITY" && IsClient)
        {
            lastAngularVelocity = NetworkCore.Vector3FromString(value);
        }


    }

    public override void NetworkedStart()
    {
        //tee hee nothing here
    }

    public override IEnumerator SlowUpdate()
    {
        while(IsConnected)
        {
            if(IsServer)
            {
                if ((rb.position - lastPosition).magnitude > threshold)
                {
                    lastPosition = rb.position;
                    SendUpdate("CHANGE_POSITION", rb.position.ToString());
                }

                if ((rb.rotation.eulerAngles - lastRotation).magnitude > threshold)
                {
                    lastRotation = rb.rotation.eulerAngles;
                    SendUpdate("CHANGE_ROTATION", rb.rotation.eulerAngles.ToString());
                }

                if ((rb.velocity - lastVelocity).magnitude > threshold)
                {
                    lastVelocity = rb.velocity;
                    SendUpdate("CHANGE_VELOCITY", rb.velocity.ToString());
                }

                if ((rb.angularVelocity - lastAngularVelocity).magnitude > threshold)
                {
                    lastAngularVelocity = rb.angularVelocity;
                    SendUpdate("CHANGE_ANGULAR_VELOCITY", rb.angularVelocity.ToString());
                }


                if (IsDirty)
                {
                    SendUpdate("CHANGE_POSITION", rb.position.ToString());
                    SendUpdate("CHANGE_ROTATION", rb.rotation.eulerAngles.ToString());
                    SendUpdate("CHANGE_VELOCITY", rb.velocity.ToString());
                    SendUpdate("CHANGE_ANGULAR_VELOCITY", rb.angularVelocity.ToString());
                    IsDirty = false;
                }
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient)
        {
            rb.velocity = lastVelocity;
            bool hardlyMoving = (lastVelocity.magnitude > threshold);
            if (hardlyMoving)
            {
                rb.velocity += adjustedVelocity;
            }
            rb.angularVelocity = lastAngularVelocity;
        }
    }
}
