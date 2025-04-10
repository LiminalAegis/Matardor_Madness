using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;
using TMPro;

public class FlagDrop : NetworkComponent
{
    public bool PickedUp = false;
    public string Team; //Team1 or Team2
    public int Score;



    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if (flag == "PICKUP")
            {
                //do visual effects for pickup
                //disable floating object effect
                //this.GetComponent<MeshRenderer>().enabled = false;
                transform.GetChild(0).gameObject.SetActive(false);
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
        if (IsServer)
        {

        }
    }
}
