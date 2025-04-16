using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;

public class FlareScript : NetworkComponent
{
    public bool PickedUp = false;
    public GameObject OwnerPlayer;

    public bool isAiming = false;



    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if (flag == "PICKEDUP")
            {

                //do visual effects for pickup
                //disable floating object effect
                transform.GetChild(0).gameObject.SetActive(false);
            }

        }

        if (IsServer)
        {

        }

    }

    public void UsePower()
    {
        OwnerPlayer.GetComponent<FlareCollisionScript>().Activate();
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
                other.gameObject.GetComponent<PlayerCharacter>().PowerUp = this.gameObject;
                transform.GetChild(0).gameObject.SetActive(false);

                SendUpdate("PICKEDUP", other.GetComponent<PlayerCharacter>().PlayerNum.ToString());
            }
        }
    }
}
