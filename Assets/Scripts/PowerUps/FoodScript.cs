using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;

public class FoodScript : NetworkComponent
{
    public bool PickedUp = false;
    public GameObject OwnerPlayer;

    ///Powerup variables
    public float SpeedMulti = 2f;
    public float PowerDuration = 5f;



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


    }
    public void UsePower()
    {
        float tempSpeed = OwnerPlayer.GetComponent<PlayerMovement>().speed *= SpeedMulti;
        OwnerPlayer.GetComponent<PlayerMovement>().speed = tempSpeed;
        StartCoroutine(EndPowerUp());
    }

    public IEnumerator EndPowerUp()
    {
        yield return new WaitForSeconds(PowerDuration);
        float tempSpeed = OwnerPlayer.GetComponent<PlayerMovement>().speed /= SpeedMulti;
        OwnerPlayer.GetComponent<PlayerMovement>().speed = tempSpeed;
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
            }
        }
    }
}
