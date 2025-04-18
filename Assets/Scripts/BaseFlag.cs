using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;

public class BaseFlag : NetworkComponent
{
    public bool PickedUp = false;
    public string Team; //Team1 or Team2



    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if(flag == "PICKUP")
            {
                //do visual effects for pickup
                //disable floating object effect
                //this.GetComponent<MeshRenderer>().enabled = false;
                transform.GetChild(0).gameObject.SetActive(false);
            }
            if(flag == "RESPAWN")
            {
                //this.GetComponent<MeshRenderer>().enabled = true;
                transform.GetChild(0).gameObject.SetActive(true);
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
    //call this on enemy score or on friendly flag return
    public void Respawn()
    {
        PickedUp = false;
        transform.GetChild(0).gameObject.SetActive(true);
        SendUpdate("RESPAWN", "1");
    }

    public void Pickup()
    {
        PickedUp = true;
        transform.GetChild(0).gameObject.SetActive(false);
        SendUpdate("PICKUP", "1");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            
        }
    }
}
