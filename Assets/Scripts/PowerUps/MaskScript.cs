using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;

public class MaskScript : NetworkComponent
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

                //disable players damage from bulls until they run into one

                //do visual effects for pickup
                //disable floating object effect

            }
            if (flag == "USEPOWER")
            {
                //for this one maybe we just have a collider attach to player and if they run into
                //a bull its used.  passive rather than active effect
            }

        }

        if (IsServer)
        {
          
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
        if(IsServer)
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

            if(other.gameObject.CompareTag("BULL"))
            {
                if (!PickedUp)
                {
                    return;
                }
                //make the bull do its tired effect?
            }
        }
    }
}
