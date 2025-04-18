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
                transform.GetChild(0).gameObject.SetActive(false);
                if (IsLocalPlayer)
                {
                    PlayerUI ui = FindObjectOfType<PlayerUI>();
                    ui.PowerUpVisual(4);
                }
            }
        }

        if (IsServer)
        {
        }
    }
    public void UsePower()
    {
        //make teh flag throw pole ways
        Quaternion adjustRotation = Quaternion.LookRotation(launchPoint.transform.up, launchPoint.transform.forward);
        //cant get the test poel to shoot out bottom first. maybe just spawn it with world rot depending on the player rotation
        //honestly maybe just have the model and hit aoe be on teh top of prefab


        //spawn the projectile
        GameObject ThrownFlag = MyCore.NetCreateObject(
                            9, //whatever number it ends up
                            this.Owner, //server owned?
                            launchPoint.transform.position,
                            adjustRotation
                        );
        ThrownFlag.GetComponent<Rigidbody>().velocity = launchPoint.transform.forward * launchSpeed;
        ThrownFlag.GetComponent<FlagThrowScript>().Team = OwnerPlayer.GetComponent<PlayerCharacter>().PTeam;

        OwnerPlayer.GetComponent<PlayerCharacter>().PlayerScore -= 1;

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
        StartCoroutine(DespawnTimer());
    }

    public IEnumerator DespawnTimer()
    {
        yield return new WaitForSeconds(30f);
        if (!PickedUp)
        {
            MyCore.NetDestroyObject(this.gameObject.GetComponent<NetworkID>().NetId);
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
                launchPoint = OwnerPlayer.GetComponent<PlayerCharacter>().LaunchPoint;

                SendUpdate("PICKEDUP", other.GetComponent<PlayerCharacter>().PlayerNum.ToString());
                other.gameObject.GetComponent<PlayerMovement>().SendUpdate("LAUNCHER", "true");
                
            }
        }
    
    }
}
