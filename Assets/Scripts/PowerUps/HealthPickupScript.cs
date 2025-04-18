using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;

public class HealthPickupScript : NetworkComponent
{
    public bool PickedUp = false;
    public GameObject OwnerPlayer;



    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if (flag == "PICKEDUP")
            {
                //do visual effects for pickup
                //disable floating object effect
                

            }
            if (flag == "USEPOWER")
            {
                //use automatically when picked up?
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
                OwnerPlayer.GetComponent<NetworkComponent>().SendUpdate("UI", "1");


                if (OwnerPlayer.GetComponent<PlayerCharacter>().PlayerHp < 3)
                {
                    OwnerPlayer.GetComponent<PlayerCharacter>().PlayerHp += 1;
                    OwnerPlayer.GetComponent<PlayerCharacter>().SendUpdate("HEAL", "1");
                }

                MyCore.NetDestroyObject(this.gameObject.GetComponent<NetworkID>().NetId);

            }
        }
    }
}
