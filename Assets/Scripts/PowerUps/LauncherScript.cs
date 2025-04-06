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
                PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();
                int playerNum = int.Parse(value);
                foreach (PlayerCharacter player in players)
                {
                    if (player.PlayerNum == playerNum)
                    {
                        OwnerPlayer = player.gameObject;
                        //also assign self to the player
                        player.PowerUp = this.gameObject;
                    }
                }

                launchPoint = OwnerPlayer.transform.GetChild(6).gameObject;//make sure to chnage the child num if it changes

                //do visual effects for pickup
                //disable floating object effect
            }

            if(flag == "AIMPOWER")
            {
                isAiming = true;
            }

            if (flag == "USEPOWER")
            {
                isAiming = false;
            }
        }

        if (IsServer)
        {
            SendUpdate("AIMPOWER", "1");
        }
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
            if(isAiming)
            {
                //line visualizer
                if (launchPoint != null && curve != null)
                {
                    Vector3 velocity = launchPoint.transform.forward * launchSpeed;

                    curve.VisualizeProjectileCurve(
                        launchPoint.transform.position,
                        0f, //start point offset
                        velocity,
                        0.1f, // projectile radius
                        0.1f, // end point offset
                        true, // debug draw
                        out updatedProjectileStartPosition,
                        out hitInfo
                    );
                }
                //end line visualizer
            }




        }
    }

    public void OnTriggerEnter(Collider other)
    {
            if (other.gameObject.CompareTag("Player"))
            {
                if (PickedUp)
                {
                    return;
                }

                PickedUp = true;
                OwnerPlayer = other.gameObject;
                SendUpdate("PICKEDUP", other.GetComponent<PlayerCharacter>().PlayerNum.ToString());
            }
    
    }
}
