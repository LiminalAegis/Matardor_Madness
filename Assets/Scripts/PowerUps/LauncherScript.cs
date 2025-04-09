using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;
using ProjectileCurveVisualizerSystem;

public class LauncherScript : NetworkComponent
{
    public bool PickedUp = false;
    public GameObject OwnerPlayer;

    public float launchSpeed = 20f;
    public GameObject launchPoint;
    public bool isAiming = false;

    //visualizer variables
    private Vector3 updatedProjectileStartPosition;
    private RaycastHit hitInfo;
    public ProjectileCurveVisualizer curve; // visualizer prefab 
    // end visualizer variables


    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if (flag == "PICKEDUP")
            {

                //do visual effects for pickup
                //disable floating object effect
                this.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        if (IsServer)
        {
        }
    }
    public void UsePower()
    {
        //spawn the projectile
        GameObject ThrownFlag = MyCore.NetCreateObject(
                            1, //whatever number it ends up
                            this.Owner, //server owned?
                            launchPoint.transform.position,
                            launchPoint.transform.rotation
                        );
        ThrownFlag.GetComponent<Rigidbody>().velocity = launchPoint.transform.forward * launchSpeed;

        MyCore.NetDestroyObject(this.gameObject.GetComponent<NetworkID>().NetId);

    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {
            if (IsServer)
            {

            }
            yield return new WaitForSeconds(.1f);
        }
    }

    void Start()
    {
        if (curve == null)
        {
            curve = FindObjectOfType<ProjectileCurveVisualizer>();
        }
    }

    void Update()
    {
        if (IsLocalPlayer)
        {
            




        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (PickedUp)
                {
                    return;
                }

                PickedUp = true;
                OwnerPlayer = other.gameObject;
                other.gameObject.GetComponent<PlayerCharacter>().PowerUp = this.gameObject;
                this.GetComponent<MeshRenderer>().enabled = false;

                SendUpdate("PICKEDUP", other.GetComponent<PlayerCharacter>().PlayerNum.ToString());
                other.gameObject.GetComponent<PlayerMovement>().SendUpdate("LAUNCHER", "true");
                
            }
        }
    
    }
}
