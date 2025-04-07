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
                    }
                }

                //do visual effects for pickup
                //disable floating object effect

            }
            if (flag == "USEPOWER")
            {
                //OwnerPlayer.GetComponent<PlayerCharacter>().Speed *=2;
                StartCoroutine(EndPowerUp());
            }

        }

        if (IsServer)
        {
            if(flag == "DESTROY")
            {
                MyCore.NetDestroyObject(this.gameObject.GetComponent<NetworkID>().NetId);
            }
        }

    }
    public IEnumerator EndPowerUp()
    {
        yield return new WaitForSeconds(5);
        //OwnerPlayer.GetComponent<PlayerCharacter>().Speed /= 2;
        SendCommand("DESTROY", "1");
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

            if (other.gameObject.CompareTag("PLAYER"))
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
}
